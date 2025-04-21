using GameLogic.Bootstrapper;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class EndDragDistributedCluster : BaseGameplayViewModelAction
    {
        [Inject] private ColorsSettings _colorsSettings;

        public override void Execute(GameplayViewModelContext context)
        {
            StopWordRowsBlinking(context);

            if (context.HintClusterWordRow != null)
            {
                SetHintClusterAsDistributed(context);
            }
            else
            {
                SetHintClusterAsUndistributed(context);
            }

            DisposeOriginWordRowCluster(context);

            context.DraggedCluster.Dispose();
            context.DraggedCluster = null;
            context.OriginDraggedCluster = null;
            context.OriginDraggedClusterWordRow = null;
            context.OriginDraggedClusterHolder = null;
            context.HintCluster = null;
            context.HintClusterWordRow = null;
        }

        private void SetHintClusterAsDistributed(GameplayViewModelContext context)
        {
            var cluster = context.HintCluster;
            cluster.SetBackgroundColor(_colorsSettings.DefaultClusterBackColor);
            cluster.SetTextColor(_colorsSettings.DefaultClusterTextColor);
            context.WordRowsClusters[context.HintClusterWordRow].Add(cluster);
            context.DistributedClusters.Add(cluster);
            context.AllClusters.Add(cluster);

        }

        private void SetHintClusterAsUndistributed(GameplayViewModelContext context)
        {
            var cluster = context.HintCluster;
            cluster.SetBackgroundColor(_colorsSettings.DefaultClusterBackColor);
            cluster.SetTextColor(_colorsSettings.DefaultClusterTextColor);
            context.UndistributedClusters.Add(cluster);
            context.AllClusters.Add(cluster);
        }

        private void DisposeOriginWordRowCluster(GameplayViewModelContext context)
        {
            context.AllClusters.Remove(context.OriginDraggedCluster);
            context.DistributedClusters.Remove(context.OriginDraggedCluster);
            context.WordRowsClusters[context.OriginDraggedClusterWordRow].Remove(context.OriginDraggedCluster);
            context.OriginDraggedClusterWordRow = null;
            context.OriginDraggedCluster.Dispose();
        }

        private void StopWordRowsBlinking(GameplayViewModelContext context)
        {
            foreach (var wordRow in context.WordRows)
            {
                wordRow.SetEnabledBlinking(false);
            }
        }

    }
}