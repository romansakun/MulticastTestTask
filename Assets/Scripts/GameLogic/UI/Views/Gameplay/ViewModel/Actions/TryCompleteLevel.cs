using System;
using Cysharp.Threading.Tasks;
using GameLogic.Ads;
using GameLogic.Factories;
using GameLogic.Model.Actions;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Operators;
using GameLogic.UI.CenterMessage;
using GameLogic.UI.Victory;
using Infrastructure.Extensions;
using Infrastructure.GameActions;
using Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class TryCompleteLevel : BaseGameplayViewModelAction
    {
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private UserContextOperator _userContextOperator;
        [Inject] private GameActionExecutor _gameActionExecutor;
        [Inject] private GameActionFactory _gameActionFactory;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private IAdsShower _adsShower;
        [Inject] private ITimerService _timerService;

        public override async UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            if (_userContext.CheckingWordsCount.Value <= 0)
            {
                var hasReward = await _adsShower.Show();
                if (context.IsDisposed) return;
                if (hasReward)
                {
                    _userContextOperator.AddCheckingWords();
                    UpdateCheckingWordsButtonState(context);
                }
                else
                {
                    await ShowCenterMessageWithTimer();
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
                UpdateCheckingWordsButtonState(context);
            }
        }

        private void UpdateCheckingWordsButtonState(GameplayViewModelContext context)
        {
            var checkingWordsCount = _userContext.CheckingWordsCount.Value;
            var checkingWordsState = ConsumableButtonState.State(checkingWordsCount, _gameDefs.DefaultSettings.ConsumablesFreeCount);
            context.CheckingWordsButtonState.SetValueAndForceNotify(checkingWordsState);
        }

        private async void ShowVictoryView(GameplayViewModelContext context)
        {
            var closing = _viewManager.Close<GameplayView>(false, false);
            var viewModel = _viewModelFactory.Create<VictoryViewModel>();
            var showing = _viewManager.ShowAsync<VictoryView, VictoryViewModel>(viewModel);
            await UniTask.WhenAll(closing, showing);
        }

        private async UniTask ShowCenterMessageWithTimer()
        {
            var viewModel = _viewModelFactory.Create<CenterMessageViewModel>();
            var duration = _userContext.GetConsumablesUpdateDurationSeconds();
            var timer = _timerService.SetTimer(duration, ts =>
                {
                    var noAdsText = _userContext.GetLocalizedText("NO_ADS_NOW_TRY_LATER");
                    var localizedText = _userContext.GetLocalizedText("FREE_UPDATE_AFTER");
                    viewModel.SetText(ts.HmsD2($"{noAdsText}\n{localizedText}"));
                },
                null, 1000);
            var view = await _viewManager.ShowAsync<CenterMessageView, CenterMessageViewModel>(viewModel);
            await view.ShowAndClose();
            timer.Dispose();
        }
        
    }
}