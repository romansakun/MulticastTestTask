namespace GameLogic.UI.Gameplay
{
    public class OnDragUndistributedCluster : BaseGameplayViewModelQualifier
    {
        public override float Score(GameplayViewModelContext context)
        {
            if (context.Input.Type != UserInputType.OnDrag)
                return 0;

            if (context.DraggedCluster == null)
                return 0;

            if (context.UndistributedClusters.Contains(context.OriginDraggedCluster) == false)
                return 0;

            context.DraggedCluster.SetPosition(context.Input.Data.position);

            return 1;
        }
    }
}