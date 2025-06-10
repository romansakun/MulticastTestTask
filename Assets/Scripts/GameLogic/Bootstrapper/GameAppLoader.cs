using System;
using System.Collections.Generic;
using GameLogic.Factories;
using GameLogic.UI;
using GameLogic.UI.GameAppLoader;
using Infrastructure.Extensions;
using UnityEngine.Device;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class GameAppLoader : IInitializable, IDisposable
    {
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;

        public void Initialize()
        {
            Application.targetFrameRate = 60;

            _viewManager.Views.Subscribe(OnViewsChanged);
        }

        private async void OnViewsChanged(IReadOnlyList<View> views)
        {
            var gameAppLoadedView = views.Find(view => view is GameAppLoaderView);
            if (gameAppLoadedView == null)
                return;

            _viewManager.Views.Unsubscribe(OnViewsChanged);

            var viewModel = _viewModelFactory.Create<GameAppLoaderViewModel>();
            await gameAppLoadedView.Initialize(viewModel);

            //viewModel.AddToLoadingQueue<YandexAuthLoader>();
            viewModel.AddToLoadingQueue<UserContextLoader>();
            viewModel.AddToLoadingQueue<MusicLoading>();
            viewModel.AddToLoadingQueue<RatingLoading>();
            viewModel.AddToLoadingQueue<InitDeferredMonoPoolsLoader>();
            viewModel.AddToLoadingQueue<ShowMainMenuLoader>();
            //viewModel.AddToLoadingQueue<YandexGraLoading>();

            viewModel.ProcessLoadingQueue();
        }

        public void Dispose()
        {
            _viewManager.Views.Unsubscribe(OnViewsChanged);
        }
    }
}