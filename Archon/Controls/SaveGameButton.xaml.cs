using Archon.Services;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Archon.Controls
{
    public sealed partial class SaveGameButton : UserControl
    {
        public event EventHandler Click;

        public SaveGameButton()
        {
            InitializeComponent();
        }

        private void Clicked()
        {
            Click?.Invoke(this, new EventArgs());
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //Tooltip.Visibility = Visibility.Visible;
            //SaveButton.Resources["ButtonForegroundPointerOver"] = Colors.DeepSkyBlue;
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //Tooltip.Visibility = Visibility.Collapsed;
            
        }
    }
}
