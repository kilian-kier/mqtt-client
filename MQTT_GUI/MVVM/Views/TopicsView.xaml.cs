using System.Windows;
using MQTT_GUI.MVVM.Controller;
using MQTT_GUI.MVVM.ViewModel;

namespace MQTT_GUI.MVVM.Views
{
    public partial class TopicsView
    {
        public static object Context { get; set; }

        public TopicsView()
        {
            InitializeComponent();
            Context = DataContext;
        }

        private void ButtonRefresh(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.TopicsViewModel.Clear();
            TopicsController.AddTopics(Dispatcher);
        }
    }
}