using GameLogic.Audio;
using GameLogic.Bootstrapper;
using GameLogic.Model.DataProviders;
using Infrastructure.Extensions;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class ClickUndistributedCluster : BaseGameplayViewModelAction
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

            var originalCluster = context.Click.OriginUndistributedClickedCluster;
            originalCluster.SetBackgroundColor(_colorsSettings.GhostClusterBackColor);
            originalCluster.SetTextColor(_colorsSettings.GhostClusterTextColor);

            _viewManager.TryGetView<GameplayView>(out var gameplayView);
            var originalClusterText = originalCluster.GetText();
            var clickedCluster = _clusterFactory.Create();
            clickedCluster.SetParent(gameplayView.transform);
            clickedCluster.SetText(originalClusterText);
            clickedCluster.SetBackgroundColor(_colorsSettings.SelectedClusterBackColor);
            clickedCluster.SetTextColor(_colorsSettings.SelectedClusterTextColor);
            clickedCluster.SetRotation(_gameplaySettings.DraggedClusterRotation);
            var position = originalCluster.GetPosition();
            var offset = _gameplaySettings.DraggedClusterOffsetPosition.AddZ();
            clickedCluster.SetPosition(position + offset);
            context.Click.HintUndistributedClickedCluster = clickedCluster;

            foreach (var wordRow in context.WordRows)
            {
                if (CadAddClickHintCluster(context, wordRow))
                {
                    AddHintClusterToWordRow(context, wordRow);
                }
            }
        }

        private bool CadAddClickHintCluster(GameplayViewModelContext context, WordRow wordRow)
        {
            var clusterTextLength = context.Click.OriginUndistributedClickedCluster.GetText().Length;
            var wordLength = context.WordRowsClusters.GetWord(wordRow).Length;
            if (wordLength + clusterTextLength > _gameDefs.LevelSettings.WordLengthsRange.Max)
                return false;

            return true;
        }

        private void AddHintClusterToWordRow(GameplayViewModelContext context, WordRow wordRow)
        {
            var hintCluster = CreateHintCluster(context);
            hintCluster.SetParent(wordRow.ClustersHolder);
            context.Click.WordRowHintClusters.Add(wordRow, hintCluster);
            context.AllClusters.Add(hintCluster);
        }

        private Cluster CreateHintCluster(GameplayViewModelContext context)
        {
            var hintCluster = _clusterFactory.Create();
            hintCluster.SetBackgroundColor(_colorsSettings.GhostClusterBackColor);
            hintCluster.SetTextColor(_colorsSettings.GhostClusterTextColor);
            hintCluster.SetText(context.Click.OriginUndistributedClickedCluster.GetText());
            return hintCluster;
        }
    }
}