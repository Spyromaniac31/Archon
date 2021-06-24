using Archon.ViewModels;
using Windows.UI.Xaml.Controls;

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

            //I want to put this in an async method but I don't know one that won't be called multiple times in the page's lifespan
            //ViewModel.InitializeAsync();
        }

        private async void Page_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ViewModel.InitializeAsync();
            //SettingsCVS.Source = ViewModel.SelectedSettingGroup;
        }
    }
}
