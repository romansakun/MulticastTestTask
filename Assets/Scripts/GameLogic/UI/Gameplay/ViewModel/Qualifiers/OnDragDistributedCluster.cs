using GameLogic.Bootstrapper;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class OnDragDistributedCluster : BaseGameplayViewModelQualifier
    {
        [Inject] private GameplaySettings _gameplaySettings;

        public override float Score(GameplayViewModelContext context)
        {
            if (context.Input.Type != UserInputType.OnDrag)
                return 0;

            if (context.DraggedCluster == null)
                return 0;

            if (context.DistributedClusters.Contains(context.OriginDraggedCluster) == false)
                return 0;

            var swipePosition = context.Input.Data.position + _gameplaySettings.DraggedClusterOffsetPosition;
            context.DraggedCluster.SetPosition(swipePosition);
            context.Input = default;

            return 1;
        }
    }
}