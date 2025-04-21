using System.Collections.Generic;
using GameLogic.Model.Contexts;

namespace GameLogic.Model.DataProviders
{
    public class LevelProgressContextDataProvider
    {
        public string LevelDefId { get; }
        public bool IsCompleted { get; }
        public IReadOnlyList<string> UndistributedClusters { get; }
        public IReadOnlyList<IReadOnlyList<string>> DistributedClusters { get; }

        public LevelProgressContextDataProvider(LevelProgressContext levelProgress)
        {
            LevelDefId = levelProgress.LevelDefId;
            IsCompleted = levelProgress.IsCompleted;
            UndistributedClusters = levelProgress.UndistributedClusters;
            DistributedClusters = levelProgress.DistributedClusters;
        }

    }
}