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

            context.Click.OriginUndistributedClickedCluster.SetBackgroundColor(_colorsSettings.DefaultClusterBackColor);
            context.Click.OriginUndistributedClickedCluster.SetTextColor(_colorsSettings.DefaultClusterTextColor);
            _audioPlayer.PlaySound(_soundsSettings.DropClusterSound);
        }
    }
}