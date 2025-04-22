using GameLogic.Audio;
using GameLogic.Bootstrapper;
using GameLogic.Model.DataProviders;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class PrepareDragDistributedCluster : BaseGameplayViewModelAction
    {
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private SoundsSettings _soundsSettings;
        [Inject] private Cluster.Factory _clusterFactory;
        [Inject] private ColorsSettings _colorsSettings;
        [Inject] private GameplaySettings _gameplaySettings;
        [Inject] private ViewManager _viewManager;
        [Inject] private GameDefsDataProvider _gameDefs;

        public override void Execute(GameplayViewModelContext context)
        {
            _audioPlayer.PlaySound(_soundsSettings.TapSound);

            var originalCluster = context.OriginDraggedCluster;
            originalCluster.SetBackgroundColor(_colorsSettings.GhostClusterBackColor);
            originalCluster.SetTextColor(_colorsSettings.GhostClusterTextColor);

            _viewManager.TryGetView<GameplayView>(out var gameplayView);
            var swipingCluster = _clusterFactory.Create();
            swipingCluster.SetParent(gameplayView.transform);
            swipingCluster.SetText(originalCluster.GetText());
            swipingCluster.SetBackgroundColor(_colorsSettings.SelectedClusterBackColor);
            swipingCluster.SetTextColor(_colorsSettings.SelectedClusterTextColor);
            swipingCluster.SetRotation(_gameplaySettings.DraggedClusterRotation);
            context.DraggedCluster = swipingCluster;

            BlinkSuitableWordRows(context);
        }

        private void BlinkSuitableWordRows(GameplayViewModelContext context)
        {
            foreach (var wordRow in context.WordRows)
            {
                if (wordRow == context.OriginDraggedClusterWordRow)
                {
                    wordRow.SetEnabledBlinking(true);
                    continue;
                }
                var clusterTextLength = context.OriginDraggedCluster.GetText().Length;
                var wordLength = context.WordRowsClusters.GetWord(wordRow).Length;
                if (clusterTextLength + wordLength > _gameDefs.LevelSettings.WordLengthsRange.Max)
                    continue;

                wordRow.SetEnabledBlinking(true);
            }
        }

    }
}