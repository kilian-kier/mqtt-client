using System.Windows.Controls;
using MQTT_GUI.MVVM.ViewModel;

namespace MQTT_GUI.MVVM.Views
{
    public partial class SubscriptionsView : UserControl
    {
        public static object Context { get; set; }
        public SubscriptionsView()
        {
            InitializeComponent();
            Context = DataContext;
            /*var root1 = new SubscriptionsViewModel("esp", "");
            var esp1 = new SubscriptionsViewModel("temperature", "19.20");
            var esp2 = new SubscriptionsViewModel("heatIndex", "15.20");
            var esp3 = new SubscriptionsViewModel("humidity", "75");
            root1.Items.Add(esp1);
            root1.Items.Add(esp2);
            root1.Items.Add(esp3);
            var root2 = new SubscriptionsViewModel("shelli", "");
            var root3 = new SubscriptionsViewModel("$SYS", "");
            TreeView.Items.Add(root1);
            TreeView.Items.Add(root2);
            TreeView.Items.Add(root3);*/
        }
    }
}