using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Infrastructure;
using Infrastructure.Services;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace GameLogic.UI
{
    public class ViewManager : IInitializable, IDisposable
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private IAssetsLoader _assetsLoader;
        [Inject] private Canvas _canvas;

        private RectTransform _canvasRectTransform;

        public IReactiveProperty<IReadOnlyList<View>> Views => _instancedViews;
        private readonly ReactiveProperty<List<View>> _instancedViews = new(new List<View>());

        public void Initialize()
        {
            _canvasRectTransform = _canvas.GetComponent<RectTransform>();
        }

        public async UniTask AddView<V, VM>(V view, VM viewModel) where V : View where VM : ViewModel
        {
            if (viewModel != null)
            {
                await view.Initialize(viewModel);
            }
            _instancedViews.Value.Add(view);
            _instancedViews.ForceNotify();
        }

        public async UniTask<V> ShowAsync<V, VM> (VM viewModel, bool force = false) where V : View where VM : ViewModel
        {
            var viewPrefab = await _assetsLoader.LoadPrefabAsync<V>();
            var view = _diContainer.InstantiatePrefabForComponent<V>(viewPrefab, _canvasRectTransform);
            view.PrepareToShow();

            var initializingTask = view.Initialize(viewModel);
            if (force == false)
            {
                await UniTask.WhenAll(initializingTask, view.AnimateShowing());
            }
            else
            {
                await initializingTask;
            }

            _instancedViews.Value.Add(view);
            _instancedViews.ForceNotify();

            return view;
        }

        public bool TryGetView<T>(out T view) where T : View
        {
            view = null;
            for (int i = _instancedViews.Value.Count - 1; i >= 0; i--)
            {
                var viewInstance = _instancedViews.Value[i];
                if (viewInstance is T needView)
                {
                    view = needView;
                    return true;
                }
            }
            return false;
        }

        public async UniTask Close<T>(bool onlyTopView = false, bool force = true) where T : View
        {
            for (int i = _instancedViews.Value.Count - 1; i >= 0; i--)
            {
                var viewInstance = _instancedViews.Value[i];
                if (viewInstance is not T)
                    continue;

                await CloseView(viewInstance, force);
                if (onlyTopView) 
                    break;
            }
        }

        public async UniTask Close(View view, bool onlyTopView = false, bool force = true)
        {
            for (int i = _instancedViews.Value.Count - 1; i >= 0; i--)
            {
                var viewInstance = _instancedViews.Value[i];
                if (viewInstance != view) 
                    continue;

                await CloseView(viewInstance, force);
                if (onlyTopView)
                    break;
            }
        }

        public async void CloseAll()
        {
            if (_instancedViews.Value == null)
                return;

            for (int i = _instancedViews.Value.Count - 1; i >= 0; i--)
            {
                var viewInstance = _instancedViews.Value[i];
                if (viewInstance == null) 
                    continue;

                await CloseView(viewInstance);
            }
        }

        private async UniTask CloseView(View view, bool force = true)
        {
            if (force == false)
            {
                await view.AnimateClosing();
            }
            _instancedViews.Value.Remove(view);
            Object.Destroy(view.gameObject);
            _instancedViews.ForceNotify();
        }

        public Vector2 ScreenPointToLocalPoint(Vector2 screenPoint)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRectTransform,
                screenPoint,
                null,
                out var localPoint
            );
            return localPoint;
        }

        public void Dispose()
        {
            _instancedViews.Dispose();
        }

    }
}