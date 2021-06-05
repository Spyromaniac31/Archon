using Archon.Models;
using Archon.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;

namespace Archon.ViewModels
{
    public class ConfigurationViewModel : ObservableObject
    {
        private ICommand _saveSettingsCommand;

        public ObservableCollection<GroupInfoList> SettingsGroups;

        public ICommand SaveSettingsCommand => _saveSettingsCommand ?? (_saveSettingsCommand = new RelayCommand(async () => await SaveSettingsAsync()));

        public ConfigurationViewModel()
        {

        }

        public async Task InitializeAsync()
        {
            SettingsGroups = await DatabaseService.GetGroupedSettingsAsync();
        }

        public async Task SaveSettingsAsync()
        {
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;
            StorageFolder localCache = ApplicationData.Current.LocalCacheFolder;

            List<string> gameLines = new List<string>() { "[/script/shootergame.shootergamemode]" };
            List<string> gameUserLines = new List<string>() { "[ServerSettings]" };
            List<string> scriptSettings = new List<string>();
            List<string> scriptArgs = new List<string>() { " -server" };

            foreach (GroupInfoList group in SettingsGroups)
            {
                foreach (GameSetting setting in group)
                {
                    if (setting.GetFormattedLine() != "")
                    {
                        switch (setting.File)
                        {
                            case File.Game:
                                gameLines.Add(setting.GetFormattedLine());
                                break;
                            case File.GameUserSettings:
                                gameUserLines.Add(setting.GetFormattedLine());
                                break;
                            case File.StartScript:
                                (setting.Type == "arg" ? scriptArgs : scriptSettings).Add(setting.GetFormattedLine());
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            StorageFile ScriptFile = await localCache.CreateFileAsync((string)appSettings.Values["ScriptName"], CreationCollisionOption.OpenIfExists);
            StorageFile GameFile = await localCache.CreateFileAsync("Game.ini", CreationCollisionOption.OpenIfExists);
            StorageFile GameUserFile = await localCache.CreateFileAsync("GameUserSettings.ini", CreationCollisionOption.OpenIfExists);

            string startLine = "./ShooterGameServer " + (string)((ApplicationDataCompositeValue)appSettings.Values["GameSettings"])["Map"] + "?listen";
            foreach (string setting in scriptSettings)
            {
                startLine += setting;
            }
            foreach (string arg in scriptArgs)
            {
                startLine += arg;
            }

            await FileIO.WriteLinesAsync(GameFile, gameLines);
            await FileIO.WriteLinesAsync(GameUserFile, gameUserLines);
            await FileIO.WriteLinesAsync(ScriptFile, new List<string>() { "#! /bin/bash", startLine });
        }
    }
}
