using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Archon.Controls
{
    public sealed partial class HintButton : UserControl
    {
        public string HintTitle { get; set; }
        public string HintText { get; set; }
        public HintButton()
        {
            InitializeComponent();
        }

        private void HintButton_Click(object sender, RoutedEventArgs e)
        {
            if (HintTip.IsOpen)
            {
                HintTip.IsOpen = false;
            }
            else
            {
                HintTip.Title = HintTitle;
                HintTextBox.Text = HintText;
                HintTip.IsOpen = true;
            }
        }
    }
}
