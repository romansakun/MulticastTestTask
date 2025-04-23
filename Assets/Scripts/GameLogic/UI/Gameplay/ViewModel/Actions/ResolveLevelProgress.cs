using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameLogic.Factories;
using GameLogic.Model.Actions;
using GameLogic.Model.DataProviders;
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
            if (TryLoadLastLevelProgress(context))
                return;

            if (await TryStartNewNextLevel(context)) 
                return;

            await RestartLevelsProgress();

            if (await TryStartNewNextLevel(context) == false)
            {
                throw new Exception($"Cant load any level");
            }
        }

        private bool TryLoadLastLevelProgress(GameplayViewModelContext context)
        {
            if (_userContext.TryGetLastUncompletedLevelProgress(out var levelProgress))
            {
                var isLevelExist = _gameDefs.Levels.TryGetValue(levelProgress.LevelDefId, out _);
                if (isLevelExist)
                {
                    context.LevelProgress = levelProgress;
                    return true;
                }
            }
            return false;
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