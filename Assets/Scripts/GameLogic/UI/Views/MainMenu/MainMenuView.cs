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
        [SerializeField] private TextMeshProUGUI _formedWordNumberText;

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
            _viewModel.FormedWordCount.Subscribe(OnFormedWordCountChanged);
        }

        protected override void Unsubscribes()
        {
            _playButton.onClick.RemoveListener(_viewModel.OnPlayButtonClicked);
            _settingsButton.onClick.RemoveListener(_viewModel.OnSettingsButtonClicked);
            _showAdRewardButton.onClick.RemoveListener(_viewModel.OnRewardAdButtonClicked);
            _viewModel.FormedWordCount.Unsubscribe(OnFormedWordCountChanged);
        }

        private void OnFormedWordCountChanged(int count)
        {
            _formedWordNumberText.text = count.ToString();
        }
    }
}