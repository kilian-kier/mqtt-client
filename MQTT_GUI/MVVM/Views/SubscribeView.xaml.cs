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
            SubErr.Visibility = Visibility.Collapsed;
            SubSuc.Visibility = Visibility.Collapsed;
        }

        private void ButtonSubscribe(object sender, RoutedEventArgs e)
        {
            SubErr.Visibility = Visibility.Collapsed;
            SubSuc.Visibility = Visibility.Collapsed;
            var topic = TopicBox.Text;
            
            if (string.IsNullOrEmpty(topic))
            {
                Dispatcher.Invoke(() =>
                {
                    SubErr.Text = "You must enter a topic to subscribe";
                    SubErr.Visibility = Visibility.Visible;
                });
                return;
            }

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
                    Dispatcher.Invoke(() =>
                    {
                        SubErr.Text = "The subscribe message was NOT successful! Broker didn't respond with an acknowledgement";
                        SubErr.Visibility = Visibility.Visible;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        SubSuc.Text = "The subscribe message was sent successfully";
                        SubSuc.Visibility = Visibility.Visible;
                    });

                    if (SubscribeViewModel.Unsubscribable.Contains(topic))
                        return;
                    Dispatcher.Invoke(() => { SubscribeViewModel.Unsubscribable.Add(topic); });
                }
            }).Start();
        }

        private void ButtonUnsubscribe(object sender, RoutedEventArgs e)
        {
            SubErr.Visibility = Visibility.Collapsed;
            SubSuc.Visibility = Visibility.Collapsed;
            if (UnsubscribeBox.SelectedValue == null)
            {
                Dispatcher.Invoke(() =>
                {
                    SubErr.Text = "You must first subscribe to a topic in order to unsubscribe from it";
                    SubErr.Visibility = Visibility.Visible;
                });
                return;
            }
            var topic = UnsubscribeBox.SelectedValue.ToString();
            new Thread(() =>
            {
                var unsubscribe = new Unsubscribe(topic);
                MQTTClient.Client.SendObject(unsubscribe);
                var unSubAck = MQTTClient.Client.Receiver.GetUnsubAck();
                if (unSubAck == null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        SubErr.Text = "The unsubscribe message was NOT successful! Broker didn't respond with an acknowledgement";
                        SubErr.Visibility = Visibility.Visible;
                    });
                    return;
                }
                
                Dispatcher.Invoke(() =>
                {
                    SubSuc.Text = "The unsubscribe message was sent successfully";
                    SubSuc.Visibility = Visibility.Visible;
                    SubscribeViewModel.Unsubscribable.Remove(topic);
                    UnsubscribeBox.SelectedIndex = 0;
                    SubscriptionsViewModel.Remove(topic);
                });
            }).Start();
        }
    }
}