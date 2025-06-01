using System;
using System.Collections.Generic;

namespace GameLogic.Model.Definitions
{
    [Serializable]
    public class LeagueDef: BaseDef
    {
        public string WreathIcon { get; set; }
        public string RomanNumberIcon { get; set; }
        public List<string> Levels = new();
    }
}