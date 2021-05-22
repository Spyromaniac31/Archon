using Archon.Models;
using Archon.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Archon.ViewModels
{
    public class ConfigurationViewModel : ObservableObject
    {
        public ObservableCollection<GroupInfoList> SettingsGroups;

        public ConfigurationViewModel()
        {

        }

        public async Task InitializeAsync()
        {
            SettingsGroups = await DatabaseService.GetGroupedSettingsAsync();
        }
    }
}
