namespace Archon.Models
{
    public class GameSetting
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Hint { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public File File { get; set; }
        public string DefaultValue { get; set; }
    }

    public enum File
    {
        GameUserSettings,
        Game,
        StartScript
    }

}
