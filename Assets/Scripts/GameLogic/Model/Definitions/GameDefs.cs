using System.Collections.Generic;
using MessagePack;

namespace GameLogic.Model.Definitions
{
    [MessagePackObject]
    public class GameDefs
    {
        [Key(0)] public DefaultSettingsDef DefaultSettings { get; set; }
        [Key(1)] public LevelSettingsDef LevelSettings { get; set; }
        [Key(2)] public Dictionary<string, LocalizationDef> Localizations { get; set; } = new();
        [Key(3)] public Dictionary<string, LevelDef> Levels { get; set; } = new();
        [Key(4)] public Dictionary<string, LeagueDef> Leagues { get; set; } = new();
    }
}