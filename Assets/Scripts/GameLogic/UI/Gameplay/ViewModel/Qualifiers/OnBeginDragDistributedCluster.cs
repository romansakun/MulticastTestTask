namespace GameLogic.UI.Gameplay
{
    public class OnBeginDragDistributedCluster : BaseGameplayViewModelQualifier
    {
        public override float Score(GameplayViewModelContext context)
        {
            if (context.Input.Type != UserInputType.OnBeginDrag)
                return 0;

            var needWordRow = context.WordRows.Find(w => w.IsContainsPoint(context.Input.Data.position));
            if (needWordRow == null)
                return 0;

            var cluster = context.WordRowsClusters[needWordRow].Find(c => c.IsContainsPoint(context.Input.Data.position));
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