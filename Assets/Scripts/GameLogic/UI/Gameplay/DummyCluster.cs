using System;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class DummyCluster : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        [SerializeField] private RectTransform _rectTransform;

        public RectTransform RectTransform => _rectTransform;

        private IMemoryPool _memoryPool;

        public bool IsCloserToRightEdge(Vector2 point)
        {
            return point.x > _rectTransform.anchoredPosition.x;
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
            if (this) _memoryPool?.Despawn(this);
        }

        public class Pool : MonoPoolableMemoryPool<IMemoryPool, DummyCluster>
        {
        }
    }
}