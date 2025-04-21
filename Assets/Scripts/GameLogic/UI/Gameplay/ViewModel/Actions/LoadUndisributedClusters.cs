using Cysharp.Threading.Tasks;
using GameLogic.Bootstrapper;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class LoadUndistributedClusters : BaseGameplayViewModelAction
    {
        [Inject] private ColorsSettings _colorsSettings;
        [Inject] private Cluster.Factory _clusterFactory;

        public override async UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            var levelProgress = context.LevelProgress;
            for (var i = 0; i < levelProgress.UndistributedClusters.Count; i++)
            {
                var clusterText = levelProgress.UndistributedClusters[i];

                // var supportCluster = _clusterFactory.Create();
                // supportCluster.SetText(clusterText);
                // supportCluster.SetParent(context.UndistributedSupportClustersHolder);
                // supportCluster.SetBackgroundColor(_colorsSettings.SelectedClusterBackColor);
                // supportCluster.SetTextColor(Color.clear);
                // context.AllSupportClusters.Add(supportCluster);

                var cluster = _clusterFactory.Create();
                cluster.SetText(clusterText);
                cluster.SetParent(context.UndistributedClustersHolder);
                cluster.SetBackgroundColor(_colorsSettings.DefaultClusterBackColor);
                cluster.SetTextColor(_colorsSettings.DefaultClusterTextColor);
                context.UndistributedClusters.Add(cluster);

                //context.AllClusters.Add(supportCluster);
                context.AllClusters.Add(cluster);
            }
            await UniTask.Yield();
        }

    }
}