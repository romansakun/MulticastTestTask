using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Model.Contexts;
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

        private readonly UserContextRepository _userContextRepository;


        public UserContextDataProvider(UserContextRepository userContextRepositoryRepository)
        {
            _userContextRepository = userContextRepositoryRepository;
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

        public bool IsAnyLevelProgressExist()
        {
            return _userContextRepository.IsAnyLevelProgressExist();
        }

        public bool IsLevelProgressExist(string needLevelDefId)
        {
            return _userContextRepository.IsLevelProgressExist(needLevelDefId);
        }

        public bool TryGetLevelProgress(string needLevelDefId, out LevelProgressContext levelProgress)
        {
            return _userContextRepository.TryGetLevelProgress(needLevelDefId, out levelProgress);
        }

        public bool IsLevelCompleted(string levelDefId)
        {
            return _userContextRepository.IsLevelCompleted(levelDefId);
        }

        public bool TryGetLastLevelDefId(out string lastLevelDefId)
        {
            return TryGetLastLevelDefId(LocalizationDefId.Value, out lastLevelDefId);
        }

        public bool TryGetLastLevelDefId(string localizationDefId, out string lastLevelDefId)
        {
            var levels = _gameDefs.Localizations[localizationDefId].Levels;
            for (var index = 1; index <= levels.Count; index++)
            {
                var levelDefId = levels[index];
                if (IsLevelCompleted(levelDefId))
                    continue;

                lastLevelDefId = levelDefId;
                return true;
            }

            lastLevelDefId = null;
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

        public void Dispose()
        {
            _userContextRepository.Dispose();
        }
    }
}