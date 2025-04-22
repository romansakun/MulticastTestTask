using UnityEngine;

namespace GameLogic.UI.Gameplay
{
    public class OnBeginDragUndistributedCluster : BaseGameplayViewModelQualifier
    {
        public override float Score(GameplayViewModelContext context)
        {
            if (context.Input.Type != UserInputType.OnBeginDrag)
                return 0;

            if (context.UndistributedClustersHolder.IsContainsScreenPoint(context.Input.Data.position) == false)
                return 0;

            var cluster = context.UndistributedClusters.Find(c => c.IsContainsScreenPoint(context.Input.Data.position));
            if (cluster == null)
                return 0;

            context.OriginDraggedCluster = cluster;
            context.OriginDraggedClusterHolder = context.UndistributedClustersHolder;
            context.OriginDraggedClusterWordRow = null;

            context.Input = default;

            return 1;
        }
    }
}