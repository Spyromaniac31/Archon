using Archon.Helpers;
using Archon.Models;
using Archon.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using WinUI = Microsoft.UI.Xaml.Controls;

namespace Archon.ViewModels
{
    public class ConfigurationViewModel : ObservableObject
    {
        private ICommand _saveSettingsCommand;
        private ICommand _itemInvokedCommand;
        private ObservableCollection<GroupInfoList> _selectedSettingGroup;
        private AutoSuggestBox _autoSuggestBox;
        private WinUI.NavigationView _navigationView;
        private List<GameSetting> AllSettings { get; set; } = new List<GameSetting>();
        private List<string> SearchSuggestions { get; set; } = new List<string>();

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

        public async void InitializeAsync(WinUI.NavigationView navigationView, AutoSuggestBox autoSuggestBox)
        {
            _navigationView = navigationView;
            await RetrieveSettingsAsync();
            _navigationView.SelectedItem = NavItems[0];
            SelectedSettingGroup = SettingGroups["ServerSettings"];
            _autoSuggestBox = autoSuggestBox;
            _autoSuggestBox.TextChanged += OnSearchTextChanged;
            _autoSuggestBox.QuerySubmitted += OnQuerySubmitted;
            _autoSuggestBox.SuggestionChosen += OnSuggestionChosen;
        }

        private void OnSearchTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            string text = sender.Text.ToLower();
            if (text == "")
            {
                if (_navigationView.SelectedItem == null)
                {
                    _navigationView.SelectedItem = NavItems[0];
                    SelectedSettingGroup = SettingGroups[(string)((SourceNavItem)_navigationView.SelectedItem).Tag];
                }
                return;
            }
            SearchSuggestions = AllSettings.Where(s => s.Name.ToLower().Contains(text) || s.Description.ToLower().Contains(text)).Select(s => s.Name).ToList();
            _autoSuggestBox.ItemsSource = SearchSuggestions;
        }

        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string text = sender.Text.ToLower();
            if (text == "")
            {
                if (_navigationView.SelectedItem == null)
                {
                    _navigationView.SelectedItem = NavItems[0];
                    SelectedSettingGroup = SettingGroups[(string)((SourceNavItem)_navigationView.SelectedItem).Tag];
                }
                return;
            }

            var settingResults = AllSettings.Where(s => s.Name.ToLower().Contains(text) || s.Description.ToLower().Contains(text)).ToList();
            SelectedSettingGroup = SettingGrouper.GroupSettingList(settingResults);
            _navigationView.SelectedItem = null;
        }

        private void OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            OnQuerySubmitted(sender, new AutoSuggestBoxQuerySubmittedEventArgs());
        }

        //This should be updated as settings sources are added
        private async Task RetrieveSettingsAsync()
        {
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;
            var gameSettings = (ApplicationDataCompositeValue)appSettings.Values["GameSettings"];

            SettingGroups.Add("ServerSettings", await DatabaseService.GetGroupedSettingsAsync("basegame"));
            NavItems.Add(new SourceNavItem("Base game", "ServerSettings", "\uEA67"));
            AllSettings.AddRange(await DatabaseService.GetSettingsAsync("basegame"));

            if (gameSettings.ContainsKey("ActiveMods") && gameSettings["ActiveMods"].ToString().Contains("731604991"))
            {
                SettingGroups.Add("StructuresPlus", await DatabaseService.GetGroupedSettingsAsync("structuresplus"));
                NavItems.Add(new SourceNavItem("Structures Plus", "StructuresPlus", "\uEA81"));
                AllSettings.AddRange(await DatabaseService.GetSettingsAsync("structuresplus"));
            }

            if (gameSettings.ContainsKey("Map") && gameSettings["Map"].ToString() == "Ragnarok")
            {
                SettingGroups.Add("Ragnarok", await DatabaseService.GetGroupedSettingsAsync("ragnarok"));
                NavItems.Add(new SourceNavItem("Ragnarok", "Ragnarok", "\uEA83"));
                AllSettings.AddRange(await DatabaseService.GetSettingsAsync("ragnarok"));
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
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;
            StorageFolder localCache = ApplicationData.Current.LocalCacheFolder;

            StorageFile scriptFile = await localCache.CreateFileAsync((string)appSettings.Values["ScriptName"], CreationCollisionOption.OpenIfExists);
            StorageFile gameFile = await localCache.CreateFileAsync("Game.ini", CreationCollisionOption.OpenIfExists);
            StorageFile gameUserFile = await localCache.CreateFileAsync("GameUserSettings.ini", CreationCollisionOption.OpenIfExists);

            //We create the backup first before we overwrite the local files with the new saves
            string backupName = $"{(string)appSettings.Values["Directory"]}/ArchonBackup".AddTimeStamp();
            if (await SshService.CreateDirectoryAsync(backupName))
            {
                _ = await SshService.UploadFileAsync(scriptFile, backupName + "/" + (string)appSettings.Values["ScriptName"]);
                _ = await SshService.UploadFileAsync(gameFile, backupName + "/Game.ini");
                _ = await SshService.UploadFileAsync(gameUserFile, backupName + "/GameUserSettings.ini");
            }

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

            var graphicsFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/graphics.txt"));
            await FileIO.AppendLinesAsync(gameUserFile, await FileIO.ReadLinesAsync(graphicsFile));

            //Changes files from CRLF to LF
            foreach (var file in new List<StorageFile>(){ scriptFile, gameFile, gameUserFile})
            {
                await FileIO.WriteTextAsync(file, (await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8)).Replace("\r\n", "\n"));
            }

            _ = await SshService.UploadFileAsync(scriptFile, $"{(string)appSettings.Values["Directory"]}/ShooterGame/Binaries/Linux/{(string)appSettings.Values["ScriptName"]}");
            //_ = await SshService.ExecuteCommandAsync($"chmod +x {(string)appSettings.Values["Directory"]}/ShooterGame/Binaries/Linux/{(string)appSettings.Values["ScriptName"]}");
            _ = await SshService.UploadFileAsync(gameFile, (string)appSettings.Values["Directory"] + "/ShooterGame/Saved/Config/LinuxServer/Game.ini");
            _ = await SshService.UploadFileAsync(gameUserFile, (string)appSettings.Values["Directory"] + "/ShooterGame/Saved/Config/LinuxServer/GameUserSettings.ini");
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
