using System.Windows;
using System.Windows.Controls;
using MQTT_GUI.MQTT;
using MQTT_GUI.MQTT.messages;

namespace MQTT_GUI.MVVM.Controller
{
    public static class ConnectController
    {
        public static bool Connect(string brokerIp, int port, TextBox errors)
        {
            MQTTClient.Client = new MQTTClient(brokerIp, port);
            if (!MQTTClient.Client.CreateTcpConnection())
            {
                errors.Visibility = Visibility.Visible;
                errors.Text = "Can not connect to MQTT Broker with this IP address and port";
                return false;
            }
            var connectPacket = new Connect("stkiekil");
            MQTTClient.Client.SendObject(connectPacket);
            var connAck = MQTTClient.Client.Receiver.GetConnAck();
            return connAck != null && connAck.Header != null && (connAck.Header.Length < 2 || connAck.Header[1] == 0);
        }

        public static void Disconnect()
        {
            if (MQTTClient.Client == null || MQTTClient.Client.TcpClient == null ||
                !MQTTClient.Client.TcpClient.Connected) return;
            var disconnect = new Disconnect();
            MQTTClient.Client.SendObject(disconnect);
        }
    }
}