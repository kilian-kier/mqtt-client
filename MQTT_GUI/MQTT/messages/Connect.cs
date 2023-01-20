using System.Collections.Generic;
using System.Text;
using MQTT_GUI.MQTT.models;

namespace MQTT_GUI.MQTT.messages
{
    public class Connect : models.MQTT
    {
        private enum ConnectFlags : byte
        {
            CleanSession = 2,
            Password = 2 ^ 6,
            UserName = 2 ^ 7,
        }

        public Connect(string clientId)
        {
            ControlHeader = (byte) MessageType.Connect;
            RemainingLength = new byte[] {(byte)(12 + clientId.Length)};
            Header = new byte[]
            {
                0x0, 0x4,
                (byte) 'M', (byte) 'Q', (byte) 'T', (byte) 'T', 0x4,
                (byte) ConnectFlags.CleanSession,
                0x0, 0x3c
            };
            var temp = new List<byte>
            {
                0x0, (byte) clientId.Length
            };
            temp.AddRange(Encoding.ASCII.GetBytes(clientId));
            Payload = temp.ToArray();
        }
    }
}