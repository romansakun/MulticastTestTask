using GameLogic.Factories;
using GameLogic.UI.Gameplay;
using GameLogic.UI.Settings;
using Zenject;

namespace GameLogic.UI.MainMenu
{
    public class MainMenuViewModel : ViewModel
    {
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;

        public override void Initialize()
        {
           
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
    }
}