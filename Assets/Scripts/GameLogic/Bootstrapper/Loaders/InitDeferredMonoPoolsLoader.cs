using Cysharp.Threading.Tasks;
using GameLogic.UI.Gameplay;
using GameLogic.UI.Leaderboards;
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
        [Inject] private DeferredMonoPool<PlayerLine> _playerLinePool;

        public async UniTask ProcessAsync()
        {
            var clusterPrefab = await _assetsLoader.LoadPrefabAsync<Cluster>();
            var guessWordPrefab = await _assetsLoader.LoadPrefabAsync<WordRow>();
            var playerLinePool = await _assetsLoader.LoadPrefabAsync<PlayerLine>();
            var canvasTransform = _canvas.transform;

            _wordRowPool.InitPool(guessWordPrefab, canvasTransform, 4);
            _clusterPool.InitPool(clusterPrefab, canvasTransform, 8);
            _playerLinePool.InitPool(playerLinePool, canvasTransform, 50);
        }

    }
}