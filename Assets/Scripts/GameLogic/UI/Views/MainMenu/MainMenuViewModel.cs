using DG.Tweening;
using GameLogic.Factories;
using GameLogic.Model.DataProviders;
using GameLogic.UI.Gameplay;
using GameLogic.UI.HowToPlayHint;
using GameLogic.UI.Settings;
using Infrastructure;
using Zenject;

namespace GameLogic.UI.MainMenu
{
    public class MainMenuViewModel : ViewModel
    {
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private UserContextDataProvider _userContext;

        public IReactiveProperty<int> CompletedLevelsCount => _completedLevelsCount;
        private readonly ReactiveProperty<int> _completedLevelsCount = new(0);
        public IReactiveProperty<bool> IsLocalizationGameOver => _isLocalizationGameOver;
        private readonly ReactiveProperty<bool> _isLocalizationGameOver = new();

        private Tween _animation;
        private int _wordsCounter;

        public override void Initialize()
        {
            AnimateCompletedLevelsCount();

            _userContext.LocalizationDefId.Subscribe(OnLocalizationDefIdChanged);
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

        public async void OnHelpButtonClicked()
        {
            var viewModel = _viewModelFactory.Create<HowToPlayHintViewModel>();
            await _viewManager.ShowAsync<HowToPlayHintView, HowToPlayHintViewModel>(viewModel);
        }

        public override void Dispose()
        {
            _animation?.Kill();
            _completedLevelsCount.Dispose();
            _isLocalizationGameOver.Dispose();

            _userContext.LocalizationDefId.Unsubscribe(OnLocalizationDefIdChanged);
        }
    }
}