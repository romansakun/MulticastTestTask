using System;
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
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private GameActionFactory _gameActionFactory;
        [Inject] private GameActionExecutor _gameActionExecutor;

        public override async UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            if (_userContext.TryGetNewNextLevelDefId(out _) == false)
            {
                var gameAction = _gameActionFactory.Create<ClearUserProgressGameAction>();
                await _gameActionExecutor.ExecuteAsync(gameAction);
            }

            if (_userContext.TryGetLastUncompletedLevelProgress(out var levelProgress))
            {
                context.LevelProgress = levelProgress;
            }
            else if (_userContext.TryGetNewNextLevelDefId(out var levelDefId))
            {
                var gameAction = _gameActionFactory.Create<StartNewLevelGameAction>(levelDefId);
                await _gameActionExecutor.ExecuteAsync(gameAction);
                if (context.IsDisposed) return;

                if (_userContext.TryGetLastUncompletedLevelProgress(out levelProgress))
                    context.LevelProgress = levelProgress;
            }
            else
            {
                var gameAction = _gameActionFactory.Create<ClearUserProgressGameAction>();
                await _gameActionExecutor.ExecuteAsync(gameAction);
            }

            await UniTask.Yield();
        }
    }
}