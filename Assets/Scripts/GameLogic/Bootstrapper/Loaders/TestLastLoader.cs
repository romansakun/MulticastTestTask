using Cysharp.Threading.Tasks;
using GameLogic.Factories;
using GameLogic.UI;
using GameLogic.UI.Gameplay;
using GameLogic.UI.Victory;
using Infrastructure;
using UnityEngine;
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
            // var viewModel = _viewModelFactory.Create<GameplayViewModel>();
            // var view = await _viewManager.ShowAsync<GameplayView, GameplayViewModel>(viewModel);  
            
            
            var viewModel = _viewModelFactory.Create<VictoryViewModel>();
            var view = await _viewManager.ShowAsync<VictoryView, VictoryViewModel>(viewModel);
        }
    }
}