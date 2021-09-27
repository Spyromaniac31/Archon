using Archon.Helpers;
using Archon.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Archon.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        public ShellPage()
        {
            InitializeComponent();

            TitleBarHelper.SetCustomTitleBar(AppTitleBar);

            DataContext = ViewModel;
            ViewModel.Initialize(shellFrame, navigationView);
        }

        private void navigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            ViewModel.ItemInvokedCommand.Execute(args);
        }
    }
}
