using System;
using Infrastructure.GameActions;
using Zenject;

namespace Factories
{
    public class GameActionFactory
    {
        [Inject] private DiContainer _diContainer;

        public T Create<T>() where T : IGameAction
        {
            var gameAction = _diContainer.Instantiate<T>(Array.Empty<object>());
            return gameAction;
        }
    }
}