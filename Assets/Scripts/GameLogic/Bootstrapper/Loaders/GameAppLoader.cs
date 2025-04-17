using Infrastructure;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class GameAppLoaderView : MonoBehaviour
    {
        [Inject] private DiContainer _diContainer;

        [SerializeField] private TextMeshProUGUI _loadingText;

        private AsyncOperationQueue _loadingQueue;

        private void Awake()
        {
            _loadingQueue = new AsyncOperationQueue();
            _loadingQueue.Progress.SubscribeToText(_loadingText, p => $"Loading... {p:P0}");
        }

        public async void Start()
        {
            _loadingQueue.Add(_diContainer.Instantiate<UnityRemoteConfigLoader>());

            await _loadingQueue.ProcessAsync();
        }

        private void OnDestroy()
        {
            _loadingQueue.Dispose();
        }
    }
}