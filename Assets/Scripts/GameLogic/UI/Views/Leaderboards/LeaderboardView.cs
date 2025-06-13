using Cysharp.Threading.Tasks;
using GameLogic.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.Leaderboards
{
    public class LeaderboardView : View
    {
        [SerializeField] private ViewContentAnimations _animations;
        [SerializeField] private RectTransform _playersContainer;
        [SerializeField] private RectTransform _myPlayerContainer;
        [SerializeField] private TextMeshProUGUI _textGeminiGPT;
        [SerializeField] private GameObject _thombGeminiGPT;
        [SerializeField] private GameObject _thomb;
        [SerializeField] private Button _closeButton;

        private LeaderboardViewModel _viewModel;

        public override async UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);

            await _viewModel.CreatePlayerLines(_playersContainer, _myPlayerContainer);
            await _viewModel.TrySetGeminiComment();
        }

        protected override void Subscribes()
        {
            _closeButton.onClick.AddListener(_viewModel.OnCloseButtonClicked);
            _viewModel.IsLeaderboardLoaded.Subscribe(OnLeaderboardLoaded);
            _viewModel.DescriptionText.Subscribe(OnDescriptionTextChanged);
        }

        protected override void Unsubscribes()
        {
            _closeButton.onClick.RemoveAllListeners();
            _viewModel.IsLeaderboardLoaded.Unsubscribe(OnLeaderboardLoaded);
            _viewModel.DescriptionText.Unsubscribe(OnDescriptionTextChanged);
        }

        private void OnDescriptionTextChanged(string text)
        {
            _thombGeminiGPT.SetActive(string.IsNullOrEmpty(text));
            _textGeminiGPT.text = text;
        }

        private void OnLeaderboardLoaded(bool state)
        {
            _thomb.SetActive(state == false);
        }

        public override UniTask AnimateShowing()
        {
            return _animations.ShowByScaleAndAlpha();
        }

        public override UniTask AnimateClosing()
        {
            return _animations.HideByScaleAndAlpha();
        }
    }
}