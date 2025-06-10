using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.Settings
{
    public class SettingsView : View 
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Toggle _soundsToggle;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Button _russianLocalizationButton;
        [SerializeField] private Button _englishLocalizationButton;

        private SettingsViewModel _viewModel;

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);

            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
            _soundsToggle.onValueChanged.AddListener(OnSoundsToggleValueChanged);
            _musicToggle.onValueChanged.AddListener(OnMusicToggleValueChanged);
            _russianLocalizationButton.onClick.AddListener(() => _viewModel.SetLocalization(SystemLanguage.Russian));
            _englishLocalizationButton.onClick.AddListener(() => _viewModel.SetLocalization(SystemLanguage.English));
            _backButton.onClick.AddListener(_viewModel.OnBackButtonClicked);

            _viewModel.IsSoundsMuted.Subscribe(OnSoundsMutedChanged);
            _viewModel.IsMusicMuted.Subscribe(OnMusicMutedChanged);
        }

        protected override void Unsubscribes()
        {
            _soundsToggle.onValueChanged.RemoveAllListeners();
            _musicToggle.onValueChanged.RemoveAllListeners();
            _russianLocalizationButton.onClick.RemoveAllListeners();
            _englishLocalizationButton.onClick.RemoveAllListeners();
            _backButton.onClick.RemoveAllListeners();
            
            _viewModel.IsSoundsMuted.Unsubscribe(OnSoundsMutedChanged);
            _viewModel.IsMusicMuted.Unsubscribe(OnMusicMutedChanged);
        }

        private void OnSoundsMutedChanged(bool isMuted)
        {
            _soundsToggle.isOn = isMuted == false;
        }

        private void OnMusicMutedChanged(bool isMuted)
        {
            _musicToggle.isOn = isMuted == false;
        }

        private void OnSoundsToggleValueChanged(bool state)
        {
            _viewModel.OnSoundsToggleValueChanged(state);
        }

        private void OnMusicToggleValueChanged(bool state)
        {
            _viewModel.OnMusicToggleValueChanged(state);
        }
    }
}