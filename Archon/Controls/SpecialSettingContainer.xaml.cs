using Archon.Controls.SpecialSettingControllers;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Archon.Controls
{
    public sealed partial class SpecialSettingContainer : UserControl
    {
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public SpecialSettingContainer()
        {
            InitializeComponent();
            
        }

        public void InitializeSetting()
        {
            switch (SettingName)
            {
                case "DinoClassDamageMultipliers":
                    ControlGrid.Children.Add(new ClassNameMultiplierControl());
                    break;
                default:
                    break;
            }
        }
    }
}