using Archon.Helpers;
using Archon.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace Archon.ViewModels
{
    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(bool serverRunning)
        {
            ServerRunning = serverRunning;
        }

        public bool ServerRunning;
    }

    public class DashboardViewModel : ObservableObject
    {
        private ICommand _saveGameCommand;
        private ICommand _updateGameCommand;
        private ICommand _toggleServerCommand;
        private ICommand _retrieveVersionsCommmand;
        private ICommand _verifyInstallCommand;
        private bool _isServerRunning;
        private bool _saveErrorOpen = false;
        private bool _updateErrorOpen = false;
        private bool _isWaiting = false;
        private bool _connectionFailed;
        private bool _connectionSuccess;
        private string _serverName;
        private string _ipAddress;
        private string _installedVersion;
        private string _latestVersion;

        public static event EventHandler<StatusChangedEventArgs> RaiseStatusChangedEvent;

        public bool IsServerRunning
        {
            get => ConnectionSuccess && _isServerRunning;
            private set
            {
                if (value != _isServerRunning)
                {
                    SetProperty(ref _isServerRunning, value);
                    RaiseStatusChangedEvent?.Invoke(this, new StatusChangedEventArgs(value));
                }
            }
        }

        //I'd rather use !ConnectionFailed but this is easier for now
        public bool ConnectionSuccess
        {
            get => _connectionSuccess;
            set => SetProperty(ref _connectionSuccess, value);
        }
        public bool ConnectionFailed
        {
            get => _connectionFailed;
            set => SetProperty(ref _connectionFailed, value);
        }
        public bool IsWaiting
        {
            get => _isWaiting;
            set => SetProperty(ref _isWaiting, value);
        }
        public bool SaveErrorOpen
        {
            get => _saveErrorOpen;
            set => SetProperty(ref _saveErrorOpen, value);
        }
        public bool UpdateErrorOpen
        {
            get => _updateErrorOpen;
            set => SetProperty(ref _updateErrorOpen, value);
        }
        public string ServerName
        {
            get => _serverName;
            set => SetProperty(ref _serverName, value);
        }
        public string IPAddress
        {
            get => _ipAddress;
            set => SetProperty(ref _ipAddress, value);
        }
        public string InstalledVersion
        {
            get => _installedVersion;
            set => SetProperty(ref _installedVersion, value);
        }
        public string LatestVersion
        {
            get => _latestVersion;
            set => SetProperty(ref _latestVersion, value);
        }

        public ICommand SaveGameCommand => _saveGameCommand ?? (_saveGameCommand = new RelayCommand(async () => await SaveGameAsync()));
        public ICommand UpdateGameCommand => _updateGameCommand ?? (_updateGameCommand = new RelayCommand(async () => await UpdateGameAsync()));
        public ICommand ToggleServerCommand => _toggleServerCommand ?? (_toggleServerCommand = new RelayCommand(async () => await ToggleServerAsync()));
        public ICommand RetrieveVersionsCommand => _retrieveVersionsCommmand ?? (_retrieveVersionsCommmand = new RelayCommand(async () => await RetrieveVersionsAsync()));
        public ICommand VerifyInstallCommand => _verifyInstallCommand ?? (_verifyInstallCommand = new RelayCommand(async () => await VerifyInstallAsync()));

        public async void InitializeAsync()
        {
            IsWaiting = false;
            var appSettings = ApplicationData.Current.LocalSettings.Values;
            ServerName = (string)((ApplicationDataCompositeValue)appSettings["GameSettings"])["SessionName"];
            IPAddress = (string)appSettings["Hostname"];
            IsWaiting = true;
            await UpdateServerStatusAsync();
            await RetrieveVersionsAsync();
            IsWaiting = false;
        }

        private async Task UpdateServerStatusAsync()
        {
            var sshResult = await SshService.ExecuteCommandAsync("pidof ShooterGameServer");

            if (sshResult == "Failed to connect")
            {
                ConnectionSuccess = false;
                ConnectionFailed = true;
                IsServerRunning = false;
            }
            else
            {
                ConnectionSuccess = true;
                ConnectionFailed = false;
                IsServerRunning = !string.IsNullOrEmpty(sshResult);
            }
        }

        private async Task RetrieveVersionsAsync()
        {
            StorageFolder localCache = ApplicationData.Current.LocalCacheFolder;
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;

            var versionFile = await localCache.CreateFileAsync("version.txt", CreationCollisionOption.OpenIfExists);
            string filePath = (string)appSettings.Values["Directory"] + "/version.txt";
            _ = await SshService.DownloadFileAsync(versionFile, filePath);
            var lines = await FileIO.ReadLinesAsync(versionFile);
            InstalledVersion = lines[0].Before(" ") ?? "Unknown";
            
            //If we ever use web content anywhere else, I'll make an HTTP service class, but for now that seemed excessive for one usage
            var httpClient = new HttpClient();
            var responseMessage = await httpClient.GetAsync("http://arkdedicated.com/version");
            var siteContent = await responseMessage.Content.ReadAsStringAsync();
            //I thought it would be good to have some sort of check to make sure the site response
            //is probably a version number while still being flexible and futureproof
            LatestVersion = (siteContent.Length <= 10) ? siteContent : "Unknown";
        }

        private async Task VerifyInstallAsync()
        {
            IsWaiting = true;
            string gameDir = (string)ApplicationData.Current.LocalSettings.Values["Directory"];
            await SshService.ExecuteCommandInStreamAsync($"steamcmd +login anonymous +force_install_dir {gameDir} +app_update 376030 validate +quit");
            IsWaiting = false;
        }

        public async Task SaveGameAsync()
        {
            if (IsServerRunning)
            {
                _ = await RconService.SendCommandAsync("SaveWorld");
            }
            else
            {
                SaveErrorOpen = true;
            }
        }

        public async Task UpdateGameAsync()
        {
            IsWaiting = true;
            string gameDir = (string)ApplicationData.Current.LocalSettings.Values["Directory"];
            await SshService.ExecuteCommandInStreamAsync($"steamcmd +login anonymous +force_install_dir {gameDir} +app_update 376030 +quit");
            IsWaiting = false;
        }

        public async Task ToggleServerAsync()
        {
            if (IsServerRunning)
            {
                await StopServerAsync();
            }
            else
            {
                await StartServerAsync();
            }
        }

        private async Task StartServerAsync()
        {
            string gameDir = (string)ApplicationData.Current.LocalSettings.Values["Directory"];
            string scriptName = (string)ApplicationData.Current.LocalSettings.Values["ScriptName"];
            IsWaiting = true;
            //Don't await because it never returns until the server's stopped
            _ = SshService.ExecuteCommandAsync($"cd {gameDir}/ShooterGame/Binaries/Linux && ./{scriptName}");

            //Checks for 15 seconds to see if server successfully started
            int refreshCount = 0;
            while (refreshCount < 15 && !IsServerRunning)
            {
                await Task.Delay(1000);
                refreshCount++;
                await UpdateServerStatusAsync();
            }

            IsWaiting = false;

            if (!IsServerRunning)
            {
                //Make error appear
            }
        }

        private async Task StopServerAsync()
        {
            IsWaiting = true;
            _ = SshService.ExecuteCommandAsync("killall ShooterGameServer");
            int refreshCount = 0;
            while (refreshCount < 15 && IsServerRunning)
            {
                await Task.Delay(1000);
                refreshCount++;
                await UpdateServerStatusAsync();
            }

            IsWaiting = false;

            if (IsServerRunning)
            {
                //Make error appear
            }
        }
    }
}
