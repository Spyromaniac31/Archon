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
            set => _sshClient = value;
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
            set => _sftpClient = value;
        }

        private static PasswordConnectionInfo GetConnectionInfo()
        {
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;
            string hostname = (string)appSettings.Values["Hostname"];
            string username = (string)appSettings.Values["Username"];
            string password = (string)appSettings.Values["Password"];

            return new PasswordConnectionInfo(hostname, username, password);
        }

        private static PasswordConnectionInfo GetBackupConnectionInfo()
        {
            ApplicationDataContainer appSettings = ApplicationData.Current.LocalSettings;
            string hostname = (string)appSettings.Values["BackupHostname"];
            string username = (string)appSettings.Values["Username"];
            string password = (string)appSettings.Values["Password"];

            return new PasswordConnectionInfo(hostname, username, password);
        }

        public static async Task<string> ExecuteCommandAsync(string command)
        {
            if (await ConnectSshAsync())
            {
                var sshCommand = SshClient.CreateCommand(command);
                //We want to make sure we await the output before we disconnect so that we get the entire output
                string output = await sshCommand.ExecuteAsync();
                SshClient.Disconnect();
                return output;
            }
            return "Failed to connect";
        }

        public static async Task<bool> UploadFileAsync(StorageFile localFile, string remotePath)
        {
            if (await ConnectSftpAsync())
            {
                var stream = await localFile.OpenStreamForReadAsync();
                SftpClient.UploadFile(stream, remotePath);
                SftpClient.Disconnect();
                return true;
            }
            return false;
        }

        public static async Task<bool> DownloadFileAsync(StorageFile localFile, string remotePath)
        {
            if (await ConnectSftpAsync())
            {
                try
                {
                    byte[] fileLines = SftpClient.ReadAllBytes(remotePath);
                    await FileIO.WriteBytesAsync(localFile, fileLines);
                }
                catch
                {
                    try
                    {
                        //TODO: Sometimes this works when the other method doesn't? Still have to figure out what actually causes the stupid MAC exception
                        var stream = await localFile.OpenStreamForWriteAsync();
                        await SftpClient.DownloadAsync(remotePath, stream);
                    }
                    catch (Exception ex)
                    {
                        ErrorReporterService.ReportError("Download failed", "Archon ran into an issue while downloading a file. The app will use a previously cached version of the file, so setting information may be out-of-date. Error: " + ex.Message, "Warning");
                        return false;
                    }
                }

                SftpClient.Disconnect();
                return true;
            }
            return false;
        }

        private static async Task<bool> ConnectSshAsync()
        {
            if (SshClient.IsConnected)
            {
                return true;
            }
            try
            {
                await SshClient.ConnectAsync();
                return true;
            }
            catch
            {
                SshClient = new SshClient(GetBackupConnectionInfo());
                try
                {
                    await SshClient.ConnectAsync();
                    ErrorReporterService.ReportError("Backup IP used", "The primary IP address failed to connect, but Archon was able to connect using the backup IP address.", "Informational");
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorReporterService.ReportError("Connection failed", "Archon failed to establish an SSH connection using either IP address. Error: " + ex.Message, "Error");
                    return false;
                }
            }
        }
        
        private static async Task<bool> ConnectSftpAsync()
        {
            if (SftpClient.IsConnected)
            {
                return true;
            }
            try
            {
                await SftpClient.ConnectAsync();
                return true;
            }
            catch
            {
                SftpClient = new SftpClient(GetBackupConnectionInfo());
                try
                {
                    await SftpClient.ConnectAsync();
                    ErrorReporterService.ReportError("Backup IP used", "The primary IP address failed to connect, but Archon was able to connect using the backup IP address.", "Informational");
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorReporterService.ReportError("Connection failed", "Archon failed to establish an SFTP connection using either IP address. Error: " + ex.Message, "Error");
                    return false;
                }
            }
        }

        private static async Task ConnectAsync(this BaseClient client)
        {
            await Task.Run(client.Connect);
        }
    }
}
