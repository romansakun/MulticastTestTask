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


        protected bool TryGetWordRowWithEmptyPlaceUnderDraggedCluster(GameplayViewModelContext context, bool isDraggedClusterFromDistributed, out WordRow wordRow)
        {
            var draggedClusterScreenPoint = context.Swipe.DraggedCluster.GetScreenPoint();
            wordRow = context.WordRows.Find(x => x.IsContainsScreenPoint(draggedClusterScreenPoint));
            if (wordRow == null) 
                return false;

            var canIgnoreWordLength = isDraggedClusterFromDistributed && context.Swipe.OriginDraggedClusterHolder == wordRow.ClustersHolder;
            if (canIgnoreWordLength)
                return true;

            var clusterTextLength = context.Swipe.OriginDraggedCluster.GetText().Length;
            var wordLength = context.WordRowsClusters.GetWord(wordRow).Length;
            if (wordLength + clusterTextLength > _gameDefs.LevelSettings.WordLengthsRange.Max)
                return false;

            return true;
        }

        protected void SetHintClusterAsUndistributed(GameplayViewModelContext context)
        {
            TryCreateHintCluster(context);
            if (context.Swipe.HintClusterHolder != context.UndistributedClustersHolder)
            {
                context.Swipe.HintCluster.SetParent(context.UndistributedClustersHolder);
                context.Swipe.HintCluster.SetSiblingIndex(0);
                context.Swipe.HintClusterHolder = context.UndistributedClustersHolder;
                context.IsHintClusterInUndistributedClusters.SetValueAndForceNotify(true);
            }
            context.Swipe.OriginDraggedCluster.SetActive(false);
            context.Swipe.HintClusterWordRow = null;
        }

        protected void SetHintClusterAsDistributed(GameplayViewModelContext context, WordRow wordRow, bool hideOriginDraggedCluster)
        {
            TryCreateHintCluster(context);
            if (context.Swipe.HintClusterHolder != wordRow.ClustersHolder)
            {
                context.Swipe.HintCluster.SetParent(wordRow.ClustersHolder);
                context.Swipe.HintClusterHolder = wordRow.ClustersHolder;
                context.Swipe.HintClusterWordRow = wordRow;
            }

            SetSiblingIndexForHintCluster(context, wordRow);

            if (hideOriginDraggedCluster)
                context.Swipe.OriginDraggedCluster.SetActive(false);
        }

        private void TryCreateHintCluster(GameplayViewModelContext context)
        {
            if (context.Swipe.HintCluster == null)
            {
                context.Swipe.HintCluster = _clusterFactory.Create();
                context.Swipe.HintCluster.SetBackgroundColor(_colorsSettings.GhostClusterBackColor);
                context.Swipe.HintCluster.SetTextColor(_colorsSettings.GhostClusterTextColor);
                context.Swipe.HintCluster.SetText(context.Swipe.DraggedCluster.GetText());
                context.AllClusters.Add(context.Swipe.HintCluster);
            }
        }

        private void SetSiblingIndexForHintCluster(GameplayViewModelContext context, WordRow wordRow)
        {
            _bufferClusters.Clear();
            _bufferClusters.Add(context.Swipe.HintCluster);
            _bufferClusters.AddRange(context.WordRowsClusters[wordRow]);
            var draggedClusterScreenPoint = context.Swipe.DraggedCluster.GetScreenPoint();
            var closest = _bufferClusters.GetClosest(draggedClusterScreenPoint);
            if (closest != context.Swipe.HintCluster)
            {
                var siblingIndex = context.WordRowsClusters.GetSiblingIndexForClusterPoint(wordRow, draggedClusterScreenPoint);
                context.Swipe.HintCluster.SetSiblingIndex(siblingIndex);
            }
        }

    }
}