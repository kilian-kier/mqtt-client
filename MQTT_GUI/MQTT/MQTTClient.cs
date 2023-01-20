using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MQTT_GUI.MQTT.messages;

namespace MQTT_GUI.MQTT
{
    public class MQTTClient
    {
        public static MQTTClient Client;
        private readonly string _ip;
        private readonly int _port;
        public ReceiveThread Receiver;
        public TcpClient TcpClient;

        public MQTTClient(string ipAddress, int port)
        {
            _ip = ipAddress;
            _port = port;
        }

        public bool CreateTcpConnection()
        {
            if (TcpClient != null && TcpClient.Connected)
            {
                TcpClient.Close();
            }
            TcpClient = new TcpClient();
            try
            {
                TcpClient.Connect(_ip, _port);
            }
            catch (Exception e)
            {
                return false;
            }

            if (!TcpClient.Connected) return false;
            Receiver = new ReceiveThread(this);
            return true;
        }

        /*public MQTTClient(string ipAddress, int port, string clientId)
        {
            _ip = ipAddress;
            _port = port;
            var connect = new Connect(clientId);
            SendObject(connect);
            
            var connAck = Receiver.GetConnAck();
            if (connAck.Header == null || connAck.Header.Length >= 2 && connAck.Header[1] != 0)
            {
                Console.Error.Write("Can not connect");
                Environment.Exit(1);
            }

            var publish = new Publish("shellies/shelly1/relay/0/command", "toggle");
            SendObject(publish);
            publish = new Publish("shellies/shelly1/relay/0/command", "on");
            SendObject(publish);
            publish = new Publish("shellies/shelly1/relay/0/command", "on");
            SendObject(publish);
            publish = new Publish("shellies/shelly1/relay/0/command", "off");
            SendObject(publish);

            var subscribe = new Subscribe("esp32/temperature");
            SendObject(subscribe);
            _ = Receiver.GetSubAck();
            subscribe = new Subscribe("esp32/humidity");
            SendObject(subscribe);
            _ = Receiver.GetSubAck();
            subscribe = new Subscribe("esp32/heatIndex");
            SendObject(subscribe);
            _ = Receiver.GetSubAck();

            while (true)
            {
                if (Receiver.SubQueue.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }


                var subscribed = Receiver.SubQueue.Dequeue();
                var topic = Encoding.ASCII.GetString(subscribed.Header, 1, subscribed.Header.Length - 1);
                var msg = Encoding.ASCII.GetString(subscribed.Payload, 0, subscribed.Payload.Length);
                Console.Out.WriteLine(topic + ": " + msg);
            }
        }*/

        public void SendObject(models.MQTT command)
        {
            var stream = TcpClient.GetStream();
            var bytes = command.GetBytes();
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}