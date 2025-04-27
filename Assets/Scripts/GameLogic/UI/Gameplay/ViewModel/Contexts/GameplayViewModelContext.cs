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
        public AdTipContext AdTip { get; } = new();
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
            AdTip.Dispose();
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
    }

    public class AdTipContext
    {
        public bool IsAdTip { get; set; }
        
        public void Dispose()
        {
        }
    }
}