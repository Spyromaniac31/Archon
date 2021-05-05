using Renci.SshNet;
using Renci.SshNet.Async;
using Renci.SshNet.Common;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Archon.Services
{
    public static class SshService
    {
        private static SshClient _sshClient;
        public static SshClient SshClient
        {
            get
            {
                if (_sshClient == null)
                {
                    _sshClient = new SshClient(GetConnectionInfo());
                }
                return _sshClient;
            }
        }

        private static SftpClient _sftpClient;
        public static SftpClient SftpClient
        {
            get
            {
                if (_sftpClient == null)
                {
                    _sftpClient = new SftpClient(GetConnectionInfo());
                }
                return _sftpClient;
            }
        }

        private static PasswordConnectionInfo GetConnectionInfo()
        {
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;
            string hostname = appSettings.Values["Hostname"] as string;
            string username = appSettings.Values["Username"] as string;
            string password = appSettings.Values["Password"] as string;

            return new PasswordConnectionInfo(hostname, username, password);
        }

        public static string ExecuteCommand(string command)
        {
            //The client might already be connected if an async command is being run
            if (!SshClient.IsConnected)
            {
                SshClient.Connect();
            }
            var sshCommand = SshClient.CreateCommand(command);
            string output = sshCommand.Execute();
            SshClient.Disconnect();
            return output;
        }

        public static async Task<string> ExecuteCommandAsync(string command)
        {
            //The client might already be connected if an async command is being run
            if (!SshClient.IsConnected)
            {
                SshClient.Connect();
            }
            var sshCommand = SshClient.CreateCommand(command);
            try
            {
                Task<string> output = sshCommand.ExecuteAsync();
                return await output;
            }
            catch (InvalidOperationException ex)
            {
                //TODO: Implement error
            }
            finally
            {
                //This could be harmful if the sshclient is disconnected while a parallel command is being executed
                SshClient.Disconnect();
            }
            return null;
        }

        public static async Task UploadFileAsync(StorageFile localFile, string remotePath)
        {
            SftpClient.Connect();
            var stream = await localFile.OpenStreamForReadAsync();
            SftpClient.UploadFile(stream, remotePath);
            SftpClient.Disconnect();
        }

        public static async Task DownloadFileAsync(StorageFile localFile, string remotePath)
        {
            SftpClient.Connect();
            //Stream stream = await localFile.OpenStreamForWriteAsync();
            try
            {
                byte[] fileLines = SftpClient.ReadAllBytes(remotePath);
                await FileIO.WriteBytesAsync(localFile, fileLines);
                //await SftpClient.DownloadAsync(remotePath, stream);
            }
            catch (SshConnectionException ex)
            {
                //TODO: Show an Infobar on the main page
            }
            SftpClient.Disconnect();
        }

    }
}
