namespace GameLogic.UI.Gameplay
{
    public class DragUndistributedCluster : BaseDragCluster
    {
        public override void Execute(GameplayViewModelContext context)
        {
            if (TryGetWordRowWithEmptyPlaceUnderDraggedCluster(context, false, out var wordRow))
            {
                SetHintClusterAsDistributed(context, wordRow, false);
            }
            else if (context.Swipe.HintCluster != null)
            {
                context.AllClusters.Remove(context.Swipe.HintCluster);
                context.Swipe.HintCluster.Dispose();
                context.Swipe.HintCluster = null;
                context.Swipe.HintClusterWordRow = null;
                context.Swipe.HintClusterHolder = null;
            }
        }
    }
}