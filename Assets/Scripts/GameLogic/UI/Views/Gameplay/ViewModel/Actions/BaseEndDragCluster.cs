using GameLogic.Audio;
using GameLogic.Bootstrapper;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public abstract class BaseEndDragCluster : BaseGameplayViewModelAction
    {
        [Inject] protected AudioPlayer _audioPlayer;
        [Inject] private ColorsSettings _colorsSettings;

        protected void SetHintClusterAsDistributed(GameplayViewModelContext context)
        {
            var cluster = context.Swipe.HintCluster;
            cluster.SetColorAlpha(1);

            context.WordRowsClusters[context.Swipe.HintClusterWordRow].Add(cluster);
            context.DistributedClusters.Add(cluster);
        }

        protected void SetHintClusterAsUndistributed(GameplayViewModelContext context)
        {
            var cluster = context.Swipe.HintCluster;
            cluster.SetColorAlpha(1);

            context.UndistributedClusters.Add(cluster);
        }

        protected void ReturnOriginClusterState(GameplayViewModelContext context)
        {
            var original = context.Swipe.OriginDraggedCluster;
            original.SetColorAlpha(1);
        }

        protected void DisposeOriginUndistributedCluster(GameplayViewModelContext context)
        {
            context.UndistributedClusters.Remove(context.Swipe.OriginDraggedCluster);
            context.AllClusters.Remove(context.Swipe.OriginDraggedCluster);
            context.Swipe.OriginDraggedCluster.Dispose();
        }
        
        protected void DisposeOriginWordRowCluster(GameplayViewModelContext context)
        {
            context.AllClusters.Remove(context.Swipe.OriginDraggedCluster);
            context.DistributedClusters.Remove(context.Swipe.OriginDraggedCluster);
            context.WordRowsClusters[context.Swipe.OriginDraggedClusterWordRow].Remove(context.Swipe.OriginDraggedCluster);
            context.Swipe.OriginDraggedCluster.Dispose();
        }

        protected void DisposeDraggedCluster(GameplayViewModelContext context)
        {
            context.Swipe.DraggedCluster.Dispose();
        }

        protected void ResetAfterDrag(GameplayViewModelContext context)
        {
            context.Swipe.DraggedCluster = null;
            context.Swipe.OriginDraggedCluster = null;
            context.Swipe.OriginDraggedClusterWordRow = null;
            context.Swipe.OriginDraggedClusterHolder = null;
            context.Swipe.HintCluster = null;
            context.Swipe.HintClusterWordRow = null;
            context.Swipe.HintClusterHolder = null;

            context.Swipe.IsSwipeInputNow = false;
        }

        protected void StopWordRowsBlinking(GameplayViewModelContext context)
        {
            foreach (var wordRow in context.WordRows)
            {
                wordRow.SetEnabledBlinking(false);
            }
        }

    }
}