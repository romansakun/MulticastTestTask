using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameLogic.UI.Gameplay
{
    public class GameplayView : View, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform _scrollRectSwipeArea;
        [SerializeField] private ScrollRect _undistributedClustersScrollRect;
        [SerializeField] private RectTransform _undistributedClustersHolder;
        [SerializeField] private RectTransform _wordsHolder;

        private bool _isScrollRectDragging;
        private GameplayViewModel _viewModel;
        
        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);

            _viewModel.LoadClusters(_undistributedClustersHolder, _wordsHolder);
            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
            
        }

        protected override void Unsubscribes()
        {
           
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _viewModel.OnPointerClick(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (CheckScrollRectArea(eventData))
            {
                _isScrollRectDragging = true;
                _undistributedClustersScrollRect.OnBeginDrag(eventData);
            }
            _viewModel.OnPointerBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isScrollRectDragging)
            {
                _undistributedClustersScrollRect.OnDrag(eventData);
            }
            _viewModel.OnPointerDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isScrollRectDragging)
            {
                _undistributedClustersScrollRect.OnEndDrag(eventData);
                _isScrollRectDragging = false;
            }
            _viewModel.OnPointerEndDrag(eventData);
        }

        private bool CheckScrollRectArea(PointerEventData eventData)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(_scrollRectSwipeArea, eventData.position, null, Vector4.zero);
        }

    }
}