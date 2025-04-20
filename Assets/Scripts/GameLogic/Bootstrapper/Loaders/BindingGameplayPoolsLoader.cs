using Cysharp.Threading.Tasks;
using GameLogic.UI.Gameplay;
using Infrastructure;
using Infrastructure.Services;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class BindingGameplayPoolsLoader : IAsyncOperation
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private AssetLoader _assetLoader;

        public async UniTask ProcessAsync()
        {
            var clusterPrefab = await _assetLoader.LoadPrefabAsync<Cluster>();
            var dummyClusterPrefab = await _assetLoader.LoadPrefabAsync<DummyCluster>();
            var guessWordPrefab = await _assetLoader.LoadPrefabAsync<GuessWord>();

            _diContainer.BindMemoryPool<Cluster, Cluster.Pool>()
                .WithInitialSize(16)
                .FromComponentInNewPrefab(clusterPrefab)
                .AsCached();
            _diContainer.BindMemoryPool<DummyCluster, DummyCluster.Pool>()
                .WithInitialSize(16)
                .FromComponentInNewPrefab(dummyClusterPrefab)
                .AsCached();
            _diContainer.BindMemoryPool<GuessWord, GuessWord.Pool>()
                .WithInitialSize(4)
                .FromComponentInNewPrefab(guessWordPrefab)
                .AsCached();
        }

    }
}