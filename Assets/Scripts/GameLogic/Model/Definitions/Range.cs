using System;
using MessagePack;

namespace GameLogic.Model.Definitions
{
    [MessagePackObject]
    public class Range
    {
        [Key(0)]
        public int Min { get; set; }
        [Key(1)]
        public int Max { get; set; }

        public bool IsInRange(int value)
        {
            return value >= Min && value <= Max;
        }

        public bool IsValid()
        {
            return Min <= Max;
        }
    }
}