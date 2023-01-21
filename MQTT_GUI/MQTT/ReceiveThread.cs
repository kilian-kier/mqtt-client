using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows;
using MQTT_GUI.MQTT.messages;
using MQTT_GUI.MQTT.models;
using Timer = System.Timers.Timer;

namespace MQTT_GUI.MQTT
{
    public class ReceiveThread
    {
        private const int Timeout = 5000;
        private readonly MQTTClient _mqttClient;
        private readonly Queue<models.MQTT> _connAckQueue = new Queue<models.MQTT>();
        private readonly Queue<models.MQTT> _pubRecQueue = new Queue<models.MQTT>();
        private readonly Queue<models.MQTT> _pubAckQueue = new Queue<models.MQTT>();
        private readonly Queue<models.MQTT> _subAckQueue = new Queue<models.MQTT>();
        private readonly Queue<models.MQTT> _unsubAckQueue = new Queue<models.MQTT>();
        private readonly Queue<models.MQTT> _pingRespQueue = new Queue<models.MQTT>();
        public readonly Queue<models.MQTT> SubQueue = new Queue<models.MQTT>();

        public ReceiveThread(MQTTClient client)
        {
            _mqttClient = client;
            var pingTimer = new Timer();
            pingTimer.Interval = 55 * 1000; // every 55 seconds
            pingTimer.Elapsed += SendPingRequest;
            pingTimer.AutoReset = true;
            pingTimer.Enabled = true;

            new Thread(() =>
            {
                while (client.TcpClient == null)
                {
                    Thread.Sleep(10);
                }

                const int connAck = (byte) MessageType.ConnAck >> 4;
                const int pubRec = (byte) MessageType.PubRec >> 4;
                const int pubAck = (byte) MessageType.PubAck >> 4;
                const int subAck = (byte) MessageType.SubAck >> 4;
                const int unsubAck = (byte) MessageType.UnsubAck >> 4;
                const int pingResp = (byte) MessageType.PingResp >> 4;
                const int publish = (byte) MessageType.Publish >> 4;

                while (true)
                {
                    var received = ReceiveObject();
                    foreach (var packet in received)
                    {
                        var type = packet.ControlHeader >> 4;

                        switch (type)
                        {
                            case connAck:
                                _connAckQueue.Enqueue(packet);
                                break;
                            case pubRec:
                                _pubRecQueue.Enqueue(packet);
                                break;
                            case pubAck:
                                _pubAckQueue.Enqueue(packet);
                                break;
                            case subAck:
                                _subAckQueue.Enqueue(packet);
                                break;
                            case unsubAck:
                                _unsubAckQueue.Enqueue(packet);
                                break;
                            case pingResp:
                                _pingRespQueue.Enqueue(packet);
                                break;
                            case publish:
                                SubQueue.Enqueue(packet);
                                break;
                        }
                    }
                }
            }).Start();
        }

        private void SendPingRequest(object source, ElapsedEventArgs e)
        {
            var pingRequest = new PingReq();
            MQTTClient.Client.SendObject(pingRequest);
            _ = GetAck(_pingRespQueue);
        }

        private models.MQTT GetAck(Queue<models.MQTT> queue)
        {
            var ms = 0;
            while (queue.Count == 0)
            {
                Thread.Sleep(10);
                ms += 10;
                if (ms >= Timeout)
                {
                    return null;
                }
            }

            return queue.Dequeue();
        }

        public models.MQTT GetConnAck()
        {
            return GetAck(_connAckQueue);
        }

        public models.MQTT GetPubRec()
        {
            return GetAck(_pubRecQueue);
        }

        public models.MQTT GetPubAck()
        {
            return GetAck(_pubAckQueue);
        }

        public models.MQTT GetSubAck()
        {
            return GetAck(_subAckQueue);
        }

        public models.MQTT GetUnsubAck()
        {
            return GetAck(_unsubAckQueue);
        }

        private List<models.MQTT> ReceiveObject()
        {
            if (!_mqttClient.TcpClient.Connected) return null;
            var stream = _mqttClient.TcpClient.GetStream();
            var data = new byte[1024];
            var ms = new MemoryStream();
            while (!stream.DataAvailable)
            {
                Thread.Sleep(10);
            }

            while (stream.DataAvailable)
            {
                var numBytesRead = stream.Read(data, 0, data.Length);
                ms.Write(data, 0, numBytesRead);
            }

            var bytes = new byte[ms.Length];
            ms.Seek(0, SeekOrigin.Begin);
            ms.Read(bytes, 0, bytes.Length);
            return ParsePacket(ms.ToArray());
        }

        private static List<models.MQTT> ParsePacket(byte[] bytes)
        {
            var ret = new List<models.MQTT>();
            var parsedBytes = 0;
            var mqtt = new models.MQTT();
            mqtt.FromBytes(bytes);
            var flag = mqtt.ControlHeader >> 4;
            const int publishFlag = (byte) MessageType.Publish >> 4;
            if (flag != publishFlag)
            {
                ret.Add(mqtt);
                return ret;
            }

            while (parsedBytes < bytes.Length)
            {
                var remaining = bytes.Length - parsedBytes;
                var packetBytes = new byte[remaining];
                Array.Copy(bytes, parsedBytes, packetBytes, 0, remaining);
                mqtt = new models.MQTT();
                parsedBytes += mqtt.FromSubscribedBytes(packetBytes);
                ret.Add(mqtt);
            }

            return ret;
        }
    }
}