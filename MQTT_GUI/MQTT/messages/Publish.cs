using System;
using System.Collections.Generic;
using System.Text;
using MQTT_GUI.MQTT.models;

namespace MQTT_GUI.MQTT.messages
{
    public class Publish : models.MQTT
    {
        public Publish(string topic, string message)
        {
            ControlHeader = (byte) MessageType.Publish;
            RemainingLength = new[] {(byte) (2 + topic.Length + message.Length)};
            var headerLength = BitConverter.GetBytes((short) topic.Length);

            var header = new List<byte>
            {
                headerLength[1],
                headerLength[0]
            };
            header.AddRange(Encoding.ASCII.GetBytes(topic));

            Header = header.ToArray();
            
            var payloadLength = BitConverter.GetBytes((short) message.Length);

            var payload = new List<byte>();
            payload.AddRange(Encoding.ASCII.GetBytes(message));

            Payload = payload.ToArray();
        }
    }
}