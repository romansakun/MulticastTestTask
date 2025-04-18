using System;
using System.Collections.Generic;

namespace GameLogic.Model.Definitions
{
    [Serializable]
    public class LocalizationDef : BaseDef 
    {
        public string Description { get; set; }
        public Dictionary<int, string> Levels = new(); 
        public Dictionary<string, string> LocalizationText = new();
    }
}