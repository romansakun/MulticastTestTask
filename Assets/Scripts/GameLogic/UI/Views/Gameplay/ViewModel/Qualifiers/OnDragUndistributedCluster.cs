using GameLogic.Bootstrapper;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class OnDragUndistributedCluster : BaseGameplayViewModelQualifier
    {
        [Inject] private GameplaySettings _gameplaySettings;

        public override float Score(GameplayViewModelContext context)
        {
            if (context.Input.Type != UserInputType.OnDrag)
                return 0;

            if (context.Swipe.DraggedCluster == null)
                return 0;

            if (context.UndistributedClusters.Contains(context.Swipe.OriginDraggedCluster) == false)
                return 0;

            var swipePosition = context.Input.Data.position + _gameplaySettings.ClusterOffsetPosition();
            context.Swipe.DraggedCluster.SetPosition(swipePosition);
            context.Input = default;

            return 1;
        }
    }
}