using GameLogic.Bootstrapper;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class EndDragUndistributedCluster : BaseGameplayViewModelAction
    {
        [Inject] private ColorsSettings _colorsSettings;

        public override void Execute(GameplayViewModelContext context)
        {
            context.IsUndistributedClustersScrollRectActive.Value = true;

            StopWordRowsBlinking(context);

            if (context.HintCluster != null)
            {
                SetHintClusterAsDistributed(context);
                DisposeOriginUndistributedCluster(context);
            }
            else
            {
                ReturnOriginClusterState(context);
            }

            context.DraggedCluster.Dispose();
            context.DraggedCluster = null;
            context.OriginDraggedCluster = null;
            context.OriginDraggedClusterWordRow = null;
            context.OriginDraggedClusterHolder = null;
        }

        private void SetHintClusterAsDistributed(GameplayViewModelContext context)
        {
            var cluster = context.HintCluster;
            cluster.SetBackgroundColor(_colorsSettings.DefaultClusterBackColor);
            cluster.SetTextColor(_colorsSettings.DefaultClusterTextColor);
            context.WordRowsClusters[context.HintClusterWordRow].Add(cluster);
            context.DistributedClusters.Add(cluster);
            context.AllClusters.Add(cluster);

            context.HintCluster = null;
            context.HintClusterWordRow = null;
        }

        private static void DisposeOriginUndistributedCluster(GameplayViewModelContext context)
        {
            context.UndistributedClusters.Remove(context.OriginDraggedCluster);
            context.AllClusters.Remove(context.OriginDraggedCluster);
            context.OriginDraggedCluster.Dispose();
        }

        private void ReturnOriginClusterState(GameplayViewModelContext context)
        {
            var original = context.OriginDraggedCluster;
            original.SetBackgroundColor(_colorsSettings.DefaultClusterBackColor);
            original.SetTextColor(_colorsSettings.DefaultClusterTextColor);
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