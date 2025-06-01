using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameLogic.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(RectTransform))]
    public abstract class View : MonoBehaviour
    {
        [Inject] protected ViewManager _viewManager;

        [SerializeField] private short _overrideSortingOrder = -1;

        private ViewModel _viewModelInternal;
        private RectTransform _rectTransform;
        private Canvas _canvas;


        public abstract UniTask Initialize(ViewModel viewModel);

        protected void UpdateViewModel<T>(ref T oldViewModel, ViewModel newViewModel) where T : ViewModel
        {
            oldViewModel?.Dispose();
            oldViewModel = (T) newViewModel;
            _viewModelInternal = newViewModel;

            Subscribes();
        }

        protected abstract void Subscribes();
        protected abstract void Unsubscribes();

        public void PrepareToShow()
        {
            _canvas ??= GetComponent<Canvas>();
            _rectTransform ??= GetComponent<RectTransform>();

            _viewManager.Views.Subscribe(OnViewsChanged);

            _rectTransform.anchoredPosition = Vector2.zero;
            _canvas.pixelPerfect = true;
            _canvas.overrideSorting = true;
        }

        private void OnViewsChanged(IReadOnlyList<View> views)
        {
            if (_overrideSortingOrder > 0)
            {
                _canvas.sortingOrder = _overrideSortingOrder;
                return;
            }

            for (var index = 0; index < views.Count; index++)
            {
                var view = views[index];
                if (view == this)
                {
                    _canvas.sortingOrder = index;
                    break;
                }
            }
        }

        public virtual UniTask AnimateShowing()
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask AnimateClosing()
        {
            return UniTask.CompletedTask;
        }

        [ContextMenu("Close")]
        public async void Close(bool force = false)
        {
            await _viewManager.Close(this);
        }

        private void OnDestroy()
        {
            _viewManager.Views.Unsubscribe(OnViewsChanged);

            Unsubscribes();
            _viewModelInternal?.Dispose();
        }
    }

}