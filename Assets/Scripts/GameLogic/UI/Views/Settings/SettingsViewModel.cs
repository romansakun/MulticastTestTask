using GameLogic.Audio;
using GameLogic.Bootstrapper;
using GameLogic.Factories;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Operators;
using Infrastructure;
using Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.Settings
{
    public class SettingsViewModel : ViewModel
    {
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private SoundsSettings _soundsSettings;
        [Inject] private UserContextOperator _userContextOperator;
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private IYandexLocalization _yandexLocalization;

        public IReactiveProperty<bool> IsSoundsMuted => _isSoundMuted;
        private ReactiveProperty<bool> _isSoundMuted;

        public override void Initialize()
        {
            _isSoundMuted = new ReactiveProperty<bool>(_userContext.IsSoundsMuted.Value);
        }

        public void SetLocalization(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.Russian:
                    _yandexLocalization.SetLocalization("ru");
                    break;
                default:
                    _yandexLocalization.SetLocalization("en");
                    break;
            }
        }

        public void OnSoundsToggleValueChanged(bool state)
        {
            var isMuted = state == false;
            _userContextOperator.SetSoundsMuted(isMuted);

            _audioPlayer.PlaySound(_soundsSettings.TapSound);
            _isSoundMuted.Value = isMuted;
        }

        public void OnBackButtonClicked()
        {
            _viewManager.Close<SettingsView>();
        }
    }
}