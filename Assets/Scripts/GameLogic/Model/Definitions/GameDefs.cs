using System;
using System.Collections.Generic;

namespace GameLogic.Model.Definitions
{
    [Serializable]
    public class GameDefs
    {
        public Dictionary<string, LocalizationDef> Localizations { get; set; }
        public Dictionary<string, ClusterDef> Clusters { get; set; }
        public Dictionary<string, LevelDef> Levels { get; set; }
    }
}