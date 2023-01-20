using System;
using MQTT_GUI.MQTT.models;

namespace MQTT_GUI.MQTT.messages
{
    public class PingReq : models.MQTT
    {
        public PingReq()
        {
            ControlHeader = (byte) MessageType.PingReq;
            RemainingLength = new byte[] {0};
            Header = Array.Empty<byte>();
            Payload = Array.Empty<byte>();
        }
    }
}