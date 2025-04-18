using System;
using System.Collections.Generic;

namespace GameLogic.Model.Definitions
{
    [Serializable]
    public class GameDefs
    {
        public Dictionary<string, LocalizationDef> Localizations = new();
        public Dictionary<string, ClusterDef> Clusters = new();
        public Dictionary<string, LevelDef> Levels = new();
    }
}