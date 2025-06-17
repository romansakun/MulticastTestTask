using Cysharp.Threading.Tasks;
using GameLogic.UI;
using GameLogic.UI.Background;
using Infrastructure;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class ShowBackgroundViewLoader : IAsyncOperation
    {
        [Inject] private ViewManager _viewManager;

        public async UniTask ProcessAsync()
        {
            await _viewManager.ShowAsync<BackgroundView, ViewModel>(null);
        }

    }
}