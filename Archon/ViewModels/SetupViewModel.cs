using Archon.Helpers;
using Archon.Services;
using Archon.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.Storage;

namespace Archon.ViewModels
{
    public class SetupViewModel : ObservableObject
    {
        private ICommand _finishSetupCommand;
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


        public ICommand FinishSetupCommand => _finishSetupCommand ?? (_finishSetupCommand = new RelayCommand(FinishSetup));

        public void Initialize()
        {
        }

        public void FinishSetup()
        {
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;
            foreach (string value in new List<string>() { Hostname, Username, Password, Directory, ScriptName })
            {
                if (string.IsNullOrEmpty(value))
                {
                    ErrorOpen = true;
                }
            }
            if (!ErrorOpen)
            {
                appSettings.SaveString("Hostname", Hostname);
                appSettings.SaveString("Username", Username);
                appSettings.SaveString("Password", Password);
                appSettings.SaveString("Directory", Directory);
                appSettings.SaveString("ScriptName", ScriptName);
                appSettings.SaveString("BackupHostname", string.IsNullOrEmpty(BackupHostname) ? Hostname : BackupHostname);
                

                _ = NavigationService.Navigate(typeof(LoadingPage));
            }
            
        }
    }
}
