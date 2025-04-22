using System;
using System.Collections.Generic;

namespace GameLogic.Model.Definitions
{
    [Serializable]
    public class LevelDef: BaseDef
    {
        public string Name;
        public Dictionary<string, List<int>> Words = new();
    }
}