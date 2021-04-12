using Archon.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            ((FontIcon)ToggleButton.Content).Glyph = _isServerRunning ? "\xEB8A" : "\xEB21";
            //Tooltip.Text = _isServerRunning ? "Stop ARK" : "Start ARK";
        }

        private void ToggleButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //TODO: Implement tooltip
        }

        private void ToggleButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //TODO: Implement tooltip
        }
    }
}
