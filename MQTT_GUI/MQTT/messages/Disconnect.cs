using System;
using MQTT_GUI.MQTT.models;

namespace MQTT_GUI.MQTT.messages
{
    public class Disconnect : models.MQTT
    {
        public Disconnect()
        {
            ControlHeader = (byte) MessageType.Disconnect;
            RemainingLength = new byte[] {0};
            Header = Array.Empty<byte>();
            Payload = Array.Empty<byte>();
        }
    }
}