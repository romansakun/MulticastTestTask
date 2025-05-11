using System;
using DG.Tweening;
using GameLogic.Ads;
using GameLogic.Factories;
using GameLogic.Model.DataProviders;
using GameLogic.UI.Gameplay;
using GameLogic.UI.Settings;
using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.MainMenu
{
    public class MainMenuViewModel : ViewModel
    {
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private ITimerService _timerService;
        [Inject] private IAdsShower _adsShower;

        public IReactiveProperty<int> CompletedLevelsCount => _completedLevelsCount;
        private readonly ReactiveProperty<int> _completedLevelsCount = new(0);
        public IReactiveProperty<bool> IsLocalizationGameOver => _isLocalizationGameOver;
        private readonly ReactiveProperty<bool> _isLocalizationGameOver = new();
        public IReactiveProperty<bool> IsConsumablesInfoTextVisible => _isConsumablesInfoTextVisible;
        private readonly ReactiveProperty<bool> _isConsumablesInfoTextVisible = new();
        public IReactiveProperty<string> ConsumablesInfoText => _consumablesInfoText;
        private readonly ReactiveProperty<string> _consumablesInfoText = new();

        private IDisposable _timer;
        private Tween _animation;
        private int _wordsCounter;

        public override void Initialize()
        {
            AnimateCompletedLevelsCount();
            SetConsumablesTimer();

            _userContext.LocalizationDefId.Subscribe(OnLocalizationDefIdChanged);
        }

        private void SetConsumablesTimer()
        {
            if (_userContext.CheckingWordsCount.Value > 0)
            {
                _isConsumablesInfoTextVisible.Value = false;
            }
            else
            {
                var duration = _userContext.GetConsumablesUpdateDurationSeconds();
                _timer = _timerService.SetTimer(duration, ts =>
                    {
                        var localizedText = _userContext.GetLocalizedText("CONSUMABLES_INFO_MAIN_MENU");
                        _consumablesInfoText.Value = ts.HmsD2(localizedText);
                    },
                    () =>
                    {
                        _isConsumablesInfoTextVisible.Value = false;
                    },
                    1000);
                _isConsumablesInfoTextVisible.Value = true;
            }
        }

        private void AnimateCompletedLevelsCount()
        {
            var count = _userContext.GetAllCompletedLevels();
            _completedLevelsCount.SetValueAndForceNotify(count);
            _animation?.Kill();
            _animation = DOTween.To(() => _wordsCounter, showingValue =>
            {
                _wordsCounter = showingValue;
                _completedLevelsCount.SetValueAndForceNotify(showingValue);
            }, count, 1f).SetEase(Ease.OutQuart);
        }

        private void OnLocalizationDefIdChanged(string defId)
        {
            _isLocalizationGameOver.Value = _userContext.IsLocalizationLevelsCompleted(defId);
        }

        public async void OnPlayButtonClicked()
        {
            var viewModel = _viewModelFactory.Create<GameplayViewModel>();
            await _viewManager.ShowAsync<GameplayView, GameplayViewModel>(viewModel);
            _viewManager.Close<MainMenuView>();
        }

        public async void OnSettingsButtonClicked()
        {
            var viewModel = _viewModelFactory.Create<SettingsViewModel>();
            await _viewManager.ShowAsync<SettingsView, SettingsViewModel>(viewModel);
        }

        public void OnRewardAdButtonClicked()
        {
            Debug.Log("OnRewardAdButtonClicked");
            //_adsShower.Show(AdSlots.HintSlotID);
        }

        public override void Dispose()
        {
            _animation?.Kill();
            _timer?.Dispose();
            _consumablesInfoText.Dispose();
            _completedLevelsCount.Dispose();
            _isLocalizationGameOver.Dispose();
            _isConsumablesInfoTextVisible.Dispose();

            _userContext.LocalizationDefId.Unsubscribe(OnLocalizationDefIdChanged);
        }

    }
}