﻿using Renci.SshNet;
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

        public static string ExecuteCommand(string command)
        {
            if (ConnectSsh())
            {
                var sshCommand = SshClient.CreateCommand(command);
                string output = sshCommand.Execute();
                SshClient.Disconnect();
                return output;
            }
            return "Failed to connect";
        }

        public static async Task<string> ExecuteCommandAsync(string command)
        {
            if (ConnectSsh())
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
            if (ConnectSftp())
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
            if (ConnectSftp())
            {
                try
                {
                    byte[] fileLines = SftpClient.ReadAllBytes(remotePath);
                    await FileIO.WriteBytesAsync(localFile, fileLines);
                }
                catch
                {
                    return false;
                    //var stream = await localFile.OpenStreamForWriteAsync();
                    //await SftpClient.DownloadAsync(remotePath, stream);
                }

                SftpClient.Disconnect();
                return true;
            }
            return false;
        }

        private static bool ConnectSsh()
        {
            if (SshClient.IsConnected)
            {
                return true;
            }
            try
            {
                SshClient.Connect();
                return true;
            }
            catch
            {
                //TODO: Show message about trying backup IP
                SshClient = new SshClient(GetBackupConnectionInfo());
                try
                {
                    SshClient.Connect();
                    return true;
                }
                catch
                {
                    //TODO: Show message
                    return false;
                }
            }
        }
        private static bool ConnectSftp()
        {
            if (SftpClient.IsConnected)
            {
                return true;
            }
            try
            {
                SftpClient.Connect();
                return true;
            }
            catch
            {
                //TODO: Show message about trying backup IP
                SftpClient = new SftpClient(GetBackupConnectionInfo());
                try
                {
                    SftpClient.Connect();
                    return true;
                }
                catch
                {
                    //TODO: Show message
                    return false;
                }
            }
        }
    }
}
