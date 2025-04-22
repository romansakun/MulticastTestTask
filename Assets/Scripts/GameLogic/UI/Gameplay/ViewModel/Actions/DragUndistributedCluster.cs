using System.Collections.Generic;
using GameLogic.Bootstrapper;
using GameLogic.Model.DataProviders;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class DragUndistributedCluster : BaseGameplayViewModelAction
    {
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private Cluster.Factory _clusterFactory;
        [Inject] private ColorsSettings _colorsSettings;

        private readonly List<Cluster> _bufferClusters = new();
        private Vector2 _draggedClusterScreenPoint;
        private bool _isWordRowContainsOriginCluster;

        public override void Execute(GameplayViewModelContext context)
        {
            _draggedClusterScreenPoint = context.DraggedCluster.GetScreenPoint();

            foreach (var wordRow in context.WordRows)
            {
                if (wordRow.IsContainsScreenPoint(_draggedClusterScreenPoint) == false)
                    continue;

                var clusterTextLength = context.OriginDraggedCluster.GetText().Length;
                var wordLength = context.WordRowsClusters.GetWord(wordRow).Length;
                if (wordLength + clusterTextLength> _gameDefs.LevelSettings.WordLengthsRange.Max)
                    continue;

                SetHintClusterForWordRow(context, wordRow);
                return;
            }
            if (context.HintCluster != null)
            {
                context.HintCluster.Dispose();
                context.HintCluster = null;
                context.HintClusterWordRow = null;
            }
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

            context.IsHintClusterInUndistributedClusters.Value = false;
        }

        private void SetSiblingIndexForHintCluster(GameplayViewModelContext context, WordRow wordRow)
        {
            _bufferClusters.Clear();
            _bufferClusters.Add(context.HintCluster);
            _bufferClusters.AddRange(context.WordRowsClusters[wordRow]);
            var closest = _bufferClusters.GetClosest(_draggedClusterScreenPoint);
            if (closest != context.HintCluster)
            {
                var siblingIndex = context.WordRowsClusters.GetSiblingIndex(wordRow, _draggedClusterScreenPoint);
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