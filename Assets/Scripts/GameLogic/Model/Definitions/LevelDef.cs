using System;
using System.Collections.Generic;

namespace GameLogic.Model.Definitions
{
    [Serializable]
    public class LevelDef: BaseDef
    {
        public Dictionary<string, List<string>> WordsAndClusters = new();
    }
}