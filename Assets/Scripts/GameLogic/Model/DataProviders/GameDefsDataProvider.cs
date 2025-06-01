using System;
using System.Collections.Generic;
using GameLogic.Model.Definitions;

namespace GameLogic.Model.DataProviders
{
    public class GameDefsDataProvider
    {
        public DefaultSettingsDef DefaultSettings => _gameDefs.DefaultSettings;
        public LevelSettingsDef LevelSettings => _gameDefs.LevelSettings;
        public IReadOnlyDictionary<string, LevelDef> Levels => _gameDefs.Levels;
        public IReadOnlyDictionary<string, LeagueDef> Leagues => _gameDefs.Leagues;
        public IReadOnlyDictionary<string, LocalizationDef> Localizations => _gameDefs.Localizations;

        private GameDefs _gameDefs;

        public GameDefsDataProvider SetGameDefs(GameDefs gameDefs)
        {
            _gameDefs = gameDefs;
            return this;
        }

        public List<LeagueDef> GetLocalizationLeagues(string localizationDefId)
        {
            var result = new List<LeagueDef>();
            var localizationLeagues = Localizations[localizationDefId].Leagues;
            foreach (var pair in localizationLeagues)
            {
                result.Add(Leagues[pair.Value]);
            }
            return result;
        }
        
        public LevelDef GetFirstLevelDef(string localizationDefId)
        {
            var localizationLevels = Localizations[localizationDefId].Levels;
            foreach (var pair in localizationLevels)
            {
                if (pair.Key == 1)
                    return Levels[pair.Value];
            }

            throw new Exception($"There is no first level in '{localizationDefId}' localization");
        }

        public bool TryGetPreviousLevelDef(string levelDefId, string localizationDefId, out LevelDef previousLevelDef)
        {
            var localizationLevels = Localizations[localizationDefId].Levels;
            previousLevelDef = null;
            foreach (var pair in localizationLevels)
            {
                if (pair.Value != levelDefId)
                    continue;

                if (pair.Key == 1)
                    return false;

                var previousLevelId = localizationLevels[pair.Key - 1];
                previousLevelDef = Levels[previousLevelId];
                return true;
            }

            throw new Exception($"There is no previous level for '{levelDefId}' in '{localizationDefId}' localization");
        }

        public bool TryGetNextLevelDef(string levelDefId, string localizationDefId, out LevelDef nextLevelDef)
        {
            var localizationLevels = Localizations[localizationDefId].Levels;
            nextLevelDef = null;
            foreach (var pair in localizationLevels)
            {
                if (pair.Value != levelDefId)
                    continue;

                if (pair.Key == localizationLevels.Count)
                    return false;

                var previousLevelId = localizationLevels[pair.Key + 1];
                nextLevelDef = Levels[previousLevelId];
                return true;
            }

            throw new Exception($"There is no previous level for '{levelDefId}' in '{localizationDefId}' localization");
        }

        public int GetLevelNumber(string levelDefId, string localizationDefId)
        {
            var levels = Localizations[localizationDefId].Levels;
            foreach (var pair in levels)
            {
                if (pair.Value == levelDefId)
                    return pair.Key;
            }
            return -1;
        }

        public bool IsLevelContainsCluster(string cluster, string levelDefId)
        {
            var levelDef = _gameDefs.Levels[levelDefId];
            foreach (var pair in levelDef.Words)
            {
                var word = pair.Key;
                var clustersLengths = pair.Value;
                var startIndex = 0;
                for (var i = 0; i < clustersLengths.Count; i++)
                {
                    var clusterLength = clustersLengths[i];
                    var levelCluster = word.Substring(startIndex, clusterLength);
                    startIndex += clusterLength;

                    if (levelCluster == cluster)
                        return true;
                }
            }
            return false;
        }

    }
}