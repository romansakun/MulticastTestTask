using System;
using Cysharp.Threading.Tasks;
using GameLogic.Audio;
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
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private UserContextOperator _userContextOperator;
        [Inject] private GameActionExecutor _gameActionExecutor;
        [Inject] private GameActionFactory _gameActionFactory;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private AudioPlayer _audioPlayer;

        public override async UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            var levelDefId = context.LevelProgress.LevelDefId;
            var completeLevelGameAction = _gameActionFactory.Create<CompleteLevelGameAction>(levelDefId);
            var validator = completeLevelGameAction.GetValidator();
            try
            {
                if (validator.Check())
                {
                    await _gameActionExecutor.ExecuteAsync(completeLevelGameAction);
                    if (context.IsDisposed) return;

                    _audioPlayer.PlaySound("TapSound");
                    ShowVictoryView();
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
            //var checkingWordsCount = _userContext.CheckingWordsCount.Value;
            var checkingWordsCount = _gameDefs.DefaultSettings.ConsumablesFreeCount;
            var checkingWordsState = ConsumableButtonState.State(checkingWordsCount, _gameDefs.DefaultSettings.ConsumablesFreeCount);
            context.CheckingWordsButtonState.SetValueAndForceNotify(checkingWordsState);
        }

        private async void ShowVictoryView()
        {
            var closing = _viewManager.Close<GameplayView>(false, false);
            var viewModel = _viewModelFactory.Create<VictoryViewModel>();
            var showing = _viewManager.ShowAsync<VictoryView, VictoryViewModel>(viewModel);
            await UniTask.WhenAll(closing, showing);
        }
        
    }
}