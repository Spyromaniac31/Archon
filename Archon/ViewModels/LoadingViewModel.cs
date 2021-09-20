using Archon.Helpers;
using Archon.Services;
using Archon.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace Archon.ViewModels
{
    public class LoadedEventArgs : EventArgs
    {
    }

    public class LoadingViewModel : ObservableObject
    {
        private string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        private StorageFile _script;
        private StorageFile _gameIni;
        private StorageFile _gameUserIni;

        private ICommand _loadedCommand;

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(async () => await OnLoadedAsync()));

        public async Task InitializeAsync()
        {
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;
            StorageFolder localCache = ApplicationData.Current.LocalCacheFolder;

            _script = await localCache.CreateFileAsync((string)appSettings.Values["ScriptName"], CreationCollisionOption.OpenIfExists);
            _gameIni = await localCache.CreateFileAsync("Game.ini", CreationCollisionOption.OpenIfExists);
            _gameUserIni = await localCache.CreateFileAsync("GameUserSettings.ini", CreationCollisionOption.OpenIfExists);
        }

        private async Task OnLoadedAsync()
        {
            await InitializeAsync();
            await CacheFilesAsync();
            await ParseFilesAsync();
            //When the ShellPage is loaded, it changes the NavigationService frame, so then we can navigate to the dashboard within the shell
            _ = NavigationService.Navigate(typeof(ShellPage));
            _ = NavigationService.Navigate(typeof(DashboardPage));
        }

        private async Task CacheFilesAsync()
        {
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;

            Status = "Caching start script";
            string scriptPath = (string)appSettings.Values["Directory"] + "/ShooterGame/Binaries/Linux/" + (string)appSettings.Values["ScriptName"];
            _ = await SshService.DownloadFileAsync(_script, scriptPath);

            Status = "Caching Game.ini";
            string gameIniPath = (string)appSettings.Values["Directory"] + "/ShooterGame/Saved/Config/LinuxServer/Game.ini";
            _ = await SshService.DownloadFileAsync(_gameIni, gameIniPath);

            Status = "Caching GameUserSettings.ini";
            string gameUserIniPath = (string)appSettings.Values["Directory"] + "/ShooterGame/Saved/Config/LinuxServer/GameUserSettings.ini";
            _ = await SshService.DownloadFileAsync(_gameUserIni, gameUserIniPath);
        }

        private async Task ParseFilesAsync()
        {
            var gameSettings = new ApplicationDataCompositeValue();

            Status = "Reading script";
            var scriptSettings = await FileParsers.ParseScriptAsync(_script);
            foreach (var setting in scriptSettings)
            {
                AddOrConcat(gameSettings, setting);
            }

            Status = "Reading Game.ini";
            var gameIniSettings = await FileParsers.ParseIniAsync(_gameIni);
            foreach (var setting in gameIniSettings)
            {
                AddOrConcat(gameSettings, setting);
            }

            Status = "Reading GameUserSettings.ini";
            var gameUserIniSettings = await FileParsers.ParseIniAsync(_gameUserIni);
            foreach (var setting in gameUserIniSettings)
            {
                AddOrConcat(gameSettings, setting);
            }

            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;
            appSettings.Values["GameSettings"] = gameSettings;
        }

        private void AddOrConcat(ApplicationDataCompositeValue settings, KeyValuePair<string, string> keyValuePair)
        {
            if (settings.ContainsKey(keyValuePair.Key))
            {
                if (((string)settings[keyValuePair.Key]).ToLower() != keyValuePair.Value.ToLower())
                {
                    settings[keyValuePair.Key] = settings[keyValuePair.Key] + "," + keyValuePair.Value;
                }
            }
            else
            {
                settings[keyValuePair.Key] = keyValuePair.Value;
            }
        }

    }
}
