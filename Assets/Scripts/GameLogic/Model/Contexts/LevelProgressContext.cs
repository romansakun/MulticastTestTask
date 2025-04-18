using System;
using System.Collections.Generic;

namespace GameLogic.Model.Contexts
{
    [Serializable]
    public class LevelProgressContext
    {
        public string LevelDefId { get; set; }
        public List<string> UndistributedClustersDefIds { get; set; } = new();
        public Dictionary<string, List<string>> DistributedClustersDefIds { get; set; } = new();
    }
}