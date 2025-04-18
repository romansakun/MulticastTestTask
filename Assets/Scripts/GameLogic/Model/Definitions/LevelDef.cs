using System.Collections.Generic;

namespace GameLogic.Model.Definitions
{
    public class LevelDef: BaseDef
    {
        public Dictionary<string, List<string>> WordsAndClusters { get; set; }
    }
}