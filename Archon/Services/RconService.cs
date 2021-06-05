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

        public async static Task<string> SendCommandAsync(string command)
        {
            await RconClient.ConnectAsync();
            string response = await RconClient.SendCommandAsync(command);
            return response;
        }

    }
}
