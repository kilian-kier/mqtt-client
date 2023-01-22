using System.ComponentModel;
using System.Linq;
using System.Windows;
using MQTT_GUI.Core;

namespace MQTT_GUI.MVVM.ViewModel
{
    public class TopicsViewModel : ObservableObject
    {
        private static Visibility _progressBar = Visibility.Hidden;
        private static readonly BindingList<Topic> _topics = new BindingList<Topic>();

        public BindingList<Topic> Topics => _topics;

        public Visibility ProgressBar
        {
            get => _progressBar;
            set
            {
                _progressBar = value;
                OnPropertyChanged();
            }
        }

        public void AddTopic(string topic)
        {
            if (Topics.Any(t => t.Name == topic))
            {
                return;
            }

            Topics.Add(new Topic(topic));
        }

        public void Clear()
        {
            Topics.Clear();
        }
    }

    public class Topic : ObservableObject
    {
        public string Name { get; }

        public Topic(string name)
        {
            Name = name;
            OnPropertyChanged();
        }
    }
}