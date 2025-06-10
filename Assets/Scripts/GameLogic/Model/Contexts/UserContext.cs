using System;
using System.Collections.Generic;

namespace GameLogic.Model.Contexts
{
    [Serializable]
    public class UserContext 
    {
        public int Version { get; set; }
        public bool IsSoundsMuted { get; set; }
        public bool IsMusicMuted { get; set; }
        public bool IsHowToPlayHintShown { get; set; }
        public string LocalizationDefId { get; set; }

        public ConsumablesContext Consumables { get; set; } = new();
        public Dictionary<string, LevelProgressContext> LevelsProgress { get; set; } = new();
    }
}