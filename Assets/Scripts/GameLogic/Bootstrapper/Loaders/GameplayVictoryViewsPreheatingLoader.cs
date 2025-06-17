using Cysharp.Threading.Tasks;
using GameLogic.UI;
using GameLogic.UI.Gameplay;
using GameLogic.UI.Victory;
using Infrastructure;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class GameplayVictoryViewsPreheatingLoader : IAsyncOperation
    {
        [Inject] private ViewManager _viewManager;

        public async UniTask ProcessAsync()
        {
            var gameplayView = await _viewManager.ShowAsync<GameplayView, ViewModel>(null);
            var victoryView = await _viewManager.ShowAsync<VictoryView, ViewModel>(null);
            await _viewManager.Close(gameplayView);
            await _viewManager.Close(victoryView);
        }
    }
}