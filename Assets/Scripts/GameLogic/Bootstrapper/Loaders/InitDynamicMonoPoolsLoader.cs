using Cysharp.Threading.Tasks;
using GameLogic.UI.Gameplay;
using Infrastructure;
using Infrastructure.Pools;
using Infrastructure.Services;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class InitDynamicMonoPoolsLoader : IAsyncOperation
    {
        [Inject] private AssetLoader _assetLoader;
        [Inject] private DeferredMonoPool<WordRow> _wordRowPool;
        [Inject] private DeferredMonoPool<Cluster> _clusterPool;

        public async UniTask ProcessAsync()
        {
            var clusterPrefab = await _assetLoader.LoadPrefabAsync<Cluster>();
            var guessWordPrefab = await _assetLoader.LoadPrefabAsync<WordRow>();

            _wordRowPool.InitPool(guessWordPrefab, 4);
            _clusterPool.InitPool(clusterPrefab, 8);
        }

    }
}