using UnityEngine;

namespace GameLogic.UI.Gameplay
{
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
}