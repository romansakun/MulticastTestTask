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

        private readonly ReactiveProperty<string> _localizationDefId = new();
        private readonly ReactiveProperty<string> _updatedLevelDefId = new();

        private readonly UserContext _userContext;
        private bool _willSave = false;


        public UserContextRepository(UserContext userContext)
        {
            _userContext = userContext;
            _localizationDefId.Value = userContext.LocalizationDefId;
        }

        public void SetLocalization(string localizationDefId)
        {
            if (_gameDefs.Localizations.TryGetValue(localizationDefId, out _) == false)
            {
                var defaultLocalization = _gameDefs.DefaultSettings.LocalizationDefId;
                Debug.LogWarning($"'{localizationDefId}' localization not found, using default: '{defaultLocalization}'");
            }
            _userContext.LocalizationDefId = localizationDefId;

            _localizationDefId.SetValueAndForceNotify(localizationDefId);
        }

        public void CompleteLevel(string levelDefId)
        {
            _userContext.LevelsProgress[levelDefId].IsCompleted = true;

            _updatedLevelDefId.SetValueAndForceNotify(levelDefId);
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