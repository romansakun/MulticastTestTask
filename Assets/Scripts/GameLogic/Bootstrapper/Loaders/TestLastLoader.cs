using Cysharp.Threading.Tasks;
using Factories;
using GameLogic.UI;
using GameLogic.UI.Gameplay;
using Infrastructure;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class TestLastLoader : IAsyncOperation
    {
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;

        public async UniTask ProcessAsync()
        {
            await ShowGameplayView();
        }

        private async UniTask ShowGameplayView()
        {
            var viewModel = _viewModelFactory.Create<GameplayViewModel>();
            var view = await _viewManager.ShowAsync<GameplayView, GameplayViewModel>(viewModel);
        }
    }
}