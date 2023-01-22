using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using MQTT_GUI.Core;
using MQTT_GUI.MVVM.Views;

namespace MQTT_GUI.MVVM.ViewModel
{
    public class SubscriptionsViewModel : ObservableObject
    {
        public static BindingList<SubscriptionsViewModel> Roots { get; } = new BindingList<SubscriptionsViewModel>();
        public string Topic { get; }
        private SubscriptionsViewModel _parent { get; set; }
        private string _message = "";

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }        
        public ObservableCollection<SubscriptionsViewModel> Items { get; set; }


        public SubscriptionsViewModel(string topic, string message)
        {
            Topic = topic;
            Message = message;
            _parent = null;
            Items = new ObservableCollection<SubscriptionsViewModel>();
        }

        public SubscriptionsViewModel()
        {
            
        }

        public void AddMessage(string topic, string message)
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
                Insert(root, t, message);
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

                var child = new SubscriptionsViewModel(topic, message)
                {
                    _parent = parent
                };
                parent.Items.Add(child);
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

            var node = new SubscriptionsViewModel(sub, "")
            {
                _parent = parent
            };
            parent.Items.Add(node);
            Insert(node, t, message);
        }

        public static void Remove(string topic)
        {
            if (topic == "#")
            {
                foreach (var item in Roots.ToList())
                {
                    if (item.Topic.StartsWith("$"))
                        return;
                    Roots.Remove(item);
                }

                return;
            }
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
                if (root.Items.Count != 0)
                    return;
                Roots.Remove(root);
            }
        }

        private static void RecRemove(SubscriptionsViewModel parent, string topic)
        {
            if (topic == "#")
            {
                parent.Items.Clear();
                parent._parent?.Items.Remove(parent);
                return;
            }
            
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

                if (parent.Items.Count != 0) return;

                parent._parent?.Items.Remove(parent);
                return;
            }

            var sub = topic.Substring(0, next);
            var t = topic.Substring(next + 1);
            foreach (var item in parent.Items)
            {
                if (item.Topic != sub)
                    continue;
                RecRemove(item, t);
                if (parent.Items.Count != 0) return;

                parent._parent?.Items.Remove(parent);
                break;
            }
        } 
    }
}