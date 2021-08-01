using Archon.ViewModels;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Archon.Controls
{
    public sealed partial class ServerStatusTextBlock : UserControl
    {
        private bool IsServerRunning = false;

        public ServerStatusTextBlock()
        {
            InitializeComponent();
            DashboardViewModel.RaiseStatusChangedEvent += UpdateServerStatus;
        }

        private void UpdateServerStatus(object sender, StatusChangedEventArgs e)
        {
            bool receivedStatus = e.ServerRunning;
            if (receivedStatus != IsServerRunning)
            {
                IsServerRunning = receivedStatus;
                ServerStatusText.Text = IsServerRunning ? "Running" : "Stopped";
                ServerStatusText.Foreground = new SolidColorBrush(IsServerRunning ? Colors.Green : Colors.Red);
            }
        }
    }
}
