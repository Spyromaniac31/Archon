using Archon.Helpers;
using Archon.Services;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Archon.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();

            TitleBarHelper.SetCustomTitleBar(AppTitleBar);
        }
        //This doesn't feel worthy of a ViewModel
        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _ = NavigationService.Navigate(typeof(SetupShellPage));
            _ = NavigationService.Navigate(typeof(HostnamePage));
        }
    }
}
