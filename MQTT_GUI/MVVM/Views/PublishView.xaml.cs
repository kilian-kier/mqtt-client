using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MQTT_GUI.MQTT;
using MQTT_GUI.MQTT.messages;

namespace MQTT_GUI.MVVM.Views
{
    public partial class PublishView : UserControl
    {
        public PublishView()
        {
            InitializeComponent();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ButtonPublish(object sender, RoutedEventArgs e)
        {
            var topic = TopicBox.Text;
            var msg = MessageBox.Text;
            var qosIdx = QoSBox.SelectedIndex;
            Publish.QOS qos;
            switch (qosIdx)
            {
                case 0:
                    qos = Publish.QOS.AtMostOnce;
                    break;
                case 1:
                    qos = Publish.QOS.ExactlyOnce;
                    break;
                case 2:
                    qos = Publish.QOS.AtLeastOnce;
                    break;
                default:
                    qos = Publish.QOS.AtMostOnce;
                    break;
            }

            var publish = new Publish(topic, msg, qos);
            MQTTClient.Client.SendObject(publish);
            
            TopicBox.Clear();
            MessageBox.Clear();
        }
    }
}