using GameLogic.Factories;
using GameLogic.Model.DataProviders;
using GameLogic.UI.Gameplay;
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

        public IReactiveProperty<int> FormedWordCount => _formedWordCount;
        private readonly ReactiveProperty<int> _formedWordCount = new(0);

        public override void Initialize()
        {
            _userContext.LocalizationDefId.Subscribe(OnLocalizationDefIdChanged);
        }

        private void OnLocalizationDefIdChanged(string defId)
        {
            var count = _userContext.GetAllFormedWordCount();
            _formedWordCount.SetValueAndForceNotify(count);
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

        public override void Dispose()
        {
            _userContext.LocalizationDefId.Unsubscribe(OnLocalizationDefIdChanged);
            _formedWordCount.Dispose();
        }
    }
}