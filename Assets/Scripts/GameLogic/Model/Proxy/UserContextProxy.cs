using GameLogic.Model.Contexts;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Infrastructure.Services;
using Newtonsoft.Json;
using UniRx;
using Zenject;

namespace GameLogic.Model.Proxy
{
    public class UserContextProxy : IInitializable, IDisposable
    {
        [Inject] private IFileService _fileService;

        public IReactiveProperty<string> CurrentLocalizationDefId => _currentLocalizationDefId;
        public IReactiveProperty<(string localizationDefId, int levelId)> CurrentLocalizationLevelId => _currentLocalizationLevelId;
        public IReactiveProperty<Dictionary<string, LevelProgressContext>> LevelsProgress => _levelsProgress;

        private readonly ReactiveProperty<string> _currentLocalizationDefId = new();
        private readonly ReactiveProperty<(string localizationDefId, int levelId)> _currentLocalizationLevelId = new();
        private readonly ReactiveProperty<Dictionary<string, LevelProgressContext>> _levelsProgress = new();

        private UserContext _userContext;
        private bool _willSave = false;


        public void Initialize()
        {
            if (TryLoadingPlayerContext() == false)
            {
                CreateNewPlayerContext();
            }
        }

        public void UpdateLocalization(string localizationDefId)
        {
            _userContext.LocalizationDefId = localizationDefId;
            _currentLocalizationDefId.SetValueAndForceNotify(localizationDefId);
            Save();
        }

        public void UpdateLocalizationLevelId(string localizationDefId, int levelId)
        {
            _userContext.LocalizationsCurrentLevels[localizationDefId] = levelId;
            _currentLocalizationLevelId.SetValueAndForceNotify((localizationDefId, levelId));
            Save();
        }

        public bool TryDistributeCluster(string levelDefId, string word, int undistributedIndex, int distributedIndex)
        {
            if (_userContext.LevelsProgress.TryGetValue(levelDefId, out var needLevel) == false)
                return false;
            if (needLevel.UndistributedClustersDefIds.Count <= undistributedIndex)
                return false;
            if (needLevel.DistributedClustersDefIds.ContainsKey(word) == false)
                return false;
            if (needLevel.DistributedClustersDefIds[word].Count < distributedIndex)
                return false;

            var needCluster = needLevel.UndistributedClustersDefIds[undistributedIndex];
            var needWordClusters = needLevel.DistributedClustersDefIds[word];

            needWordClusters.Insert(distributedIndex, needCluster);
            needLevel.UndistributedClustersDefIds.RemoveAt(undistributedIndex);
            //_levelsProgress.SetValueAndForceNotify(needLevel);

            Save();
            return true;
        }

        private bool TryLoadingPlayerContext()
        {
            if (_fileService.TryReadAllText(GamePaths.PlayerContext, out var json) == false)
                return false;

            _userContext = JsonConvert.DeserializeObject<UserContext>(json);
            return true;
        }

        private void CreateNewPlayerContext()
        {
            _userContext = new UserContext();
            Save();
        }

        private async void Save()
        {
            if (_willSave) 
                return;

            _willSave = true;
            await UniTask.Yield();
            _willSave = false;

            var json = JsonConvert.SerializeObject(_userContext, Formatting.Indented);
            _fileService.WriteAllText(GamePaths.PlayerContext, json);

            //UnityEngine.Debug.Log($"PlayerContextPath: {GamePaths.PlayerContext}\nContext:{json}");
        }

        public void Dispose()
        {
            _levelsProgress.Dispose();
            _currentLocalizationDefId.Dispose();
            _currentLocalizationLevelId.Dispose();
        }

    }
}