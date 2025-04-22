using System;
using System.Collections.Generic;
using GameLogic.Factories;
using GameLogic.UI;
using GameLogic.UI.GameAppLoader;
using Infrastructure.Extensions;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class GameAppLoader : IInitializable, IDisposable
    {
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;

        public void Initialize()
        {
            _viewManager.Views.Subscribe(OnViewsChanged);
        }

        private async void OnViewsChanged(IReadOnlyList<View> views)
        {
            var gameAppLoadedView = views.Find(view => view is GameAppLoaderView);
            if (gameAppLoadedView == null)
                return;

            var viewModel = _viewModelFactory.Create<GameAppLoaderViewModel>();
            await gameAppLoadedView.Initialize(viewModel);

            viewModel.AddToLoadingQueue<UnityRemoteConfigLoader>();
            viewModel.AddToLoadingQueue<UserContextLoader>();
            viewModel.AddToLoadingQueue<InitDynamicMonoPoolsLoader>();
            viewModel.AddToLoadingQueue<TestLastLoader>();

            viewModel.ProcessLoadingQueue();
        }

        public void Dispose()
        {
            _viewManager.Views.Unsubscribe(OnViewsChanged);
        }
    }
}