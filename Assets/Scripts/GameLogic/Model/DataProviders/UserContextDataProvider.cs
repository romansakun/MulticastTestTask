using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Model.Repositories;
using Infrastructure;
using UnityEngine;
using Zenject;

namespace GameLogic.Model.DataProviders
{
    public class UserContextDataProvider : IDisposable
    {
        [Inject] private GameDefsDataProvider _gameDefs;

        public IReactiveProperty<string> LocalizationDefId => _userContextRepository.LocalizationDefId;
        public IReactiveProperty<string> UpdatedLevelDefId => _userContextRepository.UpdatedLevelDefId;
        public IReactiveProperty<bool> IsSoundsMuted => _userContextRepository.IsSoundsMuted;
        public IReactiveProperty<int> CheckingWordsCount => _userContextRepository.CheckingWordsCount;
        public IReactiveProperty<int> AdsTipsCount => _userContextRepository.AdsTipsCount;

        private readonly UserContextRepository _userContextRepository;


        public UserContextDataProvider(UserContextRepository userContextRepositoryRepository)
        {
            _userContextRepository = userContextRepositoryRepository;
        }

        public int GetConsumablesUpdateDurationSeconds()
        {
            return _userContextRepository.GetConsumablesUpdateDurationSeconds();
        }

        public string GetLocalizedText(string key)
        {
            var localizationDef = _gameDefs.Localizations[LocalizationDefId.Value];
            if (localizationDef.LocalizationText.TryGetValue(key, out var text))
            {
                return text;
            }
            Debug.LogWarning($"'{key}' not found in '{localizationDef.Id}' localization");
            return key;
        }

        public string GetLocalizedText(string key, params object[] args)
        {
            var text = GetLocalizedText(key);
            return string.Format(text, args);
        }

        public bool IsHowToPlayHintShown()
        {
            return _userContextRepository.IsHowToPlayHintShown();
        }

        public bool IsAnyLevelProgressExist()
        {
            return _userContextRepository.IsAnyLevelProgressExist();
        }

        public bool IsLevelProgressExist(string needLevelDefId)
        {
            return _userContextRepository.IsLevelProgressExist(needLevelDefId);
        }

        public bool TryGetLevelProgress(string needLevelDefId, out LevelProgressContextDataProvider levelProgressDataProvider)
        {
            levelProgressDataProvider = null;
            if (_userContextRepository.TryGetLevelProgress(needLevelDefId, out var levelProgress) == false)
                return false;

            levelProgressDataProvider = new LevelProgressContextDataProvider(levelProgress);
            return true;
        }

        public bool IsLevelCompleted(string levelDefId)
        {
            return _userContextRepository.IsLevelCompleted(levelDefId);
        }

        public bool TryGetLastUncompletedLevelProgress(out LevelProgressContextDataProvider levelProgress)
        {
            return TryGetLastUncompletedLevelProgress(LocalizationDefId.Value, out levelProgress);
        }

        public bool IsCurrentLocalizationLevelsCompleted()
        {
            return _userContextRepository.IsLocalizationLevelsCompleted(_userContextRepository.LocalizationDefId.Value);
        }

        public bool IsLocalizationLevelsCompleted(string localizationDefId)
        {
            return _userContextRepository.IsLocalizationLevelsCompleted(localizationDefId);
        }

        public int GetAllCompletedLevels()
        {
            return _userContextRepository.GetAllCompletedLevels();  
        }

        public bool TryGetLastUncompletedLevelProgress(string localizationDefId, out LevelProgressContextDataProvider levelProgress)
        {
            levelProgress = null;
            var levels = _gameDefs.Localizations[localizationDefId].Levels;
            for (var index = 1; index <= levels.Count; index++)
            {
                var levelDefId = levels[index];
                if (IsLevelCompleted(levelDefId))
                    continue;

                if (TryGetLevelProgress(levelDefId, out levelProgress))
                    return true;
            }
            return false;
        }

        public bool TryGetLastCompletedLevelProgress(out LevelProgressContextDataProvider previousLevelProgress)
        {
            var previousLevelDefId = string.Empty;
            var localizationDefId = LocalizationDefId.Value;
            var levels = _gameDefs.Localizations[localizationDefId].Levels;
            for (var index = 1; index <= levels.Count; index++)
            {
                var levelDefId = levels[index];
                if (IsLevelCompleted(levelDefId))
                    previousLevelDefId = levelDefId;
                else
                    break;
            }
            return TryGetLevelProgress(previousLevelDefId, out previousLevelProgress);
        }

        public bool TryGetNewNextLevelDefId(out string nextLevelDefId)
        {
            nextLevelDefId = null;
            var levels = _gameDefs.Localizations[LocalizationDefId.Value].Levels;
            for (var index = 1; index <= levels.Count; index++)
            {
                var levelDefId = levels[index];
                if (IsLevelCompleted(levelDefId))
                    continue;

                if (TryGetLevelProgress(levelDefId, out _))
                    return false;

                nextLevelDefId = levelDefId;
                return true;
            }
            return false;
        }

        public bool CheckUserGuessedWords(string levelDefId)
        {
            if (_userContextRepository.TryGetLevelProgress(levelDefId, out var levelProgress) == false)
                return false;

            var guessedWords = new List<string>();
            var sb = new StringBuilder();
            foreach (var clusters in levelProgress.DistributedClusters)
            {
                sb.Clear();
                foreach (var cluster in clusters)
                {
                    sb.Append(cluster);
                }
                guessedWords.Add(sb.ToString());
            }
            var levelDef = _gameDefs.Levels[levelDefId];
            foreach (var pair in levelDef.Words)
            {
                var hiddenWord = pair.Key;
                if (guessedWords.Contains(hiddenWord) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckLevelProgress(string levelDefId)
        {
            if (_userContextRepository.TryGetLevelProgress(levelDefId, out var levelProgress) == false)
                return false;

            if (_gameDefs.Levels.TryGetValue(levelDefId, out var levelDef) == false)
                return false;

            var levelClusters = new List<string>();
            foreach (var pair in levelDef.Words)
            {
                var word = pair.Key;
                var clustersLengths = pair.Value;
                var startIndex = 0;
                for (var i = 0; i < clustersLengths.Count; i++)
                {
                    var clusterLength = clustersLengths[i];
                    var levelCluster = word.Substring(startIndex, clusterLength);
                    levelClusters.Add(levelCluster);
                    startIndex += clusterLength;
                }
            }
            foreach (var rowClusters in levelProgress.DistributedClusters)
            {
                foreach (var cluster in rowClusters)
                {
                    if (levelClusters.Contains(cluster)) continue;
                    return false;
                }
            }
            foreach (var cluster in levelProgress.UndistributedClusters)
            {
                if (levelClusters.Contains(cluster)) continue;
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            _userContextRepository.Dispose();
        }
    }
}