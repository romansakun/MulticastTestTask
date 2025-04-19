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
        [Inject] private AssetLoader _assetLoader;
        [Inject] private Canvas _canvas;

        private List<View> _instancedViews;
        private RectTransform _canvasRectTransform;

        public void Initialize()
        {
            _instancedViews = new List<View>();
            _canvasRectTransform = _canvas.GetComponent<RectTransform>();
        }

        public async UniTask AddView<V, VM>(V view, VM viewModel) where V : View where VM : ViewModel
        {
            await view.Initialize(viewModel);
            _instancedViews.Add(view);
        }

        public async UniTask<V> ShowAsync<V, VM> (VM viewModel) where V : View where VM : ViewModel
        {
            var viewPrefab = await _assetLoader.LoadPrefabAsync<V>();
            var view = _diContainer.InstantiatePrefabForComponent<V>(viewPrefab, _canvasRectTransform);

            PrepareToShow(view);
            await view.Initialize(viewModel);
            _instancedViews.Add(view);
            return view;
        }

        private void PrepareToShow<T>(T viewInstance) where T : View
        {
            var viewCanvas = viewInstance.GetComponent<Canvas>();
            var viewRectTransform = viewInstance.GetComponent<RectTransform>();
            viewRectTransform.anchoredPosition = Vector2.zero;
            viewCanvas.pixelPerfect = true;
            viewCanvas.overrideSorting = true;
            viewCanvas.sortingOrder = viewInstance.OverrideSortingOrder > 0 
                ? viewInstance.OverrideSortingOrder 
                : _instancedViews.Count;
        }

        public bool TryGetView<T>(out T view) where T : View
        {
            view = null;
            for (int i = _instancedViews.Count - 1; i >= 0; i--)
            {
                var viewInstance = _instancedViews[i];
                if (viewInstance is T needView)
                {
                    view = needView;
                    return true;
                }
            }
            return false;
        }

        public void Close<T>(bool onlyTopView = false) where T : View
        {
            for (int i = _instancedViews.Count - 1; i >= 0; i--)
            {
                var viewInstance = _instancedViews[i];
                if (viewInstance is not T)
                    continue;

                CloseView(viewInstance);
                if (onlyTopView) 
                    break;
            }
        }

        public void Close(View view, bool onlyTopView = false)
        {
            for (int i = _instancedViews.Count - 1; i >= 0; i--)
            {
                var viewInstance = _instancedViews[i];
                if (viewInstance != view) 
                    continue;

                CloseView(viewInstance);
                if (onlyTopView)
                    break;
            }
        }

        public void CloseAll()
        {
            for (int i = _instancedViews.Count - 1; i >= 0; i--)
            {
                var viewInstance = _instancedViews[i];
                if (viewInstance == null) 
                    continue;

                CloseView(viewInstance);
            }
        }

        private void CloseView(View view)
        {
            _instancedViews.Remove(view);
            Object.Destroy(view.gameObject);
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
            CloseAll();
        }

    }
}