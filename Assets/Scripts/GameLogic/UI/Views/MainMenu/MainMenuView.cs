using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.MainMenu
{
    public class MainMenuView : View
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _infoButton;
        [SerializeField] private Button _leaderboardsButton;
        [Header("League")]
        [SerializeField] private TextMeshProUGUI _leagueLevelsLabel;
        [SerializeField] private Image _leagueWreathIcon;
        [SerializeField] private Image _leagueRomanNumberIcon;
        [SerializeField] private RectTransform _leagueRectTransform;
        [SerializeField] private Button _leftLeagueButton;
        [SerializeField] private Button _rightLeagueButton;

        private MainMenuViewModel _viewModel;
        private Sequence _leagueAnimation;

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);
            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
            _playButton.onClick.AddListener(_viewModel.OnPlayButtonClicked);
            _settingsButton.onClick.AddListener(_viewModel.OnSettingsButtonClicked);
            _infoButton.onClick.AddListener(_viewModel.OnInfoButtonClicked);
            _leaderboardsButton.onClick.AddListener(_viewModel.OnLeaderboardsButtonClicked);
            _leftLeagueButton.onClick.AddListener(_viewModel.OnLeftLeagueButtonClicked);
            _rightLeagueButton.onClick.AddListener(_viewModel.OnRightLeagueButtonClicked);
            _viewModel.LeagueWreathIcon.Subscribe(OnLeagueWreathIconChanged);
            _viewModel.LeagueLevelsLabel.Subscribe(OnLeagueLevelsLabelChanged);
            _viewModel.LeagueRomanNumberIcon.Subscribe(OnLeagueRomanNumberIconChanged);
            _viewModel.IsLocalizationGameOver.Subscribe(OnLocalizationGameOverChanged);
            _viewModel.LeftLeagueButtonVisible.Subscribe(OnLeftLeagueButtonVisibleChanged);
            _viewModel.RightLeagueButtonVisible.Subscribe(OnRightLeagueButtonVisibleChanged);
        }

        protected override void Unsubscribes()
        {
            _leagueAnimation?.Kill();
            _playButton.onClick.RemoveListener(_viewModel.OnPlayButtonClicked);
            _settingsButton.onClick.RemoveListener(_viewModel.OnSettingsButtonClicked);
            _infoButton.onClick.RemoveListener(_viewModel.OnInfoButtonClicked);
            _leaderboardsButton.onClick.RemoveAllListeners();
            _leftLeagueButton.onClick.RemoveAllListeners();
            _rightLeagueButton.onClick.RemoveAllListeners();
            _viewModel.LeagueWreathIcon.Unsubscribe(OnLeagueWreathIconChanged);
            _viewModel.LeagueLevelsLabel.Unsubscribe(OnLeagueLevelsLabelChanged);
            _viewModel.LeagueRomanNumberIcon.Unsubscribe(OnLeagueRomanNumberIconChanged);
            _viewModel.IsLocalizationGameOver.Unsubscribe(OnLocalizationGameOverChanged);
            _viewModel.LeftLeagueButtonVisible.Unsubscribe(OnLeftLeagueButtonVisibleChanged);
            _viewModel.RightLeagueButtonVisible.Unsubscribe(OnRightLeagueButtonVisibleChanged);
        }

        private void OnRightLeagueButtonVisibleChanged(bool state)
        {
            _rightLeagueButton.gameObject.SetActive(state);
        }

        private void OnLeftLeagueButtonVisibleChanged(bool state)
        {
            _leftLeagueButton.gameObject.SetActive(state); 
        }

        private void OnLeagueWreathIconChanged(Sprite sprite)
        {
            _leagueWreathIcon.enabled = sprite != null;
            _leagueWreathIcon.sprite = sprite;
        }

        private void OnLeagueRomanNumberIconChanged(Sprite sprite)
        {
            _leagueRomanNumberIcon.enabled = sprite != null;
            _leagueRomanNumberIcon.sprite = sprite;
            
            _leagueAnimation?.Kill();
            _leagueAnimation = DOTween.Sequence();
            _leagueAnimation.Append(_leagueRectTransform.DOScale(1.1f, 0.15f));
            _leagueAnimation.Append(_leagueRectTransform.DOScale(1f, 0.15f));
        }

        private void OnLeagueLevelsLabelChanged(string value)
        {
            _leagueLevelsLabel.text = value;
        }

        private void OnLocalizationGameOverChanged(bool state)
        {
            _playButton.gameObject.SetActive(state == false);
        }

        public void SetActiveLeaderboardsButton(bool state)
        {
            _leaderboardsButton.gameObject.SetActive(state);
        }
        
    }
}