using Archon.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
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
        private bool _isServerRunning;
        private bool _saveErrorOpen = false;
        private bool _updateErrorOpen = false;
        private bool _isWaiting = false;
        private string _serverName;
        private string _ipAddress;

        public static event EventHandler<StatusChangedEventArgs> RaiseStatusChangedEvent;

        public bool IsServerRunning
        {
            get
            {
                UpdateServerStatus();
                return _isServerRunning;
            }
            private set
            {
                if (value != _isServerRunning)
                {
                    _isServerRunning = value;
                    RaiseStatusChangedEvent?.Invoke(this, new StatusChangedEventArgs(value));
                }
            }
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

        public ICommand SaveGameCommand => _saveGameCommand ?? (_saveGameCommand = new RelayCommand(async () => await SaveGameAsync()));
        public ICommand UpdateGameCommand => _updateGameCommand ?? (_updateGameCommand = new RelayCommand(async () => await UpdateGameAsync()));
        public ICommand ToggleServerCommand => _toggleServerCommand ?? (_toggleServerCommand = new RelayCommand(async () => await ToggleServerAsync()));

        public void Initialize()
        {
            IsWaiting = false;
            var appSettings = ApplicationData.Current.LocalSettings.Values;
            ServerName = (string)((ApplicationDataCompositeValue)appSettings["GameSettings"])["SessionName"];
            IPAddress = (string)appSettings["Hostname"];
            UpdateServerStatus();
        }

        private void UpdateServerStatus()
        {
            IsServerRunning = !string.IsNullOrEmpty(SshService.ExecuteCommand("pidof ShooterGameServer"));
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
            if (IsServerRunning)
            {
                UpdateErrorOpen = true;
            }
            else
            {
                IsWaiting = true;
                string gameDir = (string)ApplicationData.Current.LocalSettings.Values["Directory"];
                _ = await SshService.ExecuteCommandAsync($"steamcmd +login anonymous +force_install_dir {gameDir} +app_update 376030 +quit");
                IsWaiting = false;
            }
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
            _ = SshService.ExecuteCommandAsync($"cd {gameDir}/Binaries/Linux && ./{scriptName}");

            //Checks for 15 seconds to see if server successfully started
            int refreshCount = 0;
            while (refreshCount < 15 && !IsServerRunning)
            {
                await Task.Delay(1000);
                refreshCount++;
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
            }

            IsWaiting = false;

            if (IsServerRunning)
            {
                //Make error appear
            }
        }
    }
}
