using GameLogic.Bootstrapper;
using GameLogic.Model.DataProviders;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class PrepareDragDistributedCluster : BaseGameplayViewModelAction
    {
        [Inject] private Cluster.Factory _clusterFactory;
        [Inject] private ColorsSettings _colorsSettings;
        [Inject] private ViewManager _viewManager;
        [Inject] private GameDefsDataProvider _gameDefs;

        public override void Execute(GameplayViewModelContext context)
        {
            var originalCluster = context.OriginDraggedCluster;
            originalCluster.SetBackgroundColor(_colorsSettings.GhostClusterBackColor);
            originalCluster.SetTextColor(_colorsSettings.GhostClusterTextColor);

            _viewManager.TryGetView<GameplayView>(out var gameplayView);
            var swipingCluster = _clusterFactory.Create();
            swipingCluster.SetParent(gameplayView.transform);
            swipingCluster.SetText(originalCluster.GetText());
            swipingCluster.SetBackgroundColor(_colorsSettings.SelectedClusterBackColor);
            swipingCluster.SetTextColor(_colorsSettings.SelectedClusterTextColor);
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
                var word = context.WordRowsClusters.GetWord(wordRow);
                if (_gameDefs.LevelSettings.WordsRange.Max < word.Length)
                    continue;

                wordRow.SetEnabledBlinking(true);
            }
        }

    }
}