using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MQTT_GUI.MVVM.Controller;
using MQTT_GUI.MVVM.ViewModel;

namespace MQTT_GUI.MVVM.Views
{
    public partial class Connect
    {
        public Connect()
        {
            InitializeComponent();
        }

        private void ButtonConnect(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(BrokerIp.Text) || string.IsNullOrEmpty(BrokerPort.Text))
            {
                Dispatcher.Invoke(() =>
                {
                    Errors.Text = "Please enter an IP address and port of your MQTT broker";
                    Errors.Visibility = Visibility.Visible;
                });
                return;
            }

            var dispatcher = Dispatcher;
            var windows = Application.Current.Windows;
            var brokerIp = BrokerIp.Text;
            var brokerPort = Convert.ToInt32(BrokerPort.Text);
            new Thread(() =>
            {
                dispatcher.Invoke(() =>
                {
                    Errors.Visibility = Visibility.Collapsed;
                    ProgressBar.Visibility = Visibility.Visible;
                });
                foreach (var window in windows)
                {
                    if (!(window is MainWindow mainWindow)) return;
                    dispatcher.Invoke(() =>
                    {
                        mainWindow.PublishMenuButton.Visibility = Visibility.Hidden;
                        mainWindow.TopicsMenuButton.Visibility = Visibility.Hidden;
                        mainWindow.SubscribeMenuButton.Visibility = Visibility.Hidden;
                        mainWindow.SubscriptionsMenuButton.Visibility = Visibility.Hidden;
                    });
                }

                MainWindowViewModel.ConnectViewModel.ConnectedBroker = "Not connected";
                MainWindowViewModel.ConnectViewModel.ButtonEnabled = false;
                ConnectController.Disconnect();

                var successfulConnection = ConnectController.Connect(brokerIp, brokerPort, Errors, dispatcher);
                dispatcher.Invoke(() => { ProgressBar.Visibility = Visibility.Collapsed; });

                if (!successfulConnection)
                {
                    if (Errors.Visibility == Visibility.Visible) return;
                    dispatcher.Invoke(() =>
                    {
                        Errors.Visibility = Visibility.Visible;
                        Errors.Text = "Error while sending the connect message";
                    });
                    return;
                }

                foreach (var window in windows)
                {
                    if (!(window is MainWindow mainWindow)) return;
                    dispatcher.Invoke(() =>
                    {
                        mainWindow.PublishMenuButton.Visibility = Visibility.Visible;
                        mainWindow.PublishMenuButton.IsChecked = true;
                        mainWindow.TopicsMenuButton.Visibility = Visibility.Visible;
                        mainWindow.SubscribeMenuButton.Visibility = Visibility.Visible;
                        mainWindow.SubscriptionsMenuButton.Visibility = Visibility.Visible;
                    });
                }

                MainWindowViewModel.ConnectViewModel.ConnectedBroker = brokerIp + ":" + brokerPort;
                MainWindowViewModel.ConnectViewModel.ButtonEnabled = true;

                TopicsController.AddTopics(Dispatcher);
                SubscriptionsController.SubscriptionThread(Dispatcher);

                MainWindowViewModel.PublishViewCommand.Execute(null);
            }).Start();
        }

        private void ButtonConnectEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonConnect(sender, e);
            }
        }

        private void ButtonDisconnect(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.ConnectViewModel.ConnectedBroker = "Not connected";
            MainWindowViewModel.ConnectViewModel.ButtonEnabled = false;
            ConnectController.Disconnect();
            var dataContext = DataContext;
            DataContext = null;
            DataContext = dataContext;
            foreach (var window in Application.Current.Windows)
            {
                if (!(window is MainWindow mainWindow)) return;
                mainWindow.PublishMenuButton.Visibility = Visibility.Hidden;
                mainWindow.TopicsMenuButton.Visibility = Visibility.Hidden;
                mainWindow.SubscribeMenuButton.Visibility = Visibility.Hidden;
                mainWindow.SubscriptionsMenuButton.Visibility = Visibility.Hidden;
            }
        }

        private void IpKeyboard(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                Dispatcher.BeginInvoke((ThreadStart) delegate
                {
                    BrokerPort.Focus();
                    FocusManager.SetFocusedElement(this, BrokerPort);
                    Keyboard.Focus(BrokerPort);
                });
            }
        }

        private void IpSetFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;
            tb.Focusable = true;
            Keyboard.Focus(tb);
        }
    }
}