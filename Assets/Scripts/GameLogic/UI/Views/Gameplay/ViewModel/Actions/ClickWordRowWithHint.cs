using GameLogic.Audio;
using GameLogic.Bootstrapper;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class ClickWordRowWithHint : BaseGameplayViewModelAction
    {
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private ColorsSettings _colorsSettings;

        public override void Execute(GameplayViewModelContext context)
        {
            _audioPlayer.PlaySound("DropClusterSound");

            var cluster = context.Click.OriginUndistributedClickedCluster;
            var wordRow = context.Click.ClickedHintWordRow;
            var hintCluster = context.Click.WordRowHintClusters[wordRow];
            context.Click.WordRowHintClusters.Remove(wordRow);
            context.AllClusters.Remove(hintCluster);
            hintCluster.Dispose();

            context.WordRowsClusters[wordRow].Add(cluster);
            context.UndistributedClusters.Remove(cluster);
            context.DistributedClusters.Add(cluster);

            wordRow.SetClusterAsChild(cluster);
            cluster.SetColorAlpha(1);
        }
    }
}