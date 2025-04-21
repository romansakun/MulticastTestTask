using System.Collections.Generic;
using GameLogic.Bootstrapper;
using GameLogic.Model.DataProviders;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class DragDistributedCluster : BaseGameplayViewModelAction
    {
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private Cluster.Factory _clusterFactory;
        [Inject] private ColorsSettings _colorsSettings;

        private readonly List<Cluster> _bufferClusters = new();
        private bool _isWordRowContainsOriginCluster;

        public override void Execute(GameplayViewModelContext context)
        {
            foreach (var wordRow in context.WordRows)
            {
                if (wordRow.IsContainsPoint(context.Input.Data.position) == false)
                    continue;

                _isWordRowContainsOriginCluster = context.OriginDraggedClusterHolder == wordRow.ClustersHolder;
                if (_isWordRowContainsOriginCluster == false)
                {
                    var clusterTextLength = context.OriginDraggedCluster.GetText().Length;
                    var wordLength = context.WordRowsClusters.GetWord(wordRow).Length;
                    if (wordLength + clusterTextLength > _gameDefs.LevelSettings.WordLengthsRange.Max)
                        continue;
                }

                SetHintClusterForWordRow(context, wordRow);
                return;
            }

            SetHintClusterForUndistributedClusters(context);
        }

        private void SetHintClusterForUndistributedClusters(GameplayViewModelContext context)
        {
            TryCreateHintCluster(context);
            if (context.IsHintClusterInUndistributedClusters.Value == false)
            {
                context.HintCluster.SetParent(context.UndistributedClustersHolder);
                context.HintCluster.SetSiblingIndex(0);
                context.IsHintClusterInUndistributedClusters.Value = true;
            }

            context.OriginDraggedCluster.SetActive(false);
            context.HintClusterWordRow = null;
        }

        private void SetHintClusterForWordRow(GameplayViewModelContext context, WordRow wordRow)
        {
            TryCreateHintCluster(context);
            if (context.HintClusterWordRow != wordRow)
            {
                context.HintClusterWordRow = wordRow;
                context.HintCluster.SetParent(wordRow.ClustersHolder);
            }
            SetSiblingIndexForHintCluster(context, wordRow);

            context.OriginDraggedCluster.SetActive(false);
            context.IsHintClusterInUndistributedClusters.Value = false;
        }

        private void SetSiblingIndexForHintCluster(GameplayViewModelContext context, WordRow wordRow)
        {
            _bufferClusters.Clear();
            _bufferClusters.Add(context.HintCluster);
            _bufferClusters.AddRange(context.WordRowsClusters[wordRow]);
            var closest = _bufferClusters.GetClosest(context.Input.Data.position);
            if (closest != context.HintCluster)
            {
                var siblingIndex = context.WordRowsClusters.GetSiblingIndex(wordRow, context.Input.Data.position);
                context.HintCluster.SetSiblingIndex(siblingIndex);
            }
        }

        private void TryCreateHintCluster(GameplayViewModelContext context)
        {
            if (context.HintCluster == null)
            {
                context.HintCluster = _clusterFactory.Create();
                context.HintCluster.SetBackgroundColor(_colorsSettings.GhostClusterBackColor);
                context.HintCluster.SetTextColor(_colorsSettings.GhostClusterTextColor);
                context.HintCluster.SetText(context.DraggedCluster.GetText());
            }
        }
    }
}