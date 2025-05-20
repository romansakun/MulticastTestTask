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
        [SerializeField] private Button _helpButton;
        [SerializeField] private GameObject _gameOverLabel;
        [SerializeField] private TextMeshProUGUI _levelsCompletedNumberText;

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
            _helpButton.onClick.AddListener(_viewModel.OnHelpButtonClicked);
            _viewModel.CompletedLevelsCount.Subscribe(OnLevelsCompletedCountChanged);
            _viewModel.IsLocalizationGameOver.Subscribe(OnLocalizationGameOverChanged);
        }

        protected override void Unsubscribes()
        {
            _playButton.onClick.RemoveListener(_viewModel.OnPlayButtonClicked);
            _settingsButton.onClick.RemoveListener(_viewModel.OnSettingsButtonClicked);
            _helpButton.onClick.RemoveAllListeners();
            _viewModel.CompletedLevelsCount.Unsubscribe(OnLevelsCompletedCountChanged);
            _viewModel.IsLocalizationGameOver.Unsubscribe(OnLocalizationGameOverChanged);
        }

        private void OnLevelsCompletedCountChanged(int count)
        {
            _levelsCompletedNumberText.text = count.ToString();
        }

        private void OnLocalizationGameOverChanged(bool state)
        {
            _playButton.gameObject.SetActive(state == false);
            _gameOverLabel.SetActive(state);
        }

    }
}