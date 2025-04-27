using System.Collections.Generic;

namespace GameLogic.UI.Gameplay
{
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