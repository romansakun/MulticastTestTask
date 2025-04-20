using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Extensions;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Definitions;
using GameLogic.Model.Operators;
using Infrastructure.GameActions;
using Zenject;

namespace GameLogic.Model.Actions
{
    public class StartNewLevelGameAction : IGameAction  
    {
        [Inject] private UserContextDataProvider _userContextProvider;
        [Inject] private UserContextOperator _userContextOperator;
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private DiContainer _diContainer;

        private readonly int _levelId;

        public StartNewLevelGameAction(int levelId)
        {
            _levelId = levelId;
        }

        public UniTask ExecuteAsync()
        {
             var currentLocalizationDefId = _userContextProvider.LocalizationDefId.Value;
             var currentLocalizationDef = _gameDefs.Localizations[currentLocalizationDefId];
             var needLevelDefId = currentLocalizationDef.Levels[_levelId];
             var needLevelDef = _gameDefs.Levels[needLevelDefId];

             var undistributedClusters = CreateUndistributedClusters(needLevelDef);
             var distributedClusters = CreateDistributedClusters(needLevelDef);
             _userContextOperator.AddOrUpdateLevelProgress(needLevelDefId, undistributedClusters, distributedClusters);

            return UniTask.CompletedTask;
        }

        private List<string> CreateUndistributedClusters(LevelDef needLevelDef)
        {
            var undistributedClusters = new List<string>();
            foreach (var pair in needLevelDef.Words)
            {
                var word = pair.Key;
                var clustersLengths = pair.Value;
                var startIndex = 0;
                for (var i = 0; i < clustersLengths.Count; i++)
                {
                    var clusterLength = clustersLengths[i];
                    var cluster = word.Substring(startIndex, clusterLength);
                    undistributedClusters.Add(cluster);
                    startIndex += clusterLength;
                }
            }
            undistributedClusters.Shuffle();

            return undistributedClusters;
        }

        private List<List<string>> CreateDistributedClusters(LevelDef levelDef)
        {
            var distributedClusters = new List<List<string>>();
            foreach (var pair in levelDef.Words)
            {
                distributedClusters.Add(new List<string>(4));
            }
            return distributedClusters;
        }

        public IValidator GetValidator()
        {
            return _diContainer.Instantiate<Validator>(new object[] { _levelId });
        }

        private class Validator : IValidator
        {
            [Inject] private UserContextDataProvider _userContext;
            [Inject] private GameDefsDataProvider _gameDefs;

            private readonly int _levelId;

            public Validator(int levelId)
            {
                _levelId = levelId;
            }

            public bool Check()
            {
                var localizationDefId = _userContext.LocalizationDefId.Value;
                if (string.IsNullOrEmpty(localizationDefId))
                    throw new Exception($"[{nameof(StartNewLevelGameAction)}] UserContext.LocalizationDefId is null or empty!");
                
                var currentLocalizationDef = _gameDefs.Localizations[localizationDefId];
                if (currentLocalizationDef.Levels.TryGetValue(_levelId, out var levelDefId) == false)
                    throw new Exception($"[{nameof(StartNewLevelGameAction)}] {localizationDefId} doesn't contain level key: {_levelId}!");

                if (_gameDefs.TryGetPreviousLevelDef(levelDefId, localizationDefId, out var prevLevelDef))
                {
                    if (_userContext.IsLevelCompleted(prevLevelDef.Id) == false)
                        throw new Exception($"[{nameof(StartNewLevelGameAction)}] previous level '{prevLevelDef.Id}' is not completed!");
                }
                
                if (_userContext.IsLevelProgressExist(levelDefId))
                    throw new Exception($"[{nameof(StartNewLevelGameAction)}] level '{levelDefId}' is already started!");

                return true;
            }

        }
    }
}