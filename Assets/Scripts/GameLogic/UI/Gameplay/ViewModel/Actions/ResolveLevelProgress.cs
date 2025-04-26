using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameLogic.Factories;
using GameLogic.Model.Actions;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Definitions;
using Infrastructure.GameActions;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class ResolveLevelProgress : BaseGameplayViewModelAction
    {
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private GameActionFactory _gameActionFactory;
        [Inject] private GameActionExecutor _gameActionExecutor;

        public override async UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            if (await TryLoadLastLevelProgress(context))
                return;

            if (await TryStartNewNextLevel(context)) 
                return;

            await RestartLevelsProgress();

            if (await TryStartNewNextLevel(context) == false)
            {
                throw new Exception($"Cant load any level");
            }
        }

        private async UniTask<bool> TryLoadLastLevelProgress(GameplayViewModelContext context)
        {
            if (_userContext.TryGetLastUncompletedLevelProgress(out var levelProgress))
            {
                var isLevelExist = _gameDefs.Levels.TryGetValue(levelProgress.LevelDefId, out var levelDef);
                var isLevelProgressValid = IsUserProgressValid(levelProgress, levelDef);
                if (isLevelExist && isLevelProgressValid)
                {
                    context.LevelProgress = levelProgress;
                    return true;
                }
                if (isLevelExist)
                {
                    var gameAction = _gameActionFactory.Create<StartNewLevelGameAction>(levelDef.Id);
                    await _gameActionExecutor.ExecuteAsync(gameAction);
                    await UniTask.Yield();

                    if (_userContext.TryGetLastUncompletedLevelProgress(out levelProgress))
                    {
                        context.LevelProgress = levelProgress;
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsUserProgressValid(LevelProgressContextDataProvider levelProgress, LevelDef levelDef)
        {
            if (levelProgress == null || levelDef == null)
                return false;

            foreach (var rowClusters in levelProgress.DistributedClusters)
            {
                foreach (var userProgressCluster in rowClusters)
                {
                    if (_gameDefs.IsLevelContainsCluster(userProgressCluster, levelDef.Id))
                        continue;

                    return false;
                }
            }

            foreach (var cluster in levelProgress.UndistributedClusters)
            {
                if (_gameDefs.IsLevelContainsCluster(cluster, levelDef.Id))
                    continue;

                return false;
            }
            return true;
        }

        private async UniTask<bool> TryStartNewNextLevel(GameplayViewModelContext context)
        {
            if (_userContext.TryGetNewNextLevelDefId(out var levelDefId) == false) 
                return false;

            var isLevelExist = _gameDefs.Levels.TryGetValue(levelDefId, out _);
            if (isLevelExist == false)
                return false;

            var gameAction = _gameActionFactory.Create<StartNewLevelGameAction>(levelDefId);
            await _gameActionExecutor.ExecuteAsync(gameAction);
            await UniTask.Yield();

            if (_userContext.TryGetLastUncompletedLevelProgress(out var levelProgress))
            {
                context.LevelProgress = levelProgress;
                return true;
            }

            return false;
        }

        private async Task RestartLevelsProgress()
        {
            var gameAction = _gameActionFactory.Create<ClearUserProgressGameAction>();
            await _gameActionExecutor.ExecuteAsync(gameAction);
            await UniTask.Yield();
        }

    }
}