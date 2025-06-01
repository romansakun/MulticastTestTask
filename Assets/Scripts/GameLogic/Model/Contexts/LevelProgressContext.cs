using System;
using System.Collections.Generic;

namespace GameLogic.Model.Contexts
{
    [Serializable]
    public class LevelProgressContext
    {
        public string LevelDefId { get; set; }
        public bool IsCompleted { get; set; }
        public int SavesCount { get; set; }
        public List<string> UndistributedClusters { get; set; } = new();
        public List<List<string>> DistributedClusters { get; set; } = new();
    }
}