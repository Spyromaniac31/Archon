using Windows.Storage;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Archon.Controls
{
    public sealed partial class GameSettingControl : UserControl
    {
        public string SettingName { get; set; }
        public string SettingDescription { get; set; }
        public string SettingHint { get; set; }
        public string SettingType { get; set; }
        public string SettingDefaultValue { get; set; }

        public bool IsTypePercent => SettingType == "percent";
        public bool IsTypeNumber => SettingType == "number";
        public bool IsTypeBool => SettingType == "boolean";
        public bool IsTypeString => SettingType == "string";

        public GameSettingControl()
        {
            InitializeComponent();
            var gameSettings = (ApplicationDataCompositeValue)ApplicationData.Current.LocalSettings.Values["GameSettings"];
            if (gameSettings.ContainsKey(SettingName))
            {
                LoadSetting((string)gameSettings[SettingName]);
            }
            else
            {
                LoadSetting(SettingDefaultValue);
            }
        }

        private void LoadSetting(string settingValue)
        {
            switch (SettingType)
            {
                case "percent":
                    PercentNumber.Value = double.Parse(settingValue);
                    break;
                case "number":
                    NumberBox.Value = double.Parse(settingValue);
                    break;
                case "boolean":
                    BoolSwitch.IsOn = bool.Parse(settingValue);
                    break;
                case "string":
                    StringBox.Text = settingValue;
                    break;
                default:
                    //TODO: Throw an error?
                    break;
            }

        }

        private void ToggleHint()
        {
            HintTip.IsOpen = !HintTip.IsOpen;
        }
    }
}
