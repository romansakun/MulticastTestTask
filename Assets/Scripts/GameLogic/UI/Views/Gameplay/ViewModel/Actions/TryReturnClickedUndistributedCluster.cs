using GameLogic.Audio;
using GameLogic.Bootstrapper;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class TryReturnClickedUndistributedCluster : BaseGameplayViewModelAction
    {
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private SoundsSettings _soundsSettings;
        [Inject] private ColorsSettings _colorsSettings;

        public override void Execute(GameplayViewModelContext context)
        {
            if (context.Click.OriginUndistributedClickedCluster == null)
                return;

            context.Click.OriginUndistributedClickedCluster.SetColorAlpha(1);
            _audioPlayer.PlaySound(_soundsSettings.DropClusterSound);
        }
    }
}