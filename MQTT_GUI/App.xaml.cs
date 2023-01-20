using System;
using System.Windows;
using MQTT_GUI.MVVM.Controller;

namespace MQTT_GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void App_OnExit(object sender, ExitEventArgs e)
        {
            ConnectController.Disconnect();
            Environment.Exit(0);
        }
    }
}