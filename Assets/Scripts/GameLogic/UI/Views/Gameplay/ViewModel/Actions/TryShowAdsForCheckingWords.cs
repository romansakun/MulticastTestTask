using System;
using Cysharp.Threading.Tasks;
using GameLogic.Factories;
using GameLogic.Model.Actions;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Operators;
using GameLogic.UI.Victory;
using Infrastructure.GameActions;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class TryShowAdsForCheckingWords : BaseGameplayViewModelAction
    {
        [Inject] private GameActionExecutor _gameActionExecutor;
        [Inject] private GameActionFactory _gameActionFactory;
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private UserContextOperator _userContextOperator;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;

        public override async UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            var useCheckingWordsGameAction = _gameActionFactory.Create<UseCheckingWordsGameAction>();
            await _gameActionExecutor.ExecuteAsync(useCheckingWordsGameAction);

            var levelDefId = context.LevelProgress.LevelDefId;
            var completeLevelGameAction = _gameActionFactory.Create<CompleteLevelGameAction>(levelDefId);
            var validator = completeLevelGameAction.GetValidator();
            try
            {
                if (validator.Check())
                {
                    await _gameActionExecutor.ExecuteAsync(completeLevelGameAction);

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
            _viewManager.Close<GameplayView>();
            var viewModel = _viewModelFactory.Create<VictoryViewModel>();
            var view = await _viewManager.ShowAsync<VictoryView, VictoryViewModel>(viewModel);
        }

    }
}