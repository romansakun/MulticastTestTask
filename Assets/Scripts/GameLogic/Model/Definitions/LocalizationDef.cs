using System;
using System.Collections.Generic;

namespace GameLogic.Model.Definitions
{
    [Serializable]
    public class LocalizationDef : BaseDef 
    {
        public string Description = string.Empty;
        public List<string> TutorialVideos = new();
        public Dictionary<int, string> Levels = new(); 
        public Dictionary<int, string> Leagues = new(); 
        public Dictionary<string, string> LocalizationText = new();
    }
}