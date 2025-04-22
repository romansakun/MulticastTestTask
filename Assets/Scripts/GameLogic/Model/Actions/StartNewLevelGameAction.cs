using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameLogic.Extensions;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Definitions;
using GameLogic.Model.Operators;
using Infrastructure.GameActions;
using Zenject;

namespace GameLogic.Model.Actions
{
    public class StartNewLevelGameAction : IGameAction  
    {
        [Inject] private UserContextOperator _userContextOperator;
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private DiContainer _diContainer;

        private readonly string _levelDefId;

        public StartNewLevelGameAction(string levelDefId)
        {
            _levelDefId = levelDefId;
        }

        public UniTask ExecuteAsync()
        {
             var needLevelDef = _gameDefs.Levels[_levelDefId];

             var undistributedClusters = CreateUndistributedClusters(needLevelDef);
             var distributedClusters = CreateDistributedClusters(needLevelDef);
             _userContextOperator.AddOrUpdateLevelProgress(_levelDefId, undistributedClusters, distributedClusters);

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
            return _diContainer.Instantiate<Validator>(new object[] { _levelDefId });
        }

        private class Validator : IValidator
        {
            [Inject] private UserContextDataProvider _userContext;
            [Inject] private GameDefsDataProvider _gameDefs;

            private readonly string _levelDefId;

            public Validator(string levelDefId)
            {
                _levelDefId = levelDefId;
            }

            public bool Check()
            {
                var localizationDefId = _userContext.LocalizationDefId.Value;
                if (_gameDefs.TryGetPreviousLevelDef(_levelDefId, localizationDefId, out var prevLevelDef))
                {
                    if (_userContext.IsLevelCompleted(prevLevelDef.Id) == false)
                        throw new Exception($"[{nameof(StartNewLevelGameAction)}] previous level '{prevLevelDef.Id}' is not completed!");
                }

                if (_userContext.IsLevelProgressExist(_levelDefId))
                    throw new Exception($"[{nameof(StartNewLevelGameAction)}] level '{_levelDefId}' is already started!");

                return true;
            }

        }
    }
}