using System;
using System.Collections.Generic;

namespace GameLogic.Model.Contexts
{
    [Serializable]
    public class LevelProgressContext
    {
        public bool IsCompleted { get; set; }
        public List<string> UndistributedClusters { get; set; } = new();
        public List<List<string>> DistributedClusters { get; set; } = new();
    }
}