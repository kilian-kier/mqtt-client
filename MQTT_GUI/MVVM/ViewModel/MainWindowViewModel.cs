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
        public static RelayCommand TopicsViewCommand { get; set; }
        public static RelayCommand SubscribeViewCommand { get; set; }
        public static RelayCommand SubscriptionsViewCommand { get; set; }

        public static ConnectViewModel ConnectViewModel { get; set; }
        public static PublishViewModel PublishViewModel { get; set; }
        public static TopicsViewModel TopicsViewModel { get; set; }
        public static SubscribeViewModel SubscribeViewModel { get; set; }
        public static SubscriptionsViewModel SubscriptionsViewModel { get; set; }

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
            TopicsViewModel = new TopicsViewModel();
            SubscribeViewModel = new SubscribeViewModel();
            SubscriptionsViewModel = new SubscriptionsViewModel();

            CurrentView = ConnectViewModel;

            ConnectViewCommand = new RelayCommand(o => { CurrentView = ConnectViewModel; });
            PublishViewCommand = new RelayCommand(o => { CurrentView = PublishViewModel; });
            TopicsViewCommand = new RelayCommand(o => { CurrentView = TopicsViewModel; });
            SubscribeViewCommand = new RelayCommand(o => { CurrentView = SubscribeViewModel; });
            SubscriptionsViewCommand = new RelayCommand(o => { CurrentView = SubscriptionsViewModel; });
        }
    }
}