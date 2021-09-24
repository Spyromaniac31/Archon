using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Archon.Controls
{
    public sealed partial class ArkInstallationBlock : UserControl
    {
        public ArkInstallationBlock()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UpdateStatusBar();
        }

        private async void UpdateStatusBar()
        {
            //This feels gross but we'll see
            while (LatestVersionText.Text == "" || InstalledVersionText.Text == "")
            {
                await Task.Delay(500);
            }

            double latestVersion = Convert.ToDouble(LatestVersionText.Text);
            double installedVersion = Convert.ToDouble(InstalledVersionText.Text);

            if (latestVersion == installedVersion)
            {
                StatusIcon.Glyph = "\uE9A1";
                StatusBackground.Fill = new SolidColorBrush(Colors.Green);
                StatusText.Text = "ARK is up to date";
            }

            else
            {
                StatusIcon.Glyph = "\uE91C";
                StatusBackground.Fill = new SolidColorBrush(Colors.DarkGoldenrod);
                StatusText.Text = "Update available";
            }
        }

        private void RefreshButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UpdateStatusBar();
        }
    }
}
