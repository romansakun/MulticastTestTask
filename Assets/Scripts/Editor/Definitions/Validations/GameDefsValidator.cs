#if UNITY_EDITOR

using System;
using GameLogic.Model.Definitions;

namespace EditorDefinitions
{
    public class GameDefsValidator 
    {
        private readonly GameDefs _gameDefs;

        public GameDefsValidator(GameDefs gameDefs)
        {
            _gameDefs = gameDefs; 
        }

        public void Validate()
        {
            foreach (var pair in _gameDefs.Clusters)
            {
                var cluster = pair.Value;
                if (string.IsNullOrEmpty(cluster.Value))
                    throw new Exception($"[{cluster.Id}]: Cluster value is empty!");
            }
            foreach (var pair in _gameDefs.Levels)
            {
                var level = pair.Value;
                ValidateLevel(level);
            }
            foreach (var pair in _gameDefs.Localizations)
            {
                var localization = pair.Value;
                ValidateLocalization(localization);
            }
        }

        private void ValidateLevel(LevelDef level)
        {
            foreach (var pair in level.WordsAndClusters)
            {
                var hiddenWord = pair.Key;
                var clusterDefIds = pair.Value;
                var word = string.Empty;
                foreach (var clusterDefId in clusterDefIds)
                {
                    if (_gameDefs.Clusters.TryGetValue(clusterDefId, out var cluster))
                        word += cluster.Value;
                    else
                        throw new Exception($"Level[{level.Id}]: Cluster definition with id '{clusterDefId}' not found!");
                }
                if (hiddenWord != word)
                    throw new Exception($"Level[{level.Id}]: Hidden word '{hiddenWord}' is not equal to word '{word}'!");
            }
        }

        private void ValidateLocalization(LocalizationDef localization)
        {
            foreach (var pair in localization.Levels)
            {
                var levelDefId = pair.Value;
                if (_gameDefs.Levels.TryGetValue(levelDefId, out var level) == false)
                    throw new Exception($"Localization[{localization.Id}]: Level definition with id '{levelDefId}' not found!");
            }
        }

    }
}

#endif
