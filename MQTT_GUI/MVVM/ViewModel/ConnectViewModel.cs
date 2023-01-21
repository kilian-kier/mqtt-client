using System.Windows;
using System.Windows.Controls;
using MQTT_GUI.Core;

namespace MQTT_GUI.MVVM.ViewModel
{
    public class ConnectViewModel : ObservableObject
    {
        private static string _connectedBroker = "Not connected";
        private static bool _buttonEnabled = false;
        private Visibility _showProgressBar = Visibility.Collapsed;

        public string ConnectedBroker
        {
            get => _connectedBroker;
            set
            {
                _connectedBroker = value;
                OnPropertyChanged();
            }
        }

        public bool ButtonEnabled
        {
            get => _buttonEnabled;
            set
            {
                _buttonEnabled = value;
                OnPropertyChanged();
            }
        }

        public Visibility ShowProgressBar
        {
            get => _showProgressBar;
            set
            {
                _showProgressBar = value;
                OnPropertyChanged();
            }
        }
    }
}