using DG.Tweening;
using GameLogic.Ads;
using GameLogic.Factories;
using GameLogic.Model.DataProviders;
using GameLogic.UI.Gameplay;
using GameLogic.UI.Settings;
using Infrastructure;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.MainMenu
{
    public class MainMenuViewModel : ViewModel
    {
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private IAdsShower _adsShower;

        public IReactiveProperty<int> FormedWordCount => _formedWordCount;
        private readonly ReactiveProperty<int> _formedWordCount = new(0);

        private Tween _animation;
        private int _wordsCounter;

        public override void Initialize()
        {
            _userContext.LocalizationDefId.Subscribe(OnLocalizationDefIdChanged);
        }

        private void OnLocalizationDefIdChanged(string defId)
        {
            var count = _userContext.GetAllFormedWordCount();
            _formedWordCount.SetValueAndForceNotify(count);

            _animation?.Kill();
            _animation = DOTween.To(() => _wordsCounter, showingValue =>
            {
                _wordsCounter = showingValue;
                _formedWordCount.SetValueAndForceNotify(showingValue);
            }, count, 1f).SetEase(Ease.OutQuart);
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
            _adsShower.Show(AdSlots.AdRewardSlotID);
        }

        public override void Dispose()
        {
            _animation?.Kill();
            _userContext.LocalizationDefId.Unsubscribe(OnLocalizationDefIdChanged);
            _formedWordCount.Dispose();
        }
    }
}