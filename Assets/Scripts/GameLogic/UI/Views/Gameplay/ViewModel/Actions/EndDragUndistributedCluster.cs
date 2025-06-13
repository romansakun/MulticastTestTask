namespace GameLogic.UI.Gameplay
{
    public class EndDragUndistributedCluster : BaseEndDragCluster
    {
        public override void Execute(GameplayViewModelContext context)
        {
            if (context.Swipe.HintClusterWordRow != null)
            {
                SetHintClusterAsDistributed(context);
                DisposeOriginUndistributedCluster(context);
            }
            else
            {
                ReturnOriginClusterState(context);
            }

            StopWordRowsBlinking(context);
            DisposeDraggedCluster(context);
            ResetAfterDrag(context);

            context.IsUndistributedClustersScrollRectActive.SetValueAndForceNotify(true);
            _audioPlayer.PlaySound("DropClusterSound");
        }

    }
}