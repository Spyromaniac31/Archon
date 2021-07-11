using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Archon.Controls
{
    public sealed partial class DialogOpenerButton : UserControl
    {
        public static readonly DependencyProperty SettingValueProperty = DependencyProperty.Register(
            "SettingValue",
            typeof(string),
            typeof(DialogOpenerButton),
            new PropertyMetadata(null)
        );

        public string SettingValue
        {
            get => (string)GetValue(SettingValueProperty);
            set => SetValue(SettingValueProperty, value);
        }
        public string SettingName { get; set; }
        public DialogOpenerButton()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = SettingName;
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = SettingValue;

            var dialogResult = await dialog.ShowAsync();
            SettingValue = dialogResult == ContentDialogResult.Primary ? "Primary" : "Closed";
        }
    }
}
