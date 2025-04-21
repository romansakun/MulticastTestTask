using GameLogic.Bootstrapper;
using GameLogic.Model.DataProviders;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class PrepareDragUndistributedCluster : BaseGameplayViewModelAction
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
            var originalClusterText = originalCluster.GetText();
            var swipingCluster = _clusterFactory.Create();
            swipingCluster.SetParent(gameplayView.transform);
            swipingCluster.SetText(originalClusterText);
            swipingCluster.SetBackgroundColor(_colorsSettings.SelectedClusterBackColor);
            swipingCluster.SetTextColor(_colorsSettings.SelectedClusterTextColor);
            context.DraggedCluster = swipingCluster;

            BlinkSuitableWordRows(context);

            context.IsUndistributedClustersScrollRectActive.Value = false;
        }

        private void BlinkSuitableWordRows(GameplayViewModelContext context)
        {
            foreach (var wordRow in context.WordRows)
            {
                var clusterLength = context.OriginDraggedCluster.GetText().Length;
                var wordLength = context.WordRowsClusters.GetWord(wordRow).Length;
                if (wordLength + clusterLength > _gameDefs.LevelSettings.WordsRange.Max)
                    continue;

                wordRow.SetEnabledBlinking(true);
            }
        }

    }
}