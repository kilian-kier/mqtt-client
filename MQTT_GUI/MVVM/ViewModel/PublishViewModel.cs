using System.Collections.ObjectModel;
using MQTT_GUI.Core;

namespace MQTT_GUI.MVVM.ViewModel
{
    public class PublishViewModel : ObservableObject
    {
        public static ObservableCollection<string> QoS { get; } = new ObservableCollection<string>
        {
            "at most once",
            "exactly once",
            "at least once"
        };

        public static string SelectedQoS { get; set; } = "at most once";
    }
}