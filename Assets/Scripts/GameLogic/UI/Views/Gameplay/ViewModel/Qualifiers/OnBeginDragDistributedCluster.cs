namespace GameLogic.UI.Gameplay
{
    public class OnBeginDragDistributedCluster : BaseGameplayViewModelQualifier
    {
        public override float Score(GameplayViewModelContext context)
        {
            if (context.Input.Type != UserInputType.OnBeginDrag)
                return 0;

            if (context.Swipe.IsSwipeInputNow)
                return 0;

            var pressPosition = context.Input.Data.pressPosition;
            var needWordRow = context.WordRows.Find(w => w.IsContainsScreenPoint(pressPosition));
            if (needWordRow == null)
                return 0;

            var cluster = context.WordRowsClusters[needWordRow].Find(c => c.IsContainsScreenPoint(pressPosition));
            if (cluster == null)
                return 0;

            context.Swipe.OriginDraggedCluster = cluster;
            context.Swipe.OriginDraggedClusterWordRow = needWordRow;
            context.Swipe.OriginDraggedClusterHolder = needWordRow.ClustersHolder;

            context.Input = default;
            context.Swipe.IsSwipeInputNow = true;

            return 1;
        }
    }
}