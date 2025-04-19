#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
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
            ValidateLevelsSettings();
            foreach (var pair in _gameDefs.Levels)
            {
                ValidateLevel(pair.Value);
            }
            foreach (var pair in _gameDefs.Localizations)
            {
                ValidateLocalization(pair.Value);
            }
        }

        private void ValidateLevelsSettings()
        {
            if (_gameDefs.LevelSettings == null)
                throw new Exception("LevelSettings is null!");

            var wordsRange = _gameDefs.LevelSettings.WordsRange;
            if (wordsRange.IsValid() == false || wordsRange.Min < 1)
                throw new Exception("LevelSettings.WordsRange is invalid!");

            var wordLengthsRange = _gameDefs.LevelSettings.WordLengthsRange;
            if (wordLengthsRange.IsValid() == false || wordLengthsRange.Min < 3)
                throw new Exception("LevelSettings.WordLengthsRange is invalid!");

            var clusterLengthsRange = _gameDefs.LevelSettings.ClusterLengthsRange;
            if (clusterLengthsRange.IsValid() == false || clusterLengthsRange.Min < 1)
                throw new Exception("LevelSettings.ClusterLengthsRange is invalid!");
        }

        private void ValidateLevel(LevelDef level)
        {
            if (_gameDefs.LevelSettings.WordsRange.IsInRange(level.Words.Count) == false)
                throw new Exception($"Level[{level.Id}]: Words count is out of range!");

            var words = new List<string>();
            foreach (var pair in level.Words)
            {
                var hiddenWord = pair.Key;
                if (_gameDefs.LevelSettings.WordLengthsRange.IsInRange(hiddenWord.Length) == false)
                    throw new Exception($"Level[{level.Id}]: Word '{hiddenWord}' length '{hiddenWord.Length}' is out of range!");

                if (words.Contains(hiddenWord))
                    throw new Exception($"Level[{level.Id}]: Duplicate word '{hiddenWord}'!");

                words.Add(hiddenWord);

                var clusters = pair.Value;
                var wordLength = 0;
                foreach (var clusterLength in clusters)
                {
                    if (_gameDefs.LevelSettings.ClusterLengthsRange.IsInRange(clusterLength) == false)
                        throw new Exception($"Level[{level.Id}]: Cluster length '{clusterLength}' is out of range!");

                    wordLength += clusterLength;
                }
                if (hiddenWord.Length != wordLength)
                    throw new Exception($"Level[{level.Id}]: Hidden word '{hiddenWord}' does not break into clusters!");
            }
        }

        private void ValidateLocalization(LocalizationDef localization)
        {
            var levelIds = new List<int>();
            foreach (var pair in localization.Levels)
            {
                levelIds.Add(pair.Key);
                var levelDefId = pair.Value;
                if (_gameDefs.Levels.TryGetValue(levelDefId, out var level) == false)
                    throw new Exception($"Localization[{localization.Id}]: Level definition with id '{levelDefId}' not found!");
            }

            if (levelIds.Any(x => x > levelIds.Count || x < 1))
                throw new Exception($"Localization[{localization.Id}]: Invalid level ids: {string.Join(", ", levelIds)}!");
        }

    }
}

#endif
