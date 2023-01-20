using System.Windows;
using System.Windows.Controls;
using MQTT_GUI.Core;
using MQTT_GUI.MVVM.Views;

namespace MQTT_GUI.MVVM.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        public static RelayCommand ConnectViewCommand { get; set; }
        public static RelayCommand PublishViewCommand { get; set; }

        public static ConnectViewModel ConnectViewModel { get; set; }
        public static PublishViewModel PublishViewModel { get; set; }
        
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        
        public MainWindowViewModel()
        {
            ConnectViewModel = new ConnectViewModel();
            PublishViewModel = new PublishViewModel();
            CurrentView = ConnectViewModel;

            ConnectViewCommand = new RelayCommand(o =>
            {
                CurrentView = ConnectViewModel;
            });
            
            PublishViewCommand = new RelayCommand(o =>
            {
                CurrentView = PublishViewModel;
            });
        }
    }
}