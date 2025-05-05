using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.Settings
{
    public class SettingsView : View 
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Toggle _soundsToggle;
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
            _russianLocalizationButton.onClick.AddListener(() => _viewModel.SetLocalization(SystemLanguage.Russian));
            _englishLocalizationButton.onClick.AddListener(() => _viewModel.SetLocalization(SystemLanguage.English));
            _backButton.onClick.AddListener(_viewModel.OnBackButtonClicked);

            _viewModel.IsSoundsMuted.Subscribe(OnSoundsMutedChanged);
        }

        protected override void Unsubscribes()
        {
            _soundsToggle.onValueChanged.RemoveAllListeners();
            _russianLocalizationButton.onClick.RemoveAllListeners();
            _englishLocalizationButton.onClick.RemoveAllListeners();
            _backButton.onClick.RemoveAllListeners();
            
            _viewModel.IsSoundsMuted.Unsubscribe(OnSoundsMutedChanged);
        }

        private void OnSoundsToggleValueChanged(bool state)
        {
            _viewModel.OnSoundsToggleValueChanged(state);
        }

        private void OnSoundsMutedChanged(bool isMuted)
        {
            _soundsToggle.isOn = isMuted == false;
        }
    }
}