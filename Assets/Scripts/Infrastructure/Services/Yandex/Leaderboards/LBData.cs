using System;

namespace Infrastructure.Services.Leaderboards
{
    [Serializable]
    public class LBData
    {
        public string technoName;
        public string entries;
        public bool isDefault;
        public bool isInvertSortOrder;
        public int decimalOffset;
        public string type;
        public LBPlayerData[] players;
        public LBCurrentPlayerData currentPlayer;
    }

    [Serializable]
    public class LBPlayerData
    {
        public int rank;
        public string name;
        public int score;
        public string photo;
        public string uniqueID;
        public string extraData;
    }

    [Serializable]
    public class LBCurrentPlayerData
    {
        public int rank;
        public int score;
        public string extraData;
        public string name;
    }
}