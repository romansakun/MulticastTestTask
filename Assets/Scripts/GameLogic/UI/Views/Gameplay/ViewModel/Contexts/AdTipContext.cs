using System.Collections.Generic;

namespace GameLogic.UI.Gameplay
{
    public class AdTipContext
    {
        public bool IsAdTip { get; set; }

        public List<string> FormedWords = new List<string>();
        public List<string> NotFormedWords = new List<string>();
        public List<WordRow> ImmutableWordRows = new List<WordRow>();
        public WordRow SuitableWordRow { get; set; }

        public void Reset()
        {
            IsAdTip = false;
            FormedWords.Clear();
            NotFormedWords.Clear();
            ImmutableWordRows.Clear();
            SuitableWordRow = null;
        }

        public void Dispose()
        {
        }
    }
}