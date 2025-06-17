using GameLogic.Bootstrapper;
using GameLogic.Model.DataProviders;
using Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace GameLogic.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private DiContainer _diContainer;
        [Inject] private IAssetsLoader _assetsLoader;

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
            _userContext.IsMusicMuted.Subscribe(OnMusicMutedChanged);
        }

        private void OnDestroy()
        {
            _userContext?.IsSoundsMuted.Unsubscribe(OnSoundMutedChanged);
            _userContext?.IsMusicMuted.Unsubscribe(OnMusicMutedChanged);
            _signalBus.Unsubscribe<UserContextInitializedSignal>(OnUserContextInitialized);
        }

        private void OnMusicMutedChanged(bool isMuted)
        {
            _musicAudioSource.mute = isMuted;
        }

        private void OnSoundMutedChanged(bool isMuted)
        {
            _soundAudioSource.mute = isMuted;
        }

        public async void PlaySound(string soundName, bool force = false)
        {
            var sound = await _assetsLoader.LoadAsync<AudioClip>(soundName);
            PlaySound(sound, force);
        }

        public void PlaySound(AudioClip audioClip, bool force)
        {
            if (_userContext.IsSoundsMuted.Value) return;
            if (audioClip == null)
            {
                Debug.LogWarning($"There is no sound");
                return;
            }
            if (force == false && _soundAudioSource.isPlaying && _soundAudioSource.clip == audioClip) 
                return;

            _soundAudioSource.clip = audioClip;
            _soundAudioSource.Play();
        }

        public async void PlayMusic(string musicName)
        {
            var music = await _assetsLoader.LoadAsync<AudioClip>(musicName);
            PlayMusic(music);
        }

        public void PlayMusic(AudioClip music)
        {
            //if (_userContext.IsMusicMuted.Value) return;
            if (music == null)
            {
                Debug.LogWarning($"There is no music");
                return;
            }

            _musicAudioSource.clip = music;
            _musicAudioSource.Play();
        }

    }
}