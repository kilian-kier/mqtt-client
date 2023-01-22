using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using MQTT_GUI.MQTT;
using MQTT_GUI.MQTT.messages;
using MQTT_GUI.MVVM.ViewModel;
using MQTT_GUI.MVVM.Views;
using Connect = MQTT_GUI.MQTT.messages.Connect;

namespace MQTT_GUI.MVVM.Controller
{
    public static class TopicsController
    {
        private static bool _alreadyRunning;

        public static void AddTopics(Dispatcher dispatcher)
        {
            if (_alreadyRunning)
                return;

            _alreadyRunning = true;

            new Thread(() =>
            {
                if (TopicsView.Context == null)
                {
                    dispatcher.Invoke(() => { MainWindowViewModel.TopicsViewModel.ProgressBar = Visibility.Visible; });
                }
                else
                {
                    var context1 = (TopicsViewModel) TopicsView.Context;
                    TopicsView.Context = null;
                    dispatcher.Invoke(() => { context1.ProgressBar = Visibility.Visible; });
                    TopicsView.Context = context1;
                }

                var topicsClient = new MQTTClient();
                topicsClient.CreateTcpConnection();
                var clientId = Guid.NewGuid();
                var connectPacket = new Connect(clientId.ToString());
                topicsClient.SendObject(connectPacket);
                var connAck = topicsClient.Receiver.GetConnAck();
                if (connAck?.Header == null || connAck.Header.Length > 2 || connAck.Header[1] != 0)
                {
                    return;
                }

                var subscribeToSystemTopics = new Subscribe("$SYS/#", Subscribe.QoS.AtLeastOnce);
                var subscribeToOtherTopics = new Subscribe("#", Subscribe.QoS.AtLeastOnce);
                topicsClient.SendObject(subscribeToSystemTopics);
                topicsClient.SendObject(subscribeToOtherTopics);

                var ms = 0;

                // searching for 30 seconds
                while (ms < 30000)
                {
                    while (topicsClient.Receiver.SubQueue.Count == 0)
                    {
                        Thread.Sleep(10);
                        ms += 10;
                    }

                    var subscribed = topicsClient.Receiver.SubQueue.Dequeue();
                    var topic = Encoding.ASCII.GetString(subscribed.Header, 1, subscribed.Header.Length - 1);
                    dispatcher.Invoke(() => { MainWindowViewModel.TopicsViewModel.AddTopic(topic); });
                }

                while (topicsClient.Receiver.SubQueue.Count > 0)
                {
                    var subscribed = topicsClient.Receiver.SubQueue.Dequeue();
                    var topic = Encoding.ASCII.GetString(subscribed.Header, 1, subscribed.Header.Length - 1);
                    dispatcher.Invoke(() => { MainWindowViewModel.TopicsViewModel.AddTopic(topic); });
                }

                var unsubscribeToSystemTopics = new Unsubscribe("$SYS/#");
                var unsubscribeToOtherTopics = new Unsubscribe("#");
                topicsClient.SendObject(unsubscribeToSystemTopics);
                topicsClient.SendObject(unsubscribeToOtherTopics);

                _ = topicsClient.Receiver.GetUnsubAck();
                _ = topicsClient.Receiver.GetUnsubAck();

                var disconnect = new Disconnect();
                topicsClient.SendObject(disconnect);
                if (TopicsView.Context == null)
                {
                    dispatcher.Invoke(() => { MainWindowViewModel.TopicsViewModel.ProgressBar = Visibility.Hidden; });
                }
                else
                {
                    var context1 = (TopicsViewModel) TopicsView.Context;
                    TopicsView.Context = null;
                    dispatcher.Invoke(() => { context1.ProgressBar = Visibility.Hidden; });
                    TopicsView.Context = context1;
                }

                _alreadyRunning = false;
            }).Start();
        }
    }
}