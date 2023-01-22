using MQTT_GUI.Core;

namespace MQTT_GUI.MVVM.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        public static RelayCommand ConnectViewCommand { get; private set; }
        public static RelayCommand PublishViewCommand { get; private set; }
        public static RelayCommand TopicsViewCommand { get; private set; }
        public static RelayCommand SubscribeViewCommand { get; private set; }
        public static RelayCommand SubscriptionsViewCommand { get; private set; }

        public static ConnectViewModel ConnectViewModel { get; private set; }
        private static PublishViewModel PublishViewModel { get; set; }
        public static TopicsViewModel TopicsViewModel { get; private set; }
        private static SubscribeViewModel SubscribeViewModel { get; set; }
        public static SubscriptionsViewModel SubscriptionsViewModel { get; private set; }

        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            private set
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