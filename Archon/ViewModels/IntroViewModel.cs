using Archon.Services;
using Archon.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows.Input;

namespace Archon.ViewModels
{
    public class IntroViewModel : ObservableObject
    {
        private ICommand _getStartedCommand;

        public ICommand GetStartedCommand => _getStartedCommand ?? (_getStartedCommand = new RelayCommand(GetStarted));

        public IntroViewModel()
        {
        }

        private void GetStarted()
        {
            NavigationService.Navigate(typeof(SetupPage));
        }

    }
}
