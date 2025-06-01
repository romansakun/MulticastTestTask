using Cysharp.Threading.Tasks;
using GameLogic.Audio;
using Infrastructure;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class MusicLoading : IAsyncOperation
    {
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private SoundsSettings _soundsSettings;

        public UniTask ProcessAsync()
        {
            _audioPlayer.PlayMusic(_soundsSettings.BackgroundMusic);
            return UniTask.CompletedTask;
        }
    }
}