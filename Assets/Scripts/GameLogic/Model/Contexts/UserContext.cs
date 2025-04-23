using System;
using System.Collections.Generic;

namespace GameLogic.Model.Contexts
{
    [Serializable]
    public class UserContext 
    {
        public bool IsSoundsMuted { get; set; }
        public bool IsHowToPlayHintShown { get; set; }
        public string LocalizationDefId { get; set; }
        public Dictionary<string, LevelProgressContext> LevelsProgress { get; set; } = new();
    }
}