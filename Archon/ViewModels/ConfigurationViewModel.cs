using Archon.Models;
using Archon.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace Archon.ViewModels
{
    public class ConfigurationViewModel : ObservableObject
    {
        private ICommand _saveSettingsCommand;
        private ICommand _itemInvokedCommand;
        private ObservableCollection<GroupInfoList> _selectedSettingGroup;
        private WinUI.NavigationView _navigationView;

        public Dictionary<string, ObservableCollection<GroupInfoList>> SettingGroups = new Dictionary<string, ObservableCollection<GroupInfoList>>();

        public ObservableCollection<GroupInfoList> SelectedSettingGroup
        {
            get => _selectedSettingGroup;
            set => SetProperty(ref _selectedSettingGroup, value);
        }

        public ObservableCollection<SourceNavItem> NavItems = new ObservableCollection<SourceNavItem>();

        //public ObservableCollection<GroupInfoList> SettingsGroups;

        public ICommand SaveSettingsCommand => _saveSettingsCommand ?? (_saveSettingsCommand = new RelayCommand(async () => await SaveSettingsAsync()));

        public ICommand ItemInvokedCommand => _itemInvokedCommand ?? (_itemInvokedCommand = new RelayCommand<WinUI.NavigationViewItemInvokedEventArgs>(OnItemInvoked));

        public ConfigurationViewModel()
        {

        }

        public async Task InitializeAsync(WinUI.NavigationView navigationView)
        {
            _navigationView = navigationView;
            await RetrieveSettingsAsync();
            SelectedSettingGroup = SettingGroups["ServerSettings"];
            //navigationView.SelectedItem = navigationView.MenuItems[0];
        }

        //This should be updated as settings sources are added
        private async Task RetrieveSettingsAsync()
        {
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;
            var gameSettings = (ApplicationDataCompositeValue)appSettings.Values["GameSettings"];

            SettingGroups.Add("ServerSettings", await DatabaseService.GetGroupedSettingsAsync("settings"));
            NavItems.Add(new SourceNavItem("Base game", "ServerSettings", "\xEA67"));

            if (gameSettings.ContainsKey("ActiveMods") && gameSettings["ActiveMods"].ToString().Contains("731604991"))
            {
                SettingGroups.Add("StructuresPlus", await DatabaseService.GetGroupedSettingsAsync("structuresplus"));
                NavItems.Add(new SourceNavItem("Structures Plus", "StructuresPlus", "\xEA81"));
            }
        }

        private void OnItemInvoked(WinUI.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is WinUI.NavigationViewItem selectedItem)
            {
                SelectedSettingGroup = SettingGroups[(string)selectedItem.Tag];
            }

        }

        public async Task SaveSettingsAsync()
        {
            foreach (var x in _navigationView.MenuItems)
            {
                var b = 4;
            }
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;
            StorageFolder localCache = ApplicationData.Current.LocalCacheFolder;

            List<string> gameLines = new List<string>() { "[/script/shootergame.shootergamemode]" };

            Dictionary<string, List<string>> gameUserLinesSections = new Dictionary<string, List<string>>();
            //List<string> gameUserLines = new List<string>() { "[ServerSettings]" };
            List<string> scriptSettings = new List<string>();
            List<string> scriptArgs = new List<string>() { " -server" };

            foreach (KeyValuePair<string, ObservableCollection<GroupInfoList>> source in SettingGroups)
            {
                foreach (GroupInfoList group in source.Value)
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
                                    string section = source.Key;
                                    if (setting.Name == "SessionName")
                                    {
                                        section = "SessionSettings";
                                    }
                                    else if (setting.Name == "MaxPlayers")
                                    {
                                        section = "/Script/Engine.GameSession";
                                    }
                                    if (!gameUserLinesSections.ContainsKey(section))
                                    {
                                        gameUserLinesSections[section] = new List<string>();
                                    }
                                    gameUserLinesSections[section].Add(setting.GetFormattedLine());
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
            }
            

            StorageFile scriptFile = await localCache.CreateFileAsync((string)appSettings.Values["ScriptName"], CreationCollisionOption.OpenIfExists);
            StorageFile gameFile = await localCache.CreateFileAsync("Game.ini", CreationCollisionOption.OpenIfExists);
            StorageFile gameUserFile = await localCache.CreateFileAsync("GameUserSettings.ini", CreationCollisionOption.OpenIfExists);

            string startLine = "./ShooterGameServer " + (string)((ApplicationDataCompositeValue)appSettings.Values["GameSettings"])["Map"] + "?listen";
            foreach (string setting in scriptSettings)
            {
                startLine += setting;
            }
            foreach (string arg in scriptArgs)
            {
                startLine += arg;
            }
            await FileIO.WriteLinesAsync(scriptFile, new List<string>() { "#! /bin/bash", startLine });

            await FileIO.WriteLinesAsync(gameFile, gameLines);

            await FileIO.WriteTextAsync(gameUserFile, "");
            foreach (KeyValuePair<string, List<string>> section in gameUserLinesSections)
            {
                List<string> groupedSection = new List<string>() { $"[{section.Key}]" };
                groupedSection.AddRange(section.Value);
                //Adds a line between sections for readability
                groupedSection.Add("");
                await FileIO.AppendLinesAsync(gameUserFile, groupedSection);
            }
            
        }
    }

    public class SourceNavItem
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Glyph { get; set; }

        public SourceNavItem(string name, string tag, string glyph)
        {
            Name = name;
            Tag = tag;
            Glyph = glyph;
        }
    }

}
