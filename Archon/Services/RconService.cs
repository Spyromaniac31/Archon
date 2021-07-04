using CoreRCON;
using System;
using System.Net;
using System.Threading.Tasks;
using Windows.Storage;

namespace Archon.Services
{
    public static class RconService
    {
        private static RCON _rconClient;
        public static RCON RconClient
        {
            get
            {
                if (_rconClient == null)
                {
                    var appSettings = ApplicationData.Current.LocalSettings;
                    var gameSettings = appSettings.Values["GameSettings"] as ApplicationDataCompositeValue;

                    //TODO: Catch incorrectly formatted settings
                    var hostName = IPAddress.Parse((string)appSettings.Values["Hostname"]);
                    ushort port = ushort.Parse((string)gameSettings["RCONPort"]);
                    string password = (string)gameSettings["ServerAdminPassword"];

                    _rconClient = new RCON(hostName, port, password);
                }

                return _rconClient;
            }
        }

        public static async Task<string> SendCommandAsync(string command)
        {
            try
            {
                await RconClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                ErrorReporterService.ReportError("RCON failure", "Archon was unable to connect to your server using RCON. Ensure RCON is enabled, you have an ARK server admin password, and port forwarding is set up correctly. Error: " + ex.Message, "Error");
            }
            string response = await RconClient.SendCommandAsync(command);
            return response;
        }

    }
}
