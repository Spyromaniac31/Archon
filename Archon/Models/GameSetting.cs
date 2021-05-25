using Archon.ViewModels;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Archon.Models
{
    public class GameSetting : ObservableObject
    {
        private string _name;
        private string _description;
        private string _hint;
        private string _type;
        private string _category;
        private File _file;
        private string _defaultValue;
        private string _currentValue;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
        public string Hint
        {
            get => _hint;
            set => SetProperty(ref _hint, value);
        }
        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }
        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }
        public File File
        {
            get => _file;
            set => SetProperty(ref _file, value);
        }
        public string DefaultValue
        {
            get => _defaultValue;
            set => SetProperty(ref _defaultValue, value);
        }
        public string CurrentValue
        {
            get => _currentValue;
            set => SetProperty(ref _currentValue, value);
        }

        public void UpdateCurrentValue()
        {
            var gameSettings = (ApplicationDataCompositeValue)ApplicationData.Current.LocalSettings.Values["GameSettings"];
            CurrentValue = gameSettings.ContainsKey(Name) ? (string)gameSettings[Name] : DefaultValue;
        }
        
        public bool IsTypePercent => Type == "percent";
        public bool IsTypeString => Type == "string";
        public bool IsTypeBool => Type == "boolean" || Type == "arg";
        public bool IsTypeNumber => Type == "number";
        
        public GameSettingControlViewModel ViewModel => new GameSettingControlViewModel(Name, Description, Hint, Type, DefaultValue);
    }

    public enum File
    {
        GameUserSettings,
        Game,
        StartScript
    }

}
