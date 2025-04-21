using System;
using GameLogic.Factories;
using GameLogic.UI;
using GameLogic.UI.GameAppLoader;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class GameAppLoader : IInitializable, IDisposable
    {
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;

        public void Initialize()
        {
            _viewManager.CurrentView.Subscribe(OnCurrentViewChanged);
        }

        private async void OnCurrentViewChanged(View view)
        {
            if (view is not GameAppLoaderView)
                return;

            var viewModel = _viewModelFactory.Create<GameAppLoaderViewModel>();
            await view.Initialize(viewModel);

            viewModel.AddToLoadingQueue<UnityRemoteConfigLoader>();
            viewModel.AddToLoadingQueue<UserContextLoader>();
            viewModel.AddToLoadingQueue<BindingGameplayFactoriesLoader>();
            viewModel.AddToLoadingQueue<TestLastLoader>();

            viewModel.ProcessLoadingQueue();
        }

        public void Dispose()
        {
            _viewManager.CurrentView.Unsubscribe(OnCurrentViewChanged);
        }
    }
}