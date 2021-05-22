using Archon.ViewModels;
using System.ComponentModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Archon.Controls
{
    public sealed partial class GameSettingControl : UserControl
    {
        public GameSettingControlViewModel ViewModel { get; }

        public GameSettingControl()
        {
            InitializeComponent();
        }

        private void LoadSettingValue(string settingValue)
        {
            switch (ViewModel.SettingType)
            {
                case "percent":
                    PercentNumber.Value = double.Parse(settingValue);
                    break;
                case "number":
                    if (!string.IsNullOrEmpty(settingValue))
                    {
                        NumberBox.Value = double.Parse(settingValue);
                    }
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

        private void UserControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            /*ViewModel.SettingName = SettingName;
            ViewModel.SettingDescription = SettingDescription;
            ViewModel.SettingHint = SettingHint;
            ViewModel.SettingType = SettingType;
            ViewModel.SettingDefaultValue = SettingDefaultValue;*/

            var gameSettings = (ApplicationDataCompositeValue)ApplicationData.Current.LocalSettings.Values["GameSettings"];
            if (gameSettings.ContainsKey(ViewModel.SettingName))
            {
                LoadSettingValue((string)gameSettings[ViewModel.SettingName]);
            }
            else
            {
                LoadSettingValue(ViewModel.SettingDefaultValue);
            }
        }
    }
}
