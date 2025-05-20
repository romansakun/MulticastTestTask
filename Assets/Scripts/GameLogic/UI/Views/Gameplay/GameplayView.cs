using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameLogic.Audio;
using GameLogic.Bootstrapper;
using GameLogic.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class GameplayView : View, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private SoundsSettings _soundsSettings;
        
        [SerializeField] private RectTransform _scrollRectSwipeArea;
        [SerializeField] private ScrollRect _undistributedClustersScrollRect;
        [SerializeField] private RectTransform _undistributedClustersHolder;
        [SerializeField] private RectTransform _wordsHolder;
        [SerializeField] private Button _swipeToLeftButton;
        [SerializeField] private Button _swipeToRightButton;
        [SerializeField] private TextMeshProUGUI _levelName;
        [SerializeField] private TextMeshProUGUI _description;

        [Header("Ad Tip button")]
        [SerializeField] private Button _adTipButton;
        [SerializeField] private GameObject _adTipButtonIcon;
        [SerializeField] private GameObject _adTipButtonAdsImage;
        [SerializeField] private CircleCountText _adTipButtonCountText;

        [Header("Check words button")]
        [SerializeField] private Button _checkWordsButton;
        [SerializeField] private TextMeshProUGUI _checkWordsButtonLabel;
        [SerializeField] private CircleCountText _checkWordsButtonCountText;
        [SerializeField] private GameObject _checkWordsButtonAdsImage;
        [SerializeField] private List<GameObject> _checkingWordsButtonEmpties;
   

        private bool _isScrollRectDragging;
        private Tween _failButtonAnimation;
        private GameplayViewModel _viewModel;

        public override async UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);

            await _viewModel.StartLevelLoading(_wordsHolder, _undistributedClustersHolder);
        }

        protected override void Subscribes()
        {
            _checkWordsButton.onClick.AddListener(() => _viewModel.OnCheckWordsButtonClicked());
            _swipeToLeftButton.onClick.AddListener(OnSwipeToLeftButtonClicked);
            _swipeToRightButton.onClick.AddListener(OnSwipeToRightButtonClicked);
            _adTipButton.onClick.AddListener(OnAdTipButtonClicked);
            _viewModel.UndistributedClustersScrollRectNormalizedPosition.Subscribe(OnUndistributedClustersScrollRectNormalizedPositionChanged);
            _viewModel.IsUndistributedClustersScrollRectActive.Subscribe(OnUndistributedClustersScrollRectActiveChanged);
            _viewModel.IsHintClusterInUndistributedClusters.Subscribe(OnHintClusterInUndistributedClustersChanged);
            _viewModel.IsFailedCompleteLevel.Subscribe(OnFailedCompleteLevelChanged);
            _viewModel.DescriptionLevelText.Subscribe(OnDescriptionLevelTextChanged);
            _viewModel.LevelNameText.Subscribe(OnLevelNameTextChanged);
            _viewModel.CheckingWordsButtonState.Subscribe(OnCheckingWordsButtonStateChanged);
            _viewModel.TipButtonState.Subscribe(OnTipButtonStateChanged);
            _viewModel.IsTipVisible.Subscribe(OnTipVisibleChanged);
        }

        protected override void Unsubscribes()
        {
            _checkWordsButton.onClick.RemoveAllListeners();
            _swipeToLeftButton.onClick.RemoveAllListeners();
            _swipeToRightButton.onClick.RemoveAllListeners();
            _adTipButton.onClick.RemoveAllListeners();
            _viewModel.UndistributedClustersScrollRectNormalizedPosition.Unsubscribe(OnUndistributedClustersScrollRectNormalizedPositionChanged);
            _viewModel.IsUndistributedClustersScrollRectActive.Unsubscribe(OnUndistributedClustersScrollRectActiveChanged);
            _viewModel.IsHintClusterInUndistributedClusters.Unsubscribe(OnHintClusterInUndistributedClustersChanged);
            _viewModel.IsFailedCompleteLevel.Unsubscribe(OnFailedCompleteLevelChanged);
            _viewModel.DescriptionLevelText.Unsubscribe(OnDescriptionLevelTextChanged);
            _viewModel.LevelNameText.Unsubscribe(OnLevelNameTextChanged);
            _viewModel.CheckingWordsButtonState.Unsubscribe(OnCheckingWordsButtonStateChanged);
            _viewModel.TipButtonState.Unsubscribe(OnTipButtonStateChanged);
            _viewModel.IsTipVisible.Unsubscribe(OnTipVisibleChanged);

            _failButtonAnimation?.Kill();
        }

        private void OnCheckingWordsButtonStateChanged(ConsumableButtonState state)
        {
            _checkWordsButtonLabel.gameObject.SetActive(state.IsShowConsumable);
            _checkWordsButtonCountText.gameObject.SetActive(state.IsShowCount);
            _checkWordsButtonAdsImage.gameObject.SetActive(state.IsShowAdsIcon);
            _checkWordsButtonCountText.SetCount(state.Count, state.IsShowPlusCount);
            _checkingWordsButtonEmpties.ForEach(go => go.SetActive(state.IsShowAdsIcon));
        }

        private void OnTipButtonStateChanged(ConsumableButtonState state)
        {
            _adTipButtonIcon.gameObject.SetActive(state.IsShowConsumable);
            _adTipButtonCountText.gameObject.SetActive(state.IsShowCount);
            _adTipButtonAdsImage.gameObject.SetActive(state.IsShowAdsIcon);
            _adTipButtonCountText.SetCount(state.Count, state.IsShowPlusCount);
        }

        private void OnUndistributedClustersScrollRectNormalizedPositionChanged(float value)
        {
            _undistributedClustersScrollRect.horizontalNormalizedPosition = value;
        }

        private void OnLevelNameTextChanged(string levelName)
        {
            _levelName.text = levelName;
        }

        private void OnDescriptionLevelTextChanged(string rules)
        {
            _description.text = rules;
        }

        private void OnUndistributedClustersScrollRectActiveChanged(bool state)
        {
            _undistributedClustersScrollRect.enabled = state;
        }

        private void OnTipVisibleChanged(bool state)
        {
            _adTipButton.gameObject.SetActive(state);
        }

        private void OnHintClusterInUndistributedClustersChanged(bool state)
        {
            if (state) _undistributedClustersScrollRect.horizontalNormalizedPosition = 0f;
        }


        private void OnSwipeToRightButtonClicked()
        {
            _undistributedClustersScrollRect.horizontalNormalizedPosition = 1f;
        }

        private void OnAdTipButtonClicked()
        {
            _viewModel.OnAdTipButtonClicked();
        }

        private void OnSwipeToLeftButtonClicked()
        {
            _undistributedClustersScrollRect.horizontalNormalizedPosition = 0f;
        }

        private void OnFailedCompleteLevelChanged(bool state)
        {
            if (!state) return;

            _audioPlayer.PlaySound(_soundsSettings.WrongAnswerSound);

            _failButtonAnimation?.Kill();
            _checkWordsButton.interactable = false;
            _failButtonAnimation = _checkWordsButton.transform.DOShakePosition(.75f, new Vector2(15, 0));
            _failButtonAnimation.OnComplete(() =>
            {
                _checkWordsButton.interactable = true;
            });
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _viewModel.OnPointerClick(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_scrollRectSwipeArea.IsContainsScreenPoint(eventData.position))
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