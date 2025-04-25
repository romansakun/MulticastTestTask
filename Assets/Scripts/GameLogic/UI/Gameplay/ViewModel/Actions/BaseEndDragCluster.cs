using GameLogic.Audio;
using GameLogic.Bootstrapper;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public abstract class BaseEndDragCluster : BaseGameplayViewModelAction
    {
        [Inject] protected AudioPlayer _audioPlayer;
        [Inject] protected SoundsSettings _soundsSettings;
        [Inject] private ColorsSettings _colorsSettings;

        protected void SetHintClusterAsDistributed(GameplayViewModelContext context)
        {
            var cluster = context.HintCluster;
            cluster.SetBackgroundColor(_colorsSettings.DefaultClusterBackColor);
            cluster.SetTextColor(_colorsSettings.DefaultClusterTextColor);

            context.WordRowsClusters[context.HintClusterWordRow].Add(cluster);
            context.DistributedClusters.Add(cluster);
            context.AllClusters.Add(cluster);
        }

        protected void SetHintClusterAsUndistributed(GameplayViewModelContext context)
        {
            var cluster = context.HintCluster;
            cluster.SetBackgroundColor(_colorsSettings.DefaultClusterBackColor);
            cluster.SetTextColor(_colorsSettings.DefaultClusterTextColor);

            context.UndistributedClusters.Add(cluster);
            context.AllClusters.Add(cluster);
        }

        protected void ReturnOriginClusterState(GameplayViewModelContext context)
        {
            var original = context.OriginDraggedCluster;
            original.SetBackgroundColor(_colorsSettings.DefaultClusterBackColor);
            original.SetTextColor(_colorsSettings.DefaultClusterTextColor);
        }

        protected void DisposeOriginUndistributedCluster(GameplayViewModelContext context)
        {
            context.UndistributedClusters.Remove(context.OriginDraggedCluster);
            context.AllClusters.Remove(context.OriginDraggedCluster);
            context.OriginDraggedCluster.Dispose();
        }
        
        protected void DisposeOriginWordRowCluster(GameplayViewModelContext context)
        {
            context.AllClusters.Remove(context.OriginDraggedCluster);
            context.DistributedClusters.Remove(context.OriginDraggedCluster);
            context.WordRowsClusters[context.OriginDraggedClusterWordRow].Remove(context.OriginDraggedCluster);
            context.OriginDraggedCluster.Dispose();
        }

        protected void DisposeDraggedCluster(GameplayViewModelContext context)
        {
            context.DraggedCluster.Dispose();
        }

        protected void ResetAfterDrag(GameplayViewModelContext context)
        {
            context.DraggedCluster = null;
            context.OriginDraggedCluster = null;
            context.OriginDraggedClusterWordRow = null;
            context.OriginDraggedClusterHolder = null;
            context.HintCluster = null;
            context.HintClusterWordRow = null;
            context.HintClusterHolder = null;
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