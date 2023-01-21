using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using MQTT_GUI.Core;

namespace MQTT_GUI.MVVM.ViewModel
{
    public class TopicsViewModel : ObservableObject
    {
        private static Visibility _progressBar = Visibility.Hidden;
        public static ObservableCollection<Topic> Topics { get; } = new ObservableCollection<Topic>();

        public Visibility ProgressBar
        {
            get => _progressBar;
            set
            {
                _progressBar = value;
                OnPropertyChanged("ProgressBar");
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

    public class Topic
    {
        public string Name { get; }

        public Topic(string name)
        {
            Name = name;
        }
    }
}