namespace GameLogic.UI.Gameplay
{
    public class OnBeginDragUndistributedCluster : BaseGameplayViewModelQualifier
    {
        public override float Score(GameplayViewModelContext context)
        {
            if (context.Input.Type != UserInputType.OnBeginDrag)
                return 0;

            if (context.Swipe.IsSwipeInputNow)
                return 0;

            var pressPosition = context.Input.Data.pressPosition;
            if (context.UndistributedClustersHolder.IsContainsScreenPoint(pressPosition) == false)
                return 0;

            var cluster = context.UndistributedClusters.Find(c => c.IsContainsScreenPoint(pressPosition));
            if (cluster == null)
                return 0;

            context.Swipe.OriginDraggedCluster = cluster;
            context.Swipe.OriginDraggedClusterHolder = context.UndistributedClustersHolder;
            context.Swipe.OriginDraggedClusterWordRow = null;

            context.Input = default;
            context.Swipe.IsSwipeInputNow = true;

            return 1;
        }
    }
}