using Infrastructure.GameActions;
using Zenject;

namespace GameLogic.Factories
{
    public class GameActionFactory
    {
        [Inject] private DiContainer _diContainer;

        public T Create<T>(params object[] extraArgs) where T : IGameAction
        {
            var gameAction = _diContainer.Instantiate<T>(extraArgs);
            return gameAction;
        }
    }
}