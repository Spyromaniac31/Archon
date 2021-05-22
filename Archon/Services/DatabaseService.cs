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
        private static Dictionary<string, string> categories { get; } = new Dictionary<string, string>()
        {
            ["player"] = "🧔🏽 Player",
            ["dino"] = "🦎 Dino",
            ["time"] = "🌦 Time and Weather",
            ["breeding"] = "🥚 Breeding",
            ["gameplay"] = "🎮 Gameplay",
            ["xp"] = "📈 XP and Engrams",
            ["tribe"] = "🤝🏽 Tribe",
            ["resources"] = "🪨 Resources",
            ["server"] = "🖥 Server",
            ["structure"] = "🛖 Structures",
            ["platform"] = "🛖 Structures",
            ["cryo"] = "❄️ Cryopods",
            ["pve"] = "🧑🏽‍🤝‍🧑🏾 PVE",
            ["tribute"] = "🌐 Tribute"
        };
        public static SqliteConnection SqliteConnection { get; private set; }

        private static async Task InitializeConnection()
        {
            if (SqliteConnection != null)
            {
                return;
            }
            Uri uri = new Uri("ms-appx:///Data/archon.db");
            StorageFile dbFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
            SqliteConnection = new SqliteConnection($"Filename={dbFile.Path}");
        }

        public static async Task<ObservableCollection<GameSetting>> GetSettingsByCategoryAsync(string category)
        {
            await InitializeConnection();
            var settings = new ObservableCollection<GameSetting>();
            SqliteConnection.Open();

            //Creates SELECT command using method parameter
            string commandText = "SELECT * FROM settings WHERE category = $category";
            var selectCommand = new SqliteCommand(commandText, SqliteConnection);
            _ = selectCommand.Parameters.AddWithValue("$category", category);

            var dataReader = selectCommand.ExecuteReader();

            //Adds returned records to the ObservableCollection
            while (dataReader.Read())
            {
                settings.Add(new GameSetting()
                {
                    Name = !dataReader.IsDBNull(0) ? dataReader.GetString(0) : null,
                    Description = !dataReader.IsDBNull(1) ? dataReader.GetString(1) : null,
                    Hint = !dataReader.IsDBNull(2) ? dataReader.GetString(2) : null,
                    Type = !dataReader.IsDBNull(3) ? dataReader.GetString(3) : null,
                    Category = !dataReader.IsDBNull(4) ? dataReader.GetString(4) : null,
                    File = (File)(!dataReader.IsDBNull(5) ? Enum.Parse(typeof(File), dataReader.GetString(5)) : null),
                    DefaultValue = !dataReader.IsDBNull(6) ? dataReader.GetString(6) : null,
                });
            }

            SqliteConnection.Close();

            return settings;
        }

        public static async Task<List<GameSetting>> GetAllSettingsAsync()
        {
            await InitializeConnection();
            var settings = new List<GameSetting>();
            SqliteConnection.Open();

            //Creates SELECT command using method parameter
            string commandText = "SELECT * FROM settings";
            var selectCommand = new SqliteCommand(commandText, SqliteConnection);

            var dataReader = selectCommand.ExecuteReader();

            //Adds returned records to the ObservableCollection
            while (dataReader.Read())
            {
                settings.Add(new GameSetting()
                {
                    Name = !dataReader.IsDBNull(0) ? dataReader.GetString(0) : null,
                    Description = !dataReader.IsDBNull(1) ? dataReader.GetString(1) : null,
                    Hint = !dataReader.IsDBNull(2) ? dataReader.GetString(2) : null,
                    Type = !dataReader.IsDBNull(3) ? dataReader.GetString(3) : null,
                    Category = !dataReader.IsDBNull(4) ? dataReader.GetString(4) : null,
                    File = (File)(!dataReader.IsDBNull(5) ? Enum.Parse(typeof(File), dataReader.GetString(5), true) : null),
                    DefaultValue = !dataReader.IsDBNull(6) ? dataReader.GetString(6) : null,
                });
            }

            SqliteConnection.Close();

            return settings;
        }

        public static async Task<ObservableCollection<GroupInfoList>> GetGroupedSettingsAsync()
        {
            IEnumerable<GroupInfoList> query = from setting in await GetAllSettingsAsync()
                                               group setting by categories[setting.Category] into g
                                               select new GroupInfoList(g) { Key = g.Key };
            return new ObservableCollection<GroupInfoList>(query);
        }


    }
}
