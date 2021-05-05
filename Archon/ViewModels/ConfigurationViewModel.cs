using Archon.Models;
using Archon.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Archon.ViewModels
{
    public class ConfigurationViewModel : ObservableObject
    {
        private ObservableCollection<GroupInfoList> settingsGroups;

        public ConfigurationViewModel()
        {

        }

        public async Task InitializeAsync()
        {
            settingsGroups = await DatabaseService.GetGroupedSettingsAsync();
        }
    }
}
