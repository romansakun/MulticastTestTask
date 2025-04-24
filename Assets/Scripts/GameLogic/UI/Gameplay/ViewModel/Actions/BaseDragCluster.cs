using System.Collections.Generic;
using GameLogic.Bootstrapper;
using GameLogic.Model.DataProviders;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public abstract class BaseDragCluster : BaseGameplayViewModelAction
    {
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private Cluster.Factory _clusterFactory;
        [Inject] private ColorsSettings _colorsSettings;

        private readonly List<Cluster> _bufferClusters = new();


        protected bool TryGetWordRowUnderDraggedCluster(GameplayViewModelContext context, bool isDraggedClusterFromDistributed, out WordRow wordRow)
        {
            var draggedClusterScreenPoint = context.DraggedCluster.GetScreenPoint();
            wordRow = context.WordRows.Find(x => x.IsContainsScreenPoint(draggedClusterScreenPoint));
            if (wordRow == null) 
                return false;

            var dontIgnoreWordLength = isDraggedClusterFromDistributed && context.OriginDraggedClusterHolder != wordRow.ClustersHolder;
            var clusterTextLength = context.OriginDraggedCluster.GetText().Length;
            var wordLength = context.WordRowsClusters.GetWord(wordRow).Length;
            if (dontIgnoreWordLength  && wordLength + clusterTextLength > _gameDefs.LevelSettings.WordLengthsRange.Max)
                return false;

            return true;
        }

        protected void SetHintClusterAsUndistributed(GameplayViewModelContext context)
        {
            TryCreateHintCluster(context);
            if (context.HintClusterHolder != context.UndistributedClustersHolder)
            {
                context.HintCluster.SetParent(context.UndistributedClustersHolder);
                context.HintCluster.SetSiblingIndex(0);
                context.HintClusterHolder = context.UndistributedClustersHolder;
                context.IsHintClusterInUndistributedClusters.SetValueAndForceNotify(true);
            }
            context.OriginDraggedCluster.SetActive(false);
            context.HintClusterWordRow = null;
        }

        protected void SetHintClusterAsDistributed(GameplayViewModelContext context, WordRow wordRow, bool hideOriginDraggedCluster)
        {
            TryCreateHintCluster(context);
            if (context.HintClusterHolder != wordRow.ClustersHolder)
            {
                context.HintCluster.SetParent(wordRow.ClustersHolder);
                context.HintClusterHolder = wordRow.ClustersHolder;
                context.HintClusterWordRow = wordRow;
            }

            SetSiblingIndexForHintCluster(context, wordRow);

            if (hideOriginDraggedCluster)
                context.OriginDraggedCluster.SetActive(false);
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

        private void SetSiblingIndexForHintCluster(GameplayViewModelContext context, WordRow wordRow)
        {
            _bufferClusters.Clear();
            _bufferClusters.Add(context.HintCluster);
            _bufferClusters.AddRange(context.WordRowsClusters[wordRow]);
            var draggedClusterScreenPoint = context.DraggedCluster.GetScreenPoint();
            var closest = _bufferClusters.GetClosest(draggedClusterScreenPoint);
            if (closest != context.HintCluster)
            {
                var siblingIndex = context.WordRowsClusters.GetSiblingIndexForClusterPoint(wordRow, draggedClusterScreenPoint);
                context.HintCluster.SetSiblingIndex(siblingIndex);
            }
        }

    }
}