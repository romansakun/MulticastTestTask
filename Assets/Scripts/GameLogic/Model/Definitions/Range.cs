using System;

namespace GameLogic.Model.Definitions
{
    [Serializable]
    public class Range
    {
        public int Min { get; set; }
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