using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Archon.Controls
{
    public sealed partial class UpdateGameButton : UserControl
    {
        public event EventHandler Click;

        public UpdateGameButton()
        {
            InitializeComponent();
        }

        private void Clicked()
        {
            Click?.Invoke(this, new EventArgs());
        }

        private void UpdateButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //Tooltip.Visibility = Visibility.Visible;
        }

        private void UpdateButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //Tooltip.Visibility = Visibility.Collapsed;
        }
    }
}
