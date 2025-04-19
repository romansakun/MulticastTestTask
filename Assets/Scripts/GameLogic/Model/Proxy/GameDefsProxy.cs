using System.Collections.Generic;
using GameLogic.Model.Definitions;

namespace GameLogic.Model.Proxy
{
    public class GameDefsProxy
    {
        public LevelSettingsDef LevelSettings => _gameDefs.LevelSettings;
        public IReadOnlyDictionary<string, LevelDef> Levels => _gameDefs.Levels;
        public IReadOnlyDictionary<string, LocalizationDef> Localizations => _gameDefs.Localizations;

        private GameDefs _gameDefs;

        public GameDefsProxy SetGameDefs(GameDefs gameDefs)
        {
            _gameDefs = gameDefs;
            return this;
        }

    }
}