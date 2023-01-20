namespace MQTT_GUI.MQTT.models
{
    public enum MessageType : byte
    {
        Connect = 1 << 4,
        ConnAck = 2 << 4,
        Publish = 3 << 4,
        PubAck = 4 << 4,
        PubRec = 5 << 4,
        PubRel = 6 << 4,
        PubComp = 7 << 4,
        Subscribe = 8 << 4,
        SubAck = 9 << 4,
        Unsubscribe = 10 << 4,
        UnsubAck = 11 << 4,
        PingReq = 12 << 4,
        PingResp = 13 << 4,
        Disconnect = 14 << 4
    }
}