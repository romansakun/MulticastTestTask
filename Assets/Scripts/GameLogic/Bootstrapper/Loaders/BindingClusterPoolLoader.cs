using Cysharp.Threading.Tasks;
using GameLogic.UI.Gameplay;
using Infrastructure;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class BindingClusterPoolLoader : IAsyncOperation
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private AssetLoader _assetLoader;

        public async UniTask ProcessAsync()
        {
            var clusterPrefab = await _assetLoader.LoadPrefabAsync<Cluster>();

            _diContainer.BindMemoryPool<Cluster, Cluster.Pool>()
                .WithInitialSize(16)
                .FromComponentInNewPrefab(clusterPrefab)
                .AsSingle();
        }

    }
}