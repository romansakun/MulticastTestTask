using Cysharp.Threading.Tasks;
using GameLogic.Audio;
using GameLogic.Factories;
using GameLogic.UI;
using GameLogic.UI.MainMenu;
using Infrastructure;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class ShowMainMenuLoader : IAsyncOperation
    {
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private SoundsSettings _soundsSettings;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;

        public async UniTask ProcessAsync()
        {
            await ShowMainMenu();
        }

        private async UniTask ShowMainMenu()
        {
            var viewModel = _viewModelFactory.Create<MainMenuViewModel>();
            var view = await _viewManager.ShowAsync<MainMenuView, MainMenuViewModel>(viewModel);
        }
    }
}