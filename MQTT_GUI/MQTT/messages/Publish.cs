using System;
using System.Collections.Generic;
using System.Text;
using MQTT_GUI.MQTT.models;

namespace MQTT_GUI.MQTT.messages
{
    public class Publish : models.MQTT
    {
        private static int _idCounter = 1;

        public enum Qos : byte
        {
            ExactlyOnce = 4,
            AtLeastOnce = 2,
            AtMostOnce = 0
        }

        public Publish(string topic, string message, Qos qos)
        {
            ControlHeader = (byte) (MessageType.Publish + (byte) qos);

            if (qos == 0)
            {
                RemainingLength = new[] {(byte) (2 + topic.Length + message.Length)};
            }
            else
            {
                RemainingLength = new[] {(byte) (4 + topic.Length + message.Length)};
            }

            var headerLength = BitConverter.GetBytes((short) topic.Length);

            var header = new List<byte>
            {
                headerLength[1],
                headerLength[0],
            };

            header.AddRange(Encoding.ASCII.GetBytes(topic));

            if (qos != 0)
            {
                var id = BitConverter.GetBytes((short) _idCounter++);
                header.AddRange(new List<byte>
                {
                    id[1],
                    id[0]
                });
            }

            Header = header.ToArray();

            var payload = new List<byte>();
            payload.AddRange(Encoding.ASCII.GetBytes(message));

            Payload = payload.ToArray();
        }
    }
}