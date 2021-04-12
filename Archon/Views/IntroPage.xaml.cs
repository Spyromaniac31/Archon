using Archon.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Archon.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IntroPage : Page
    {
        public IntroViewModel ViewModel { get; } = new IntroViewModel();

        public IntroPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
