using Cysharp.Threading.Tasks;
using Factories;
using GameLogic.Model.Actions;
using Infrastructure;
using Infrastructure.GameActions;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class LastLoader : IAsyncOperation
    {
        [Inject] private GameActionExecutor _gameActionExecutor;
        [Inject] private GameActionFactory _gameActionFactory;

        public UniTask ProcessAsync()
        {
            var gameAction = _gameActionFactory.Create<StartLevelGameAction>();
            _gameActionExecutor.Execute(gameAction);

            return UniTask.CompletedTask;
        }

    }
}