using System.Collections.Generic;
using MessagePack;

namespace GameLogic.Model.Definitions
{
    [MessagePackObject]
    public class LevelDef: BaseDef
    {
        [Key(1)]
        public Dictionary<string, List<int>> Words { get; set; } = new();
    }
}