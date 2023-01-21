using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using MQTT_GUI.Core;
using MQTT_GUI.MVVM.Views;

namespace MQTT_GUI.MVVM.ViewModel
{
    public class SubscriptionsViewModel : ObservableObject
    {
        public static ObservableCollection<SubscriptionsViewModel> Roots { get; } = new ObservableCollection<SubscriptionsViewModel>();
        public string Topic { get; }
        public string Message { get; set; }        
        public ObservableCollection<SubscriptionsViewModel> Items { get; set; }


        public SubscriptionsViewModel(string topic, string message)
        {
            Topic = topic;
            Message = message;
            Items = new ObservableCollection<SubscriptionsViewModel>();
        }

        public SubscriptionsViewModel()
        {
            
        }

        public void AddMessage(string topic, string message, ref SubscriptionsViewModel context)
        {
            var sub = topic.IndexOf('/');
            if (sub == -1)
            {
                sub = topic.Length;
            }

            var rootTopic = topic.Substring(0, sub);

            var root = Roots.FirstOrDefault(r => r.Topic == rootTopic);

            if (root == null)
            {
                root = new SubscriptionsViewModel(rootTopic, "");
                Roots.Add(root);
            }

            if (sub == topic.Length)
            {
                root.Message = message;
            }
            else
            {
                var t = topic.Substring(sub + 1);
                context.Insert(root, t, message);
            }
            
            OnPropertyChanged();
        }

        private void Insert(SubscriptionsViewModel parent, string topic, string message)
        {
            var next = topic.IndexOf('/');
            if (next == -1)
            {
                foreach (var item in parent.Items)
                {
                    if (item.Topic != topic)
                        continue;
                    item.Message = message;
                    return;
                }
                parent.Items.Add(new SubscriptionsViewModel(topic, message));
                return;
            }

            var sub = topic.Substring(0, next);
            var t = topic.Substring(next + 1);
            foreach (var item in parent.Items)
            {
                if (item.Topic != sub)
                    continue;
                Insert(item, t, message);
                return;
            }
            var node = new SubscriptionsViewModel(sub, "");
            parent.Items.Add(node);
            Insert(node, t, message);
        }

        public static void Remove(string topic)
        {
            var sub = topic.IndexOf('/');
            if (sub == -1)
            {
                sub = topic.Length;
            }

            var rootTopic = topic.Substring(0, sub);

            var root = Roots.FirstOrDefault(r => r.Topic == rootTopic);

            if (root == null)
            {
                return;
            }

            if (sub == topic.Length)
            {
                Roots.Remove(root);
            }
            else
            {
                var t = topic.Substring(sub + 1);
                RecRemove(root, t);
            }
        }

        private static void RecRemove(SubscriptionsViewModel parent, string topic)
        {
            // TODO: ref
            var next = topic.IndexOf('/');
            if (next == -1)
            {
                foreach (var item in parent.Items)
                {
                    if (item.Topic != topic)
                        continue;
                    parent.Items.Remove(item);
                    break;
                }

                if (parent.Items.Count == 0)
                {
                    parent = null;
                }
                return;
            }

            var sub = topic.Substring(0, next);
            var t = topic.Substring(next + 1);
            for (var i = 0; i < parent.Items.Count; i++)
            {
                if (parent.Items[i].Topic != sub)
                    continue;
                RecRemove(parent.Items[i], t);
                break;
            }
            if (parent.Items.Count == 0)
            {
                parent = null;
            }
        } 
    }
}