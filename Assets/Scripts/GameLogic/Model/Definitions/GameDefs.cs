using System;
using System.Collections.Generic;

namespace GameLogic.Model.Definitions
{
    [Serializable]
    public class GameDefs
    {
        public DefaultSettingsDef DefaultSettings { get; set; }
        public LevelSettingsDef LevelSettings { get; set; }
        public Dictionary<string, LocalizationDef> Localizations = new();
        public Dictionary<string, LevelDef> Levels = new();
    }
}