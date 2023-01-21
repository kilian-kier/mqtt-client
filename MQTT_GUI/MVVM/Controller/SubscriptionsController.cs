using System.Text;
using System.Threading;
using System.Windows.Threading;
using MQTT_GUI.MQTT;
using MQTT_GUI.MVVM.ViewModel;
using MQTT_GUI.MVVM.Views;

namespace MQTT_GUI.MVVM.Controller
{
    public class SubscriptionsController
    {
        public static void SubscriptionThread(Dispatcher dispatcher)
        {
            new Thread(() =>
            {
                while (true)
                {
                    while (MQTTClient.Client.Receiver.SubQueue.Count == 0)
                    {
                        Thread.Sleep(10);
                    }

                    var subscribed = MQTTClient.Client.Receiver.SubQueue.Dequeue();
                    var topic = Encoding.ASCII.GetString(subscribed.Header, 1, subscribed.Header.Length - 1);
                    var message = Encoding.ASCII.GetString(subscribed.Payload, 0, subscribed.Payload.Length);
                    dispatcher.Invoke(() =>
                    {
                        while (SubscriptionsView.Context == null)
                        {
                            Thread.Sleep(100);
                        }
                        var context = (SubscriptionsViewModel) SubscriptionsView.Context;
                        SubscriptionsView.Context = null;
                        context.AddMessage(topic, message, ref context);
                        SubscriptionsView.Context = context;
                    });
                }
            }).Start();
        }
    }
}