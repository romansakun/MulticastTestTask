using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameLogic.Model.Contexts;
using GameLogic.Model.DataProviders;
using Infrastructure;
using Infrastructure.Services;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace GameLogic.Model.Repositories
{
    public class UserContextRepository : IDisposable
    {
        [Inject] private IFileService _fileService;
        [Inject] private GameDefsDataProvider _gameDefs;

        public IReactiveProperty<string> LocalizationDefId => _localizationDefId;
        public IReactiveProperty<string> UpdatedLevelDefId => _localizationDefId;
        public IReactiveProperty<bool> IsSoundsMuted => _isSoundsMuted;
        public IReactiveProperty<int> CheckingWordsCount => _checkingWordsCount;
        public IReactiveProperty<int> AdsTipsCount => _adsTipsCount;

        private readonly ReactiveProperty<string> _localizationDefId = new();
        private readonly ReactiveProperty<string> _updatedLevelDefId = new();
        private readonly ReactiveProperty<bool> _isSoundsMuted = new();
        private readonly ReactiveProperty<int> _checkingWordsCount = new();
        private readonly ReactiveProperty<int> _adsTipsCount = new();

        private readonly UserContext _userContext;
        private bool _willSave = false;


        public UserContextRepository(UserContext userContext)
        {
            _userContext = userContext;
            _localizationDefId.Value = userContext.LocalizationDefId;
            _isSoundsMuted.Value = userContext.IsSoundsMuted;
            _checkingWordsCount.Value = userContext.Consumables.WordsCheckingCount;
            _adsTipsCount.Value = userContext.Consumables.AdsTipCount;
        }

        public void SetSoundsMuted(bool isMuted)
        {
            _userContext.IsSoundsMuted = isMuted;
            _isSoundsMuted.SetValueAndForceNotify(isMuted);
        }

        public void SetLocalization(string localizationDefId)
        {
            if (_gameDefs.Localizations.TryGetValue(localizationDefId, out _) == false)
            {
                Debug.LogWarning($"'{localizationDefId}' localization not found");
                localizationDefId = _userContext.LocalizationDefId ?? _gameDefs.DefaultSettings.LocalizationDefId;
            }
            _userContext.LocalizationDefId = localizationDefId;

            _localizationDefId.SetValueAndForceNotify(localizationDefId);
        }

        public void CompleteLevel(string levelDefId)
        {
            _userContext.LevelsProgress[levelDefId].IsCompleted = true;
            _updatedLevelDefId.SetValueAndForceNotify(levelDefId);

            _checkingWordsCount.Value = _userContext.Consumables.WordsCheckingCount;
        }

        public void AddOrUpdateLevelProgress(string needLevelDefId, List<string> undistributedClusters, List<List<string>> distributedClusters)
        {
            if (_userContext.LevelsProgress.TryGetValue(needLevelDefId, out var needLevel) == false)
            {
                needLevel = new LevelProgressContext
                {
                    LevelDefId = needLevelDefId
                };
                _userContext.LevelsProgress.Add(needLevelDefId, needLevel);
            }
            needLevel.UndistributedClusters = undistributedClusters;
            needLevel.DistributedClusters = distributedClusters;

            _updatedLevelDefId.SetValueAndForceNotify(needLevelDefId);
        }

        public bool IsAnyLevelProgressExist()
        {
            return _userContext.LevelsProgress.Count > 0;
        }

        public bool IsLevelProgressExist(string needLevelDefId)
        {
            return _userContext.LevelsProgress.ContainsKey(needLevelDefId);
        }

        public bool TryGetLevelProgress(string needLevelDefId, out LevelProgressContext levelProgress)
        {
            return _userContext.LevelsProgress.TryGetValue(needLevelDefId, out levelProgress);
        }

        public bool IsLevelCompleted(string levelDefId)
        {
            return _userContext.LevelsProgress.TryGetValue(levelDefId, out var levelProgress) && levelProgress.IsCompleted;
        }

        public bool IsHowToPlayHintShown()
        {
            return _userContext.IsHowToPlayHintShown;
        }

        public void SetHowToPlayHintShown()
        {
            _userContext.IsHowToPlayHintShown = true;
        }

        public int GetAllCompletedLevels()
        {
            var result = 0;
            foreach (var pair in _userContext.LevelsProgress)
            {
                if (pair.Value.IsCompleted)
                    result += 1;
            }
            return result;
        }

        public bool IsLocalizationLevelsCompleted(string localizationDefId)
        {
            var levels = _gameDefs.Localizations[localizationDefId].Levels;
            foreach (var pair in levels)
            {
                if (_userContext.LevelsProgress.TryGetValue(pair.Value, out var levelProgress) == false)
                    return false;
                if (levelProgress.IsCompleted == false)
                    return false;
            }
            return true;
        }

        public int GetConsumablesUpdateDurationSeconds()
        {
            var durationSeconds = _gameDefs.DefaultSettings.ConsumablesUpdateIntervalSeconds;
            var nextUpdate = _userContext.Consumables.LastFreeUpdateTime.AddSeconds(durationSeconds);
            return (int)(nextUpdate - DateTime.Now).TotalSeconds;
        }

        public bool TryUpdateFreeConsumablesCount()
        {
            var dateTime = DateTime.Now;
            var checkingWords = _userContext.Consumables;
            var durationSeconds = _gameDefs.DefaultSettings.ConsumablesUpdateIntervalSeconds;
            if (checkingWords.LastFreeUpdateTime != default && checkingWords.LastFreeUpdateTime.AddSeconds(durationSeconds) > dateTime) 
                return false;

            _userContext.Consumables.WordsCheckingCount = _gameDefs.DefaultSettings.CheckingWordsDailyFreeCount;
            _userContext.Consumables.AdsTipCount = _gameDefs.DefaultSettings.AdsTipDailyFreeCount;
            _userContext.Consumables.LastFreeUpdateTime = dateTime;

            _checkingWordsCount.Value = _userContext.Consumables.WordsCheckingCount;
            _adsTipsCount.Value = _userContext.Consumables.AdsTipCount;

            return true;
        }

        public void AddCheckingWords()
        {
            _userContext.Consumables.WordsCheckingCount += 1;
            _checkingWordsCount.Value = _userContext.Consumables.WordsCheckingCount;
        }

        public void UseCheckingWords()
        {
            _userContext.Consumables.WordsCheckingCount -= 1;
            _checkingWordsCount.Value = _userContext.Consumables.WordsCheckingCount;
        }

        public void AddAdsTip()
        {
            _userContext.Consumables.AdsTipCount += 1;
            _adsTipsCount.Value = _userContext.Consumables.AdsTipCount;
        }

        public void UseAdsTip()
        {
            _userContext.Consumables.AdsTipCount -= 1;
            _adsTipsCount.Value = _userContext.Consumables.AdsTipCount;
        }

        public void ClearProgress()
        {
            var localizationDefId = _userContext.LocalizationDefId;
            var levels = _gameDefs.Localizations[localizationDefId].Levels;
            foreach (var pair in levels)
            {
                _userContext.LevelsProgress.Remove(pair.Value);
            }
        }

        public async void Save()
        {
            if (_willSave) 
                return;

            _willSave = true;
            await UniTask.Yield();
            _willSave = false;

            SaveInternal();
        }

        private void SaveInternal()
        {
            var json = JsonConvert.SerializeObject(_userContext, Formatting.Indented);
            _fileService.WriteAllText(GamePaths.PlayerContext, json);
        }

        public void Dispose()
        {
            _localizationDefId.Dispose();
            _updatedLevelDefId.Dispose();
        }
    }
}