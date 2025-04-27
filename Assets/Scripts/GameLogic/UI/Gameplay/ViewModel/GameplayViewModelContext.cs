using System.Collections.Generic;
using GameLogic.Model.DataProviders;
using Infrastructure;
using Infrastructure.LogicUtility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameLogic.UI.Gameplay
{
    public class GameplayViewModelContext : IContext
    {
        public readonly ReactiveProperty<bool> IsUndistributedClustersScrollRectActive = new(true);
        public readonly ReactiveProperty<bool> IsHintClusterInUndistributedClusters = new(false);
        public readonly ReactiveProperty<bool> IsFailedCompleteLevel = new(false);

        public ClickContext Click { get; } = new();
        public SwipeContext Swipe { get; } = new();
        public LevelProgressContextDataProvider LevelProgress { get; set; }
        public RectTransform WordRowsHolder { get; set; }
        public RectTransform UndistributedClustersHolder { get; set; }

        public List<Cluster> AllClusters { get; } = new();
        public List<Cluster> DistributedClusters { get; } = new();
        public List<Cluster> UndistributedClusters { get; } = new();
        public List<WordRow> WordRows { get; } = new();
        public Dictionary<WordRow, List<Cluster>> WordRowsClusters { get; } = new();
        public (UserInputType Type, PointerEventData Data) Input { get; set; }
        public bool CheckCompleteLevel { get; set; }


        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            Swipe.Dispose();
            Click.Dispose();
            IsUndistributedClustersScrollRectActive.Dispose();
            IsHintClusterInUndistributedClusters.Dispose();
            IsFailedCompleteLevel.Dispose();
            AllClusters.ForEach(c => c.Dispose());
            WordRows.ForEach(w => w.Dispose());

            WordRows.Clear();
            AllClusters.Clear();
            DistributedClusters.Clear();
            UndistributedClusters.Clear();
            WordRowsClusters.Clear();

            IsDisposed = true;
        }

        public class SwipeContext
        {
            public bool IsSwipeInputNow { get; set; }

            public Cluster HintCluster { get; set; }
            public WordRow HintClusterWordRow { get; set; }

            public RectTransform HintClusterHolder { get; set; }

            public Cluster DraggedCluster { get; set; }
            public Cluster OriginDraggedCluster { get; set; }
            public WordRow OriginDraggedClusterWordRow { get; set; }
            public RectTransform OriginDraggedClusterHolder { get; set; }
            
            public void Dispose()
            {
            }
        }
        
        public class ClickContext
        {
            public bool IsClickInputNow { get; set; }
            public Dictionary<WordRow, Cluster> WordRowHintClusters { get; } = new();
            public Cluster HintUndistributedClickedCluster { get; set; }
            public Cluster OriginUndistributedClickedCluster { get; set; }
            public WordRow ClickedHintWordRow { get; set; }

            public void Dispose()
            {
                WordRowHintClusters.Clear();
            }
        }
    }
}