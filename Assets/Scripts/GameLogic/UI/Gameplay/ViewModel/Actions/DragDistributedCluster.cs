namespace GameLogic.UI.Gameplay
{
    public class DragDistributedCluster : BaseDragCluster
    {
        public override void Execute(GameplayViewModelContext context)
        {
            if (TryGetWordRowWithEmptyPlaceUnderDraggedCluster(context, true, out var wordRow))
            {
                SetHintClusterAsDistributed(context, wordRow, true);
            }
            else
            {
                SetHintClusterAsUndistributed(context);
            }
        }
    }
}