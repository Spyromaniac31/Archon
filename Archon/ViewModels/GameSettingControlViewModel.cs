using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Archon.ViewModels
{
    public class GameSettingControlViewModel : ObservableObject
    {
        private string _settingName;
        private string _settingDescription;
        private string _settingHint;
        private string _settingType;
        private string _settingDefaultValue;

        public string SettingName
        {
            get => _settingName;
            set => SetProperty(ref _settingName, value);
        }

        public string SettingDescription
        {
            get => _settingDescription;
            set => SetProperty(ref _settingDescription, value);
        }

        public string SettingHint
        {
            get => _settingHint;
            set => SetProperty(ref _settingHint, value);
        }

        public string SettingType
        {
            get => _settingType;
            set => SetProperty(ref _settingType, value);
        }

        public string SettingDefaultValue
        {
            get => _settingDefaultValue;
            set => SetProperty(ref _settingDefaultValue, value);
        }

        public bool IsTypePercent => SettingType == "percent";
        public bool IsTypeNumber => SettingType == "number";
        public bool IsTypeBool => SettingType == "boolean";
        public bool IsTypeString => SettingType == "string";

        public GameSettingControlViewModel(string name, string description, string hint, string type, string defaultValue)
        {
            SettingName = name;
            SettingDescription = description;
            SettingHint = hint;
            SettingType = type;
            SettingDefaultValue = defaultValue;
        }

        public GameSettingControlViewModel()
        {
        }
    }
}
