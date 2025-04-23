using GameLogic.Bootstrapper;
using GameLogic.Model.DataProviders;
using UnityEngine;
using Zenject;

namespace GameLogic.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private DiContainer _diContainer;

        [SerializeField] private AudioSource _soundAudioSource;
        [SerializeField] private AudioSource _musicAudioSource;

        private UserContextDataProvider _userContext;

        private void Awake()
        {
            _signalBus.Subscribe<UserContextInitializedSignal>(OnUserContextInitialized);
        }

        private void OnUserContextInitialized()
        {
            _userContext = _diContainer.Resolve<UserContextDataProvider>();
            _userContext.IsSoundsMuted.Subscribe(OnSoundMutedChanged);
        }

        private void OnSoundMutedChanged(bool isMuted)
        {
            _soundAudioSource.mute = isMuted;
            _musicAudioSource.mute = isMuted;
        }

        public void PlaySound(AudioClip audioClip)
        {
            if (_userContext.IsSoundsMuted.Value) return;
            if (audioClip == null)
            {
                Debug.LogWarning($"There is no sound");
                return;
            }

            _soundAudioSource.clip = audioClip;
            _soundAudioSource.Play();
        }

        public void PlayMusic(AudioClip music)
        {
            if (_userContext.IsSoundsMuted.Value) return;
            if (music == null)
            {
                Debug.LogWarning($"There is no music");
                return;
            }

            _musicAudioSource.clip = music;
            _musicAudioSource.Play();
        }

        private void OnDestroy()
        {
            _userContext.IsSoundsMuted.Unsubscribe(OnSoundMutedChanged);
            _signalBus.Unsubscribe<UserContextInitializedSignal>(OnUserContextInitialized);
        }
    }
}