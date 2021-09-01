using Archon.Helpers;
using Archon.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Archon.Services
{
    public static class DatabaseService
    {
        public static SqliteConnection SqliteConnection { get; private set; }

        private static async Task InitializeConnection(string database)
        {
            if (SqliteConnection != null && SqliteConnection.DataSource.Contains(database))
            {
                return;
            }
            Uri uri = new Uri($"ms-appx:///Data/{database}.db");
            StorageFile dbFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
            SqliteConnection = new SqliteConnection($"Filename={dbFile.Path}");
        }

        public static async Task<List<GameSetting>> GetSettingsAsync(string table)
        {
            await InitializeConnection("settings");
            var settings = new List<GameSetting>();
            SqliteConnection.Open();

            //Creates SELECT command using method parameter
            string commandText = $"SELECT * FROM {table}";
            var selectCommand = new SqliteCommand(commandText, SqliteConnection);

            var dataReader = selectCommand.ExecuteReader();

            //Adds returned records to the ObservableCollection
            while (dataReader.Read())
            {
                GameSetting gameSetting = new GameSetting()
                {
                    Name = !dataReader.IsDBNull(0) ? dataReader.GetString(0) : null,
                    Description = !dataReader.IsDBNull(1) ? dataReader.GetString(1) : null,
                    Hint = !dataReader.IsDBNull(2) ? dataReader.GetString(2) : null,
                    Type = !dataReader.IsDBNull(3) ? dataReader.GetString(3) : null,
                    Category = !dataReader.IsDBNull(4) ? dataReader.GetString(4) : null,
                    File = (File)(!dataReader.IsDBNull(5) ? Enum.Parse(typeof(File), dataReader.GetString(5), true) : null),
                    DefaultValue = !dataReader.IsDBNull(6) ? dataReader.GetString(6) : null,
                };
                gameSetting.UpdateCurrentValue();
                settings.Add(gameSetting);
            }

            SqliteConnection.Close();

            return settings;
        }

        public static async Task<ObservableCollection<GroupInfoList>> GetGroupedSettingsAsync(string table)
        {
            return SettingGrouper.GroupSettingList(await GetSettingsAsync(table));
        }
    }
}
