using Archon.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Archon.Controls
{
    public sealed partial class ServerStatusTextBlock : UserControl
    {
        private bool _isServerRunning = false;

        public ServerStatusTextBlock()
        {
            InitializeComponent();
            DashboardViewModel.RaiseStatusChangedEvent += UpdateServerStatus;
        }

        private void UpdateServerStatus(object sender, StatusChangedEventArgs e)
        {
            bool receivedStatus = e.ServerRunning;
            if (receivedStatus != _isServerRunning)
            {
                _isServerRunning = receivedStatus;
                ServerStatusText.Text = _isServerRunning ? "Running" : "Stopped";
                ServerStatusText.Foreground = new SolidColorBrush(_isServerRunning ? Colors.Green : Colors.Red);
            }
        }
    }
}
