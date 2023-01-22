using System.Threading;
using System.Windows;
using MQTT_GUI.MQTT;
using MQTT_GUI.MQTT.messages;

namespace MQTT_GUI.MVVM.Views
{
    public partial class PublishView
    {
        public PublishView()
        {
            InitializeComponent();
        }

        private void ButtonPublish(object sender, RoutedEventArgs e)
        {
            PubErr.Visibility = Visibility.Collapsed;
            PubSuc.Visibility = Visibility.Collapsed;
            PubInf.Visibility = Visibility.Collapsed;
            var topic = TopicBox.Text;
            var msg = MessageBox.Text;

            if (topic == string.Empty)
            {
                Dispatcher.Invoke(() =>
                {
                    PubErr.Text = "Please enter a topic";
                    PubErr.Visibility = Visibility.Visible;
                });
                return;
            }

            var qosIdx = QoSBox.SelectedIndex;
            Publish.Qos qos;
            switch (qosIdx)
            {
                case 0:
                    qos = Publish.Qos.AtMostOnce;
                    break;
                case 1:
                    qos = Publish.Qos.ExactlyOnce;
                    break;
                case 2:
                    qos = Publish.Qos.AtLeastOnce;
                    break;
                default:
                    qos = Publish.Qos.AtMostOnce;
                    break;
            }

            var publish = new Publish(topic, msg, qos);
            MQTTClient.Client.SendObject(publish);

            if (qosIdx == 0)
            {
                PubInf.Text = "The publish message was sent (arrived at the broker without guarantee)";
                PubInf.Visibility = Visibility.Visible;
            }
            else
            {
                new Thread(() =>
                {
                    Dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Visible; });
                    if (qosIdx == 2)
                    {
                        var pubRec = MQTTClient.Client.Receiver.GetPubAck();
                        Dispatcher.Invoke(() =>
                        {
                            ProgressBar.Visibility = Visibility.Collapsed;
                            if (pubRec == null)
                            {
                                PubErr.Text =
                                    "The publish message was NOT successful! Broker didn't respond with an acknowledgement";
                                PubErr.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                PubSuc.Text = "The publish message was sent successfully";
                                PubSuc.Visibility = Visibility.Visible;
                            }
                        });
                    }
                    else if (qosIdx == 1)
                    {
                        var pubAck = MQTTClient.Client.Receiver.GetPubRec();
                        Dispatcher.Invoke(() =>
                        {
                            ProgressBar.Visibility = Visibility.Collapsed;
                            if (pubAck == null)
                            {
                                PubErr.Text =
                                    "The publish message NOT successful! Broker didn't respond with a receive message";
                                PubErr.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                PubSuc.Text = "The broker received the publish message";
                                PubSuc.Visibility = Visibility.Visible;
                            }
                        });
                    }
                }).Start();
            }
        }

        private void ButtonClear(object sender, RoutedEventArgs e)
        {
            TopicBox.Clear();
            MessageBox.Clear();
            QoSBox.SelectedIndex = 0;
        }
    }
}