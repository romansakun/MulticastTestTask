using System;
using Cysharp.Threading.Tasks;
using GameLogic.Factories;
using GameLogic.Model.Actions;
using GameLogic.Model.DataProviders;
using GameLogic.UI.Victory;
using Infrastructure.GameActions;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class TryCompleteLevel : BaseGameplayViewModelAction
    {
        [Inject] private GameActionExecutor _gameActionExecutor;
        [Inject] private GameActionFactory _gameActionFactory;
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;

        public override async UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            var levelDefId = context.LevelProgress.LevelDefId;
            var gameAction = _gameActionFactory.Create<CompleteLevelGameAction>(levelDefId);
            var validator = gameAction.GetValidator();
            try
            {
                if (validator.Check())
                {
                    await _gameActionExecutor.ExecuteAsync(gameAction);

                    ShowVictoryView(context);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"CompleteLevelGameAction: validator check failed\n{ex.Message}");

                context.IsFailedCompleteLevel.SetValueAndForceNotify(true);
            }
        }

        private async void ShowVictoryView(GameplayViewModelContext context)
        {
            var viewModel = _viewModelFactory.Create<VictoryViewModel>();
            var view = await _viewManager.ShowAsync<VictoryView, VictoryViewModel>(viewModel);
        }

    }
}