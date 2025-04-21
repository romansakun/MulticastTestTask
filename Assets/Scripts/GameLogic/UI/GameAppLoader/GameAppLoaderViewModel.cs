using Cysharp.Threading.Tasks;
using DG.Tweening;
using Infrastructure;
using Zenject;

namespace GameLogic.UI.GameAppLoader
{
    public class GameAppLoaderViewModel : ViewModel
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private ViewManager _viewManager;

        private readonly AsyncOperationQueue _loadingQueue = new();
        public IReactiveProperty<string> ProgressText => _progressText;
        private readonly ReactiveProperty<string> _progressText = new();

        private float _showingProgress;
        private Tween _animation;

        public override void Initialize()
        {

        }

        public void AddToLoadingQueue<T>() where T : IAsyncOperation, new()
        {
            _loadingQueue.Add(_diContainer.Instantiate<T>());
        }

        public async void ProcessLoadingQueue()
        {
            _loadingQueue.Progress.Subscribe(OnProgressChanged);

            await _loadingQueue.ProcessAsync();
            while (_animation.IsActive())
            {
                await UniTask.Yield();
            }
            _viewManager.Close<GameAppLoaderView>();
        }

        private void OnProgressChanged(float progressValue)
        {
            _animation?.Kill();
            _animation = DOTween.To(() => _showingProgress, showingValue =>
            {
                _showingProgress = showingValue;
                _progressText.SetValueAndForceNotify($"Loading: {showingValue:P0}");
            }, progressValue, 1f);
        }

        public override void Dispose()
        {
            _loadingQueue.Progress.Unsubscribe(OnProgressChanged);
            _loadingQueue.Dispose();
            _progressText.Dispose();
            _animation?.Kill();
        }
    }
}