using Archon.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archon.Helpers
{
    public class SettingGrouper
    {
        private static Dictionary<string, string> Categories { get; } = new Dictionary<string, string>()
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
            ["tribute"] = "🌐 Tribute",
            ["slot"] = "🎒 Slot Counts",
            ["speed"] = "💨 Crafting Speeds",
            ["range"] = "🎯 Structure Ranges",
            ["general"] = "🌐 General Settings",
            ["functionality"] = "⚙ Structure Functionality",
            ["building"] = "🏗 Building and Placement",
            ["list"] = "📃 Item lists",
            ["unicorn"] = "🦄 Unicorn",
            ["volcano"] = "🌋 Volcano"
        };

        public static ObservableCollection<GroupInfoList> GroupSettingList(List<GameSetting> allSettings)
        {
            IEnumerable<GroupInfoList> query = from setting in allSettings
                                               group setting by Categories[setting.Category] into g
                                               select new GroupInfoList(g) { Key = g.Key };
            return new ObservableCollection<GroupInfoList>(query);
        }
    }
}
