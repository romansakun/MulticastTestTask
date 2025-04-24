namespace GameLogic.UI.Gameplay
{
    public class DragUndistributedCluster : BaseDragCluster
    {
        public override void Execute(GameplayViewModelContext context)
        {
            if (TryGetWordRowUnderDraggedCluster(context, false, out var wordRow))
            {
                SetHintClusterAsDistributed(context, wordRow, false);
            }
            else if (context.HintCluster != null)
            {
                context.HintCluster.Dispose();
                context.HintCluster = null;
                context.HintClusterWordRow = null;
                context.HintClusterHolder = null;
            }
        }
    }
}