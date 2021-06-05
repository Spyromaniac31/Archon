using Archon.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Archon.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfigurationPage : Page
    {
        private ConfigurationViewModel ViewModel { get; } = new ConfigurationViewModel();

        public ConfigurationPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            
        }

        private async void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
            SettingsCVS.Source = ViewModel.SettingsGroups;
        }
    }
}
