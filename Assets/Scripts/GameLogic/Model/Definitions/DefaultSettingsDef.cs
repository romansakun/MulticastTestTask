using System;

namespace GameLogic.Model.Definitions
{
    [Serializable]
    public class DefaultSettingsDef: BaseDef
    {
        public string LocalizationDefId;

        public int ConsumablesUpdateIntervalSeconds;
        public int ConsumablesFreeCount;
        public int ConsumablesByAdsCount;

        // public int ScoreByFinishedLevel;
        // public int MaxScoreByLevel;
        // public int DecrementScoreByMove;
    }
}