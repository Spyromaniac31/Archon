using Archon.Services;
using Renci.SshNet;
using Renci.SshNet.Common;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Archon.Controls
{
    public sealed partial class TerminalOutputBlock : UserControl
    {
        public TerminalOutputBlock()
        {
            InitializeComponent();
            SshService.DataReceived += ShellStream_DataReceived;
            TerminalText.Text = "Welcome to Archon!\nHere you'll see the terminal output of certain commands such as updating and verification.\n";
        }

        private async void ShellStream_DataReceived(object sender, ShellDataEventArgs e)
        {
            var shellStream = (ShellStream)sender;
            string line = string.Empty;
            try
            {
                while ((line = shellStream.ReadLine(TimeSpan.FromSeconds(3))) != null)
                {
                    //We run the UI update through the dispatcher because this method 
                    //will likely not be called from the UI thread
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                    {
                        TerminalText.Text += line + "\n";
                        _ = TerminalScrollViewer.ChangeView(0.0f, double.MaxValue, 1.0f);
                    });
                }
            }
            catch (Exception ex)
            {
                //TODO: Implement terminal error messages
            }
        }
    }
}
