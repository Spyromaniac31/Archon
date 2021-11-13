using Archon.Helpers;
using Archon.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Archon.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private string _themeName = ThemeSelectorService.Theme.ToString();
        private string _hostname = (string)ApplicationData.Current.LocalSettings.Values["Hostname"];
        private string _backupHostname = (string)ApplicationData.Current.LocalSettings.Values["BackupHostname"];
        private string _username;
        private string _password;
        private string _directory = (string)ApplicationData.Current.LocalSettings.Values["Directory"];
        private string _scriptName = (string)ApplicationData.Current.LocalSettings.Values["ScriptName"];

        public string ThemeName
        {
            get => _themeName;
            set => SetProperty(ref _themeName, value);
        }
        public string Hostname
        {
            get => _hostname;
            set => SetProperty(ref _hostname, value);
        }
        public string BackupHostname
        {
            get => _backupHostname;
            set => SetProperty(ref _backupHostname, value);
        }
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        public string Directory
        {
            get => _directory;
            set => SetProperty(ref _directory, value);
        }
        public string ScriptName
        {
            get => _scriptName;
            set => SetProperty(ref _scriptName, value);
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get => _versionDescription;
            set => SetProperty(ref _versionDescription, value);
        }

        public SettingsViewModel()
        {
        }

        public async Task InitializeAsync()
        {
            var credentials = CredentialHelper.RetrieveServerCredentials();
            Username = credentials.UserName;
            Password = credentials.Password;
            VersionDescription = GetVersionDescription();
            await Task.CompletedTask;
        }

        public async void UpdateThemeAsync()
        {
            if (Enum.TryParse(typeof(ElementTheme), ThemeName, out var parsedTheme))
            {
                await ThemeSelectorService.SetThemeAsync((ElementTheme)parsedTheme);
            }
            else
            {
                ErrorReporterService.ReportError("Error parsing theme", "There was an issue applying the selected theme. Please file feedback if this error continues.", "Error");
            }
        }

        public void UpdateAppSettings(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if ((string)textBox.Header == "Username")
                {
                    CredentialHelper.UpdateServerCredentials(textBox.Text, Password);
                    return;
                }
                ApplicationData.Current.LocalSettings.SaveString((string)textBox.Tag, textBox.Text);
            }
            else if (sender is PasswordBox passwordBox)
            {
                if (string.IsNullOrEmpty(passwordBox.Password))
                {
                    return;
                }
                CredentialHelper.UpdateServerCredentials(Username, passwordBox.Password);
            }
        }

        private string GetVersionDescription()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"Archon - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
