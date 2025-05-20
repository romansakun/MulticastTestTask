using Cysharp.Threading.Tasks;
using GameLogic.UI.Gameplay;
using Infrastructure;
using Infrastructure.Pools;
using Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class InitDeferredMonoPoolsLoader : IAsyncOperation
    {
        [Inject] private Canvas _canvas;
        [Inject] private IAssetsLoader _assetsLoader;
        [Inject] private DeferredMonoPool<WordRow> _wordRowPool;
        [Inject] private DeferredMonoPool<Cluster> _clusterPool;

        public async UniTask ProcessAsync()
        {
            var clusterPrefab = await _assetsLoader.LoadPrefabAsync<Cluster>();
            var guessWordPrefab = await _assetsLoader.LoadPrefabAsync<WordRow>();
            var canvasTransform = _canvas.transform;

            _wordRowPool.InitPool(guessWordPrefab, canvasTransform, 4);
            _clusterPool.InitPool(clusterPrefab, canvasTransform, 8);
        }

    }
}