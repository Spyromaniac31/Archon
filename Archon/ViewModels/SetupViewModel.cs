using Archon.Helpers;
using Archon.Services;
using Archon.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace Archon.ViewModels
{
    public class SetupViewModel : ObservableObject
    {
        private bool _isBackEnabled;
        private WinUI.NavigationView _navigationView;
        private WinUI.NavigationViewItem _selected;
        private ICommand _loadedCommand;
        private ICommand _itemInvokedCommand;
        private ICommand _finishSetupCommand;
        private ICommand _nextPageCommand;
        private bool _errorOpen = false;

        public string Hostname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Directory { get; set; }
        public string ScriptName { get; set; }
        public string BackupHostname { get; set; }

        public bool ErrorOpen
        {
            get => _errorOpen;
            set => SetProperty(ref _errorOpen, value);
        }

        public bool IsBackEnabled
        {
            get => _isBackEnabled;
            set => SetProperty(ref _isBackEnabled, value);
        }

        public WinUI.NavigationViewItem Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }


        public ICommand FinishSetupCommand => _finishSetupCommand ?? (_finishSetupCommand = new RelayCommand(FinishSetup));

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

        public ICommand ItemInvokedCommand => _itemInvokedCommand ?? (_itemInvokedCommand = new RelayCommand<WinUI.NavigationViewItemInvokedEventArgs>(OnItemInvoked));

        public ICommand NextPageCommand => _nextPageCommand ?? (_nextPageCommand = new RelayCommand(NextPage));

        public void Initialize()
        {
        }

        public void Initialize(Frame frame, WinUI.NavigationView navigationView)
        {
            _navigationView = navigationView;
            NavigationService.Frame = frame;
            NavigationService.NavigationFailed += Frame_NavigationFailed;
            NavigationService.Navigated += Frame_Navigated;
            _navigationView.BackRequested += OnBackRequested;
        }

        private async void OnLoaded()
        {
            await Task.CompletedTask;
        }

        private void OnItemInvoked(WinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is WinUI.NavigationViewItem selectedItem)
            {
                var pageType = selectedItem.GetValue(NavHelper.NavigateToProperty) as Type;
                _ = NavigationService.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void OnBackRequested(WinUI.NavigationView sender, WinUI.NavigationViewBackRequestedEventArgs args)
        {
            _ = NavigationService.GoBack();
        }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw e.Exception;
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = _navigationView.SettingsItem as WinUI.NavigationViewItem;
                return;
            }

            var selectedItem = GetSelectedItem(_navigationView.MenuItems, e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
            }
        }

        private WinUI.NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
        {
            foreach (var item in menuItems.OfType<WinUI.NavigationViewItem>())
            {
                if (IsMenuItemForPageType(item, pageType))
                {
                    return item;
                }

                var selectedChild = GetSelectedItem(item.MenuItems, pageType);
                if (selectedChild != null)
                {
                    return selectedChild;
                }
            }

            return null;
        }

        private bool IsMenuItemForPageType(WinUI.NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        public void NextPage()
        {
            _ = (Selected.Content as string) == "Hostname"
                ? NavigationService.Navigate(typeof(CredentialsPage))
                : NavigationService.Navigate(typeof(DirectoryPage));
        }

        public void FinishSetup()
        {
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;
            bool continueSetup = true;
            foreach (string value in new List<string>() { Hostname, Username, Password, Directory })
            {
                if (string.IsNullOrEmpty(value))
                {
                    //This causes the popup
                    ErrorOpen = true;

                    continueSetup = false;
                }
            }
            if (continueSetup)
            {
                appSettings.SaveString("Hostname", Hostname);
                appSettings.SaveString("Directory", Directory);
                appSettings.SaveString("ScriptName", string.IsNullOrEmpty(ScriptName) ? "server_start.sh" : ScriptName);
                appSettings.SaveString("BackupHostname", string.IsNullOrEmpty(BackupHostname) ? Hostname : BackupHostname);

                CredentialHelper.UpdateServerCredentials(Username, Password);

                Window.Current.Content = new Frame();
                NavigationService.Frame = Window.Current.Content as Frame;
                _ = NavigationService.Navigate(typeof(LoadingPage));
            }
            
        }
    }
}
