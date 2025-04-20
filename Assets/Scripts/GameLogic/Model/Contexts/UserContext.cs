using System;
using System.Collections.Generic;

namespace GameLogic.Model.Contexts
{
    [Serializable]
    public class UserContext 
    {
        public string LocalizationDefId { get; set; }
        public Dictionary<string, LevelProgressContext> LevelsProgress { get; set; } = new();
    }
}