using Cysharp.Threading.Tasks;
using GameLogic.Audio;
using Infrastructure;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class MusicLoading : IAsyncOperation
    {
        [Inject] private AudioPlayer _audioPlayer;

        public UniTask ProcessAsync()
        {
            _audioPlayer.PlayMusic("BackgroundMusic");
            return UniTask.CompletedTask;
        }
    }
}