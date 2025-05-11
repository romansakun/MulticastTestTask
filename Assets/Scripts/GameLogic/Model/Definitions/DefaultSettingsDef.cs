using System;

namespace GameLogic.Model.Definitions
{
    [Serializable]
    public class DefaultSettingsDef: BaseDef
    {
        public string LocalizationDefId;

        public int ConsumablesUpdateIntervalSeconds;
        public int CheckingWordsDailyFreeCount;
        public int CheckingWordsVictoryAddingCount;
        public int AdsTipDailyFreeCount;
    }
}