using System;

namespace GameLogic.Model.Contexts
{
    [Serializable]
    public class ConsumablesContext
    {
        public int WordsCheckingCount { get; set; }
        public int AdsTipCount { get; set; }
        public DateTime LastFreeUpdateTime { get; set; }
    }
}