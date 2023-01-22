using System;
using System.Net.Sockets;

namespace MQTT_GUI.MQTT
{
    public class MQTTClient
    {
        public static MQTTClient Client;
        private static string _ip;
        private static int _port;
        public ReceiveThread Receiver;
        public TcpClient TcpClient;

        public MQTTClient(string ipAddress, int port)
        {
            _ip = ipAddress;
            _port = port;
        }

        public MQTTClient()
        {
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
            catch (Exception)
            {
                return false;
            }

            if (!TcpClient.Connected) return false;
            Receiver = new ReceiveThread(this);
            return true;
        }

        public void SendObject(models.MQTT command)
        {
            var stream = TcpClient.GetStream();
            var bytes = command.GetBytes();
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}