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
        [SerializeField] private Button _checkWordsButton;

        private bool _isScrollRectDragging;
        private GameplayViewModel _viewModel;

        public override async UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);

            await _viewModel.StartLevelLoading(_wordsHolder, _undistributedClustersHolder);
        }

        protected override void Subscribes()
        {
            _checkWordsButton.onClick.AddListener(() => _viewModel.OnCheckWordsButtonClicked());
            _viewModel.IsUndistributedClustersScrollRectActive.Subscribe(OnUndistributedClustersScrollRectActiveChanged);
            _viewModel.IsHintClusterInUndistributedClusters.Subscribe(OnHintClusterInUndistributedClustersChanged);
        }

        protected override void Unsubscribes()
        {
            _checkWordsButton.onClick.RemoveAllListeners();
            _viewModel.IsUndistributedClustersScrollRectActive.Unsubscribe(OnUndistributedClustersScrollRectActiveChanged);
            _viewModel.IsHintClusterInUndistributedClusters.Unsubscribe(OnHintClusterInUndistributedClustersChanged);
        }

        private void OnUndistributedClustersScrollRectActiveChanged(bool state)
        {
            _undistributedClustersScrollRect.enabled = state;
        }

        private void OnHintClusterInUndistributedClustersChanged(bool state)
        {
            if (state) _undistributedClustersScrollRect.horizontalNormalizedPosition = 0f;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _viewModel.OnPointerClick(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_scrollRectSwipeArea.IsContainsPoint(eventData.position))
            {
                _isScrollRectDragging = true;
                _undistributedClustersScrollRect.OnBeginDrag(eventData);
            }
            _viewModel.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isScrollRectDragging)
            {
                _undistributedClustersScrollRect.OnDrag(eventData);
            }
            _viewModel.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isScrollRectDragging)
            {
                _undistributedClustersScrollRect.OnEndDrag(eventData);
                _isScrollRectDragging = false;
            }
            _viewModel.OnEndDrag(eventData);
        }

    }
}