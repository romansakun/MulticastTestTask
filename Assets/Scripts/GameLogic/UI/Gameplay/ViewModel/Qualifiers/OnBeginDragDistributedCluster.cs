namespace GameLogic.UI.Gameplay
{
    public class OnBeginDragDistributedCluster : BaseGameplayViewModelQualifier
    {
        public override float Score(GameplayViewModelContext context)
        {
            if (context.Input.Type != UserInputType.OnBeginDrag)
                return 0;

            var pressPosition = context.Input.Data.pressPosition;
            var needWordRow = context.WordRows.Find(w => w.IsContainsScreenPoint(pressPosition));
            if (needWordRow == null)
                return 0;

            var cluster = context.WordRowsClusters[needWordRow].Find(c => c.IsContainsScreenPoint(pressPosition));
            if (cluster == null)
                return 0;

            context.OriginDraggedCluster = cluster;
            context.OriginDraggedClusterWordRow = needWordRow;
            context.OriginDraggedClusterHolder = needWordRow.ClustersHolder;

            context.Input = default;

            return 1;
        }
    }
}