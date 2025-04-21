using Cysharp.Threading.Tasks;
using GameLogic.Bootstrapper;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class LoadWordDistributedClusters : BaseGameplayViewModelAction
    {
        [Inject] private ColorsSettings _colorsSettings;
        [Inject] private Cluster.Factory _clusterFactory;

        public override async UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            var levelProgress = context.LevelProgress;
            for (var i = 0; i < levelProgress.DistributedClusters.Count; i++)
            {
                var clustersRow = levelProgress.DistributedClusters[i];
                var wordRow = context.WordRows[i];
                foreach (var clusterText in clustersRow)
                {
                    var cluster = _clusterFactory.Create();
                    cluster.SetText(clusterText);
                    cluster.SetParent(wordRow.ClustersHolder);
                    cluster.SetBackgroundColor(_colorsSettings.DefaultClusterBackColor);
                    cluster.SetTextColor(_colorsSettings.DefaultClusterTextColor);

                    context.WordRowsClusters[wordRow].Add(cluster);
                    context.DistributedClusters.Add(cluster);
                    context.AllClusters.Add(cluster);
                }
            }
            await UniTask.Yield();
        }

    }
}