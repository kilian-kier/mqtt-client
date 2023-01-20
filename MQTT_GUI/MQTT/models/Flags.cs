namespace MQTT_GUI.MQTT.models
{
    public enum Flags : byte
    {
        Duplicate = 8,
        ExactlyOnce = 4,
        AtLeastOnce = 2,
        Retain = 1,
        AtMostOnce = 0
    }
}