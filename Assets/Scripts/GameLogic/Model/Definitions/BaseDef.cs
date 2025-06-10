using MessagePack;

namespace GameLogic.Model.Definitions
{
    [Union(0, typeof(DefaultSettingsDef))]
    [Union(1, typeof(LevelSettingsDef))]
    [Union(2, typeof(LeagueDef))]
    [Union(3, typeof(LevelDef))]
    [Union(4, typeof(LocalizationDef))]
    
    [MessagePackObject]
    public abstract class BaseDef
    {
        [Key(0)] public string Id { get; set; }
    }
}