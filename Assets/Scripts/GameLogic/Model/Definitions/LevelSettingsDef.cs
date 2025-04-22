using System;

namespace GameLogic.Model.Definitions
{
    [Serializable]
    public class LevelSettingsDef: BaseDef
    {
        public Range WordsRange;
        public Range WordLengthsRange;
        public Range ClusterLengthsRange;
        public string RulesDescriptionLocalizationKey;
        public string LevelNumberLocalizationKey;
    }
}