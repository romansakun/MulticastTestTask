using System.Collections.Generic;
using MessagePack;

namespace GameLogic.Model.Definitions
{
    [MessagePackObject]
    public class LocalizationDef : BaseDef 
    {
        [Key(1)]
        public string Description { get; set; } = string.Empty;
        [Key(2)]
        public List<string> TutorialVideos { get; set; } = new();
        [Key(3)]
        public Dictionary<int, string> Levels { get; set; } = new();
        [Key(4)]
        public Dictionary<int, string> Leagues { get; set; } = new();
        [Key(5)]
        public Dictionary<string, string> LocalizationText { get; set; } = new();
    }
}