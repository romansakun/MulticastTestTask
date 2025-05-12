using System;
using Cysharp.Threading.Tasks;
using GameLogic.Ads;
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
    public class TryCompleteLevel : BaseGameplayViewModelAction
    {
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private UserContextOperator _userContextOperator;
        [Inject] private GameActionExecutor _gameActionExecutor;
        [Inject] private GameActionFactory _gameActionFactory;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private IAdsShower _adsShower;

        public override async UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            if (_userContext.CheckingWordsCount.Value <= 0)
            {
                var hasReward = await _adsShower.Show();
                if (context.IsDisposed) return;
                if (hasReward)
                {
                    _userContextOperator.AddCheckingWords();
                    context.IsCheckingWordsByAdsActive.Value = false;
                    context.CheckingWordsCount.Value = _userContext.CheckingWordsCount.Value;
                }
                return;
            }

            var levelDefId = context.LevelProgress.LevelDefId;
            var completeLevelGameAction = _gameActionFactory.Create<CompleteLevelGameAction>(levelDefId);
            var validator = completeLevelGameAction.GetValidator();
            try
            {
                if (validator.Check())
                {
                    await _gameActionExecutor.ExecuteAsync(completeLevelGameAction);
                    if (context.IsDisposed) return;

                    ShowVictoryView(context);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"CompleteLevelGameAction: validator check failed\n{ex.Message}");
                context.IsFailedCompleteLevel.SetValueAndForceNotify(true);

                if (context.UndistributedClusters.Count <= 0)
                {
                    _userContextOperator.UseCheckingWords();
                }
                context.CheckingWordsCount.Value = _userContext.CheckingWordsCount.Value;
            }

            context.IsCheckingWordsByAdsActive.Value = _userContext.CheckingWordsCount.Value <= 0;
        }

        private async void ShowVictoryView(GameplayViewModelContext context)
        {
            _viewManager.Close<GameplayView>();
            var viewModel = _viewModelFactory.Create<VictoryViewModel>();
            var view = await _viewManager.ShowAsync<VictoryView, VictoryViewModel>(viewModel);
        }

    }
}