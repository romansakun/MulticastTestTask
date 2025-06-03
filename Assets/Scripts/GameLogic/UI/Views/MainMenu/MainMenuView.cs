using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.MainMenu
{
    public class MainMenuView : View
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _leaderboardsButton;
        [Header("League")]
        [SerializeField] private TextMeshProUGUI _leagueLevelsLabel;
        [SerializeField] private Image _leagueWreathIcon;
        [SerializeField] private Image _leagueRomanNumberIcon;

        private MainMenuViewModel _viewModel;

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);
            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
            _playButton.onClick.AddListener(_viewModel.OnPlayButtonClicked);
            _settingsButton.onClick.AddListener(_viewModel.OnSettingsButtonClicked);
            _leaderboardsButton.onClick.AddListener(_viewModel.OnLeaderboardsButtonClicked);
            _viewModel.LeagueWreathIcon.Subscribe(OnLeagueWreathIconChanged);
            _viewModel.LeagueLevelsLabel.Subscribe(OnLeagueLevelsLabelChanged);
            _viewModel.LeagueRomanNumberIcon.Subscribe(OnLeagueRomanNumberIconChanged);
            _viewModel.IsLocalizationGameOver.Subscribe(OnLocalizationGameOverChanged);
        }

        protected override void Unsubscribes()
        {
            _playButton.onClick.RemoveListener(_viewModel.OnPlayButtonClicked);
            _settingsButton.onClick.RemoveListener(_viewModel.OnSettingsButtonClicked);
            _leaderboardsButton.onClick.RemoveAllListeners();
            _viewModel.LeagueWreathIcon.Unsubscribe(OnLeagueWreathIconChanged);
            _viewModel.LeagueLevelsLabel.Unsubscribe(OnLeagueLevelsLabelChanged);
            _viewModel.LeagueRomanNumberIcon.Unsubscribe(OnLeagueRomanNumberIconChanged);
            _viewModel.IsLocalizationGameOver.Unsubscribe(OnLocalizationGameOverChanged);
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