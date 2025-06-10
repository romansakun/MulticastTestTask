using System.Collections.Generic;
using MessagePack;

namespace GameLogic.Model.Definitions
{
    [MessagePackObject]
    public class LeagueDef: BaseDef
    {
        [Key(1)]
        public string WreathIcon { get; set; }
        [Key(2)]
        public string RomanNumberIcon { get; set; }
        [Key(3)]
        public List<string> Levels { get; set; } = new();
    }
}