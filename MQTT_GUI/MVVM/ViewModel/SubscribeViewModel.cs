using System.Collections.ObjectModel;
using MQTT_GUI.Core;

namespace MQTT_GUI.MVVM.ViewModel
{
    public class SubscribeViewModel : ObservableObject
    {
        public static ObservableCollection<string> QoS { get; } = new ObservableCollection<string>
        {
            "at most once",
            "exactly once",
            "at least once"
        };

        public static string SelectedQoS { get; set; } = "at most once";

        public static ObservableCollection<string> Unsubscribable { get; } = new ObservableCollection<string>();

        public static string SelectedUnsubscribe { get; set; } = "";
    }
}