using Cysharp.Threading.Tasks;
using Factories;
using GameLogic.Model.Actions;
using GameLogic.UI;
using GameLogic.UI.Gameplay;
using Infrastructure;
using Infrastructure.GameActions;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class LastLoader : IAsyncOperation
    {
        [Inject] private GameActionExecutor _gameActionExecutor;
        [Inject] private GameActionFactory _gameActionFactory;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;

        public async UniTask ProcessAsync()
        {
            var gameAction = _gameActionFactory.Create<StartLevelGameAction>();
            _gameActionExecutor.Execute(gameAction);

            var viewModel = _viewModelFactory.Create<GameplayViewModel>();
            var view = await _viewManager.ShowAsync<GameplayView, GameplayViewModel>(viewModel);
        }

    }
}