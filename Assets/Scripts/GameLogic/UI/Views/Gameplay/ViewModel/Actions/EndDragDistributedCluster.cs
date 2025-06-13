namespace GameLogic.UI.Gameplay
{
    public class EndDragDistributedCluster : BaseEndDragCluster
    {
        public override void Execute(GameplayViewModelContext context)
        {
            if (context.Swipe.HintClusterWordRow != null)
            {
                SetHintClusterAsDistributed(context);
            }
            else
            {
                SetHintClusterAsUndistributed(context);
            }

            StopWordRowsBlinking(context);
            DisposeOriginWordRowCluster(context);
            DisposeDraggedCluster(context);
            ResetAfterDrag(context);

            _audioPlayer.PlaySound("DropClusterSound");
        }

    }
}