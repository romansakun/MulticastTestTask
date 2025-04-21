using Cysharp.Threading.Tasks;
using GameLogic.UI.Gameplay;
using Infrastructure;
using Infrastructure.Services;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class BindingGameplayFactoriesLoader : IAsyncOperation
    {
        [Inject] private DiContainer _diContainer;
        [Inject] private AssetLoader _assetLoader;

        public async UniTask ProcessAsync()
        {
            var clusterPrefab = await _assetLoader.LoadPrefabAsync<Cluster>();
            var guessWordPrefab = await _assetLoader.LoadPrefabAsync<WordRow>();

            _diContainer.BindFactory<Cluster, Cluster.Factory>()
                .FromPoolableMemoryPool(pool => pool
                    .WithInitialSize(8)
                    .FromComponentInNewPrefab(clusterPrefab));
            _diContainer.BindFactory<WordRow, WordRow.Factory>()
                .FromPoolableMemoryPool(pool => pool
                    .WithInitialSize(4)
                    .FromComponentInNewPrefab(guessWordPrefab));

        }

    }
}