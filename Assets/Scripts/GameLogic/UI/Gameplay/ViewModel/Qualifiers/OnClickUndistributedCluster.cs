namespace GameLogic.UI.Gameplay
{
    public class OnClickUndistributedCluster : BaseGameplayViewModelQualifier
    {
        public override float Score(GameplayViewModelContext context)
        {
            if (context.Input.Type != UserInputType.OnPointerClick)
                return 0;

            if (context.Click.IsClickInputNow)
                return 0;

            var position = context.Input.Data.position;
            if (context.UndistributedClustersHolder.IsContainsScreenPoint(position) == false)
                return 0;

            var cluster = context.UndistributedClusters.Find(c => c.IsContainsScreenPoint(position));
            if (cluster == null)
                return 0;

            context.Click.OriginUndistributedClickedCluster = cluster;
            context.Click.IsClickInputNow = true;
            context.Input = default;

            return 1;
        }
    }
}