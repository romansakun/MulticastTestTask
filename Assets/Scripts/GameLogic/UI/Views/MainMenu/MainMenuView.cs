using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.MainMenu
{
    public class MainMenuView : View
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _showAdRewardButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private GameObject _gameOverLabel;
        [SerializeField] private TextMeshProUGUI _levelsCompletedNumberText;
        [SerializeField] private TextMeshProUGUI _consumablesInfoText;

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
            _showAdRewardButton.onClick.AddListener(_viewModel.OnRewardAdButtonClicked);
            _viewModel.CompletedLevelsCount.Subscribe(OnLevelsCompletedCountChanged);
            _viewModel.IsLocalizationGameOver.Subscribe(OnLocalizationGameOverChanged);
            _viewModel.IsConsumablesInfoTextVisible.Subscribe(OnConsumablesInfoTextVisibleChanged);
            _viewModel.ConsumablesInfoText.Subscribe(OnConsumablesInfoTextChanged);
        }

        protected override void Unsubscribes()
        {
            _playButton.onClick.RemoveListener(_viewModel.OnPlayButtonClicked);
            _settingsButton.onClick.RemoveListener(_viewModel.OnSettingsButtonClicked);
            _showAdRewardButton.onClick.RemoveListener(_viewModel.OnRewardAdButtonClicked);
            _viewModel.CompletedLevelsCount.Unsubscribe(OnLevelsCompletedCountChanged);
            _viewModel.IsLocalizationGameOver.Unsubscribe(OnLocalizationGameOverChanged);
            _viewModel.IsConsumablesInfoTextVisible.Unsubscribe(OnConsumablesInfoTextVisibleChanged);
            _viewModel.ConsumablesInfoText.Unsubscribe(OnConsumablesInfoTextChanged);
        }

        private void OnConsumablesInfoTextChanged(string text)
        {
            _consumablesInfoText.text = text;
        }

        private void OnConsumablesInfoTextVisibleChanged(bool state)
        {
            _consumablesInfoText.gameObject.SetActive(state);
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