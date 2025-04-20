using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class GuessWord : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        [Inject] private DummyCluster.Pool _dummyClusterPool;

        [SerializeField] private Image _backImage;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _clustersHolder;
        [SerializeField] private RectTransform _dummyClustersHolder;

        private readonly List<DummyCluster> _dummyClusters = new();
        private readonly List<Cluster> _clusters = new();

        private IMemoryPool _memoryPool;


        public DummyCluster AddDummyToSuitablePosition(Vector2 point)
        {
            var dummyCluster = _dummyClusterPool.Spawn(_dummyClusterPool);
            var sublimeIndex = 0;
            if (_dummyClusters.Count > 0)
            {
                var closestDummyCluster = _dummyClusters.GetClosest(point);
                sublimeIndex += closestDummyCluster.IsCloserToRightEdge(point) ? 1 : 0;
            }

            _dummyClusters.Add(dummyCluster);
            dummyCluster.SetParent(_dummyClustersHolder);
            dummyCluster.RectTransform.SetSiblingIndex(sublimeIndex);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_dummyClustersHolder);

            return dummyCluster;
        }

        public void SetBackgroundColor(Color color)
        {
            _backImage.color = color;
        }

        public void SetPosition(Vector2 position)
        {
            _rectTransform.anchoredPosition = position;
        }

        public void SetParent(Transform parent)
        {
            _rectTransform.SetParent(parent, false);
        }

        public void OnSpawned(IMemoryPool memoryPool)
        {
            _memoryPool = memoryPool;
        }

        public void OnDespawned()
        {
            _memoryPool = null;
        }

        public void Dispose()
        {
            foreach (var dummyCluster in _dummyClusters)
            {
                dummyCluster.Dispose();
            }
            foreach (var cluster in _clusters)
            {
                cluster.Dispose();
            }
            if (this) _memoryPool?.Despawn(this);
        }

        public class Pool : MonoPoolableMemoryPool<IMemoryPool, GuessWord>
        {
        }
    }
}