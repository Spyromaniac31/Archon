using Archon.ViewModels;
using System;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Archon.Controls
{
    public sealed partial class ToggleServerButton : UserControl
    {
        private bool _isServerRunning = false;

        public event EventHandler Click;

        public ToggleServerButton()
        {
            InitializeComponent();
            DashboardViewModel.RaiseStatusChangedEvent += UpdateServerStatus;
        }

        private void Clicked()
        {
            Click?.Invoke(this, new EventArgs());
        }

        private void UpdateServerStatus(object sender, StatusChangedEventArgs e)
        {
            bool receivedStatus = e.ServerRunning;
            if (receivedStatus != _isServerRunning)
            {
                _isServerRunning = receivedStatus;
                UpdateButton();
            }
        }

        private void UpdateButton()
        {
            ((FontIcon)ToggleButton.Content).Glyph = _isServerRunning ? "\xE9F9" : "\xEB21";
            ((FontIcon)ToggleButton.Content).Foreground = new SolidColorBrush(_isServerRunning ? Colors.Red : Colors.Green);
            ToolTip toolTip = new ToolTip
            {
                Content = _isServerRunning ? "Stop ARK" : "Start ARK"
            };
            ToolTipService.SetToolTip(ToggleButton, toolTip);
        }
    }
}
