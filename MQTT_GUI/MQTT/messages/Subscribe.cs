using System;
using System.Collections.Generic;
using System.Text;
using MQTT_GUI.MQTT.models;

namespace MQTT_GUI.MQTT.messages
{
    public class Subscribe : models.MQTT
    {
        private static short _idCounter = 1;
        public Subscribe(string topic)
        {
            ControlHeader = (byte) MessageType.Subscribe + 2;
            RemainingLength = new[] {(byte) (5 + topic.Length)};
            var id = BitConverter.GetBytes(_idCounter);
            _idCounter++;
            var header = new List<byte>
            {
                id[1],
                id[0]
            };
            var headerLength = BitConverter.GetBytes((short) topic.Length);

            header.Add(headerLength[1]);
            header.Add(headerLength[0]);
            
            header.AddRange(Encoding.ASCII.GetBytes(topic));

            Header = header.ToArray();

            Payload = new[] {(byte)Flags.AtMostOnce};
        }
    }
}