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
        public short OverrideSortingOrder => _overrideSortingOrder;


        public abstract UniTask Initialize(ViewModel viewModel);

        protected void UpdateViewModel<T>(ref T oldViewModel, ViewModel newViewModel) where T : ViewModel
        {
            oldViewModel?.Dispose();
            oldViewModel = (T) newViewModel;
            _viewModelInternal = newViewModel;
        }

        [ContextMenu("Close")]
        public void Close()
        {
            _viewManager.Close(this);
        }

        protected virtual void OnDestroy()
        {
            _viewModelInternal?.Dispose();
        }
    }

}