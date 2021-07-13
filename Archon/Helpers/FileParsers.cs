using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Archon.Helpers
{
    public static class FileParsers
    {
        /// <summary>
        /// Extracts the individual settings and arguments from a start script.
        /// </summary>
        /// <param name="script">The script file to be parsed</param>
        /// <returns>A Dictionary containing each of the settings and arguments in the script.</returns>
        public static async Task<Dictionary<string, string>> ParseScriptAsync(StorageFile script)
        {
            var scriptLines = await FileIO.ReadLinesAsync(script);

            //this retrieves the map name from the script and trims the stuff we don't need
            if (scriptLines.Count == 0)
            {
                return new Dictionary<string, string>();
            }
            string mainLine = scriptLines[scriptLines.Count - 1];
            Dictionary<string, string> settings = new Dictionary<string, string>
            {
                ["Map"] = mainLine.Between("ShooterGameServer ", "?")
            };
            string trimmedLine = mainLine.After("listen?");

            //this separates each individual argument and setting
            string[] argsAndSettings = trimmedLine.Split(new string[] { "?", " -" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string parameter in argsAndSettings)
            {
                //This means it's a setting
                if (parameter.Contains("="))
                {
                    string settingName = parameter.Before("=");
                    string settingValue = parameter.After("=");
                    settings[settingName] = settingValue;
                }
                //This means it's an argument
                else
                {
                    settings[parameter] = "true";
                }
            }

            return settings;
        }

        /// <summary>
        /// Extracts the individual settings from a .ini file
        /// </summary>
        /// <param name="ini">The .ini file to be parsed</param>
        /// <returns>A Dictionary containing each of the settings in the ini file.</returns>
        /// TODO: Deal with special settings
        public static async Task<Dictionary<string, string>> ParseIniAsync(StorageFile ini)
        {
            var settings = new Dictionary<string, string>();
            var fileLines = await FileIO.ReadLinesAsync(ini);

            foreach (string line in fileLines)
            {
                if (line.Contains("="))
                {
                    string settingName = line.Before("=");
                    string settingValue = line.After("=");

                    if (settings.ContainsKey(settingName))
                    {
                        if (settings[settingName] != settingValue)
                        {
                            settings[settingName] += "," + settingValue;
                        }
                    }
                    else
                    {
                        settings[settingName] = settingValue;
                    }
                }
            }

            return settings;
        }
    }
}
