using MessagePack;

namespace GameLogic.Model.Definitions
{
    [MessagePackObject]
    public class DefaultSettingsDef: BaseDef
    {
        [Key(1)] public string LocalizationDefId { get; set; }
        [Key(2)] public int ConsumablesUpdateIntervalSeconds { get; set; }
        [Key(3)] public int ConsumablesFreeCount { get; set; }
        [Key(4)] public int ConsumablesByAdsCount { get; set; }

        // public int ScoreByFinishedLevel;
        // public int MaxScoreByLevel;
        // public int DecrementScoreByMove;
    }
}