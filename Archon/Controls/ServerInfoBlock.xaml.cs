using System;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Archon.Controls
{
    public sealed partial class ServerInfoBlock : UserControl
    {
        public ServerInfoBlock()
        {
            InitializeComponent();
            var appSettings = ApplicationData.Current.LocalSettings.Values;
            ServerNameTextBlock.Text = (string)((ApplicationDataCompositeValue)appSettings["GameSettings"])?["SessionName"] ?? "[No name]";
            PrimaryIPTextBlock.Text = (string)appSettings["Hostname"];
            BackupIPTextBlock.Text = (string)appSettings["BackupHostname"];
            string mapName = (string)((ApplicationDataCompositeValue)appSettings["GameSettings"])?["Map"] ?? "TheIsland";
            LogoImage.Source = new BitmapImage(new Uri(BaseUri, string.Format($"/Assets/Logos/{mapName}.png")));
        }
    }
}
