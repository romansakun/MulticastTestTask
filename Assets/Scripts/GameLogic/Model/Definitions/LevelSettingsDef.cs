using MessagePack;

namespace GameLogic.Model.Definitions
{
    [MessagePackObject]
    public class LevelSettingsDef: BaseDef
    {
        [Key(1)]
        public Range WordsRange { get; set; }
        [Key(2)]
        public Range WordLengthsRange { get; set; }
        [Key(3)]
        public Range ClusterLengthsRange { get; set; }
        [Key(4)]
        public string RulesDescriptionLocalizationKey { get; set; }
        [Key(5)]
        public string LevelNumberLocalizationKey { get; set; }
    }
}