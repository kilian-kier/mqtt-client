using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using MQTT_GUI.MQTT;
using MQTT_GUI.MQTT.messages;
using MQTT_GUI.MVVM.ViewModel;

namespace MQTT_GUI.MVVM.Views
{
    public partial class SubscribeView : UserControl
    {
        public SubscribeView()
        {
            InitializeComponent();
        }

        private void ButtonClear(object sender, RoutedEventArgs e)
        {
            TopicBox.Clear();
            QoSBox.SelectedIndex = 0;
        }

        private void ButtonSubscribe(object sender, RoutedEventArgs e)
        {
            var topic = TopicBox.Text;

            var qosIdx = QoSBox.SelectedIndex;
            Subscribe.QOS qos;
            switch (qosIdx)
            {
                case 0:
                    qos = Subscribe.QOS.AtMostOnce;
                    break;
                case 1:
                    qos = Subscribe.QOS.ExactlyOnce;
                    break;
                case 2:
                    qos = Subscribe.QOS.AtLeastOnce;
                    break;
                default:
                    qos = Subscribe.QOS.AtMostOnce;
                    break;
            }

            var subscribe = new Subscribe(topic, qos);
            MQTTClient.Client.SendObject(subscribe);
            new Thread(() =>
            {
                var subAck = MQTTClient.Client.Receiver.GetSubAck();
                if (subAck == null)
                {
                    // TODO: Error
                }
                else
                {
                    // TODO: success

                    if (SubscribeViewModel.Unsubscribable.Contains(topic))
                        return;
                    Dispatcher.Invoke(() => { SubscribeViewModel.Unsubscribable.Add(topic); });
                }
            }).Start();
        }

        private void ButtonUnsubscribe(object sender, RoutedEventArgs e)
        {
            var topic = UnsubscribeBox.SelectedValue.ToString();
            new Thread(() =>
            {
                var unsubscribe = new Unsubscribe(topic);
                MQTTClient.Client.SendObject(unsubscribe);
                var unSubAck = MQTTClient.Client.Receiver.GetUnsubAck();
                if (unSubAck == null)
                {
                    // TODO: Error
                    return;
                }

                // TODO: success
                Dispatcher.Invoke(() =>
                {
                    SubscribeViewModel.Unsubscribable.Remove(topic);
                    UnsubscribeBox.SelectedIndex = 0;
                });
                // TODO: remove
                // SubscriptionsViewModel.Remove(topic);
            }).Start();
        }
    }
}