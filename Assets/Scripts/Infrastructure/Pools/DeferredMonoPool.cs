using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Infrastructure.Pools
{
    public class DeferredMonoPool<T> : IMemoryPool, IDisposable where T : MonoBehaviour, IPoolable<IMemoryPool>
    {
        [Inject] private DiContainer _diContainer;

        private readonly Stack<T> _pool = new();
        private Transform _despawnObjectsParent;
        private T _prefab;
        private bool _isDisposed;

        public int NumTotal { get; private set; }
        public int NumActive { get; private set;}
        public int NumInactive { get; private set;}
        public Type ItemType { get; private set;}


        public DeferredMonoPool()
        {
            Application.quitting += Dispose;
        }

        public void InitPool(T prefab, Transform parent, int initialSize)
        {
            _prefab = prefab ? prefab : throw new ArgumentNullException(nameof(prefab));
            _despawnObjectsParent = parent ? parent : throw new ArgumentNullException(nameof(parent));
            NumTotal = initialSize;
            NumInactive = initialSize;
            NumActive = 0;
            ItemType = typeof(T);

            for (int i = 0; i < initialSize; i++)
            {
                Despawn(CreateNew());
            }
        }

        public void Clear()
        {
            _pool.Clear();
        }

        public void Despawn(object obj)
        {
            T item = (T)obj;
            Despawn(item);
        }

        public void Despawn(T item)
        {
            if (_isDisposed) return;
            if (item == null) return;

            item.OnDespawned();
            if (_despawnObjectsParent && item.transform.parent != _despawnObjectsParent)
                item.transform.SetParent(_despawnObjectsParent, false);

            _pool.Push(item);
        }

        public T Spawn()
        {
            if (_isDisposed) return null;
            if (_prefab == null) return null;

            while (_pool.Count > 0)
            {
                T instance = _pool.Pop();
                if (!instance) 
                    continue;

                instance.OnSpawned(this);
                return instance;
            }
            return CreateNew();
        }

        private T CreateNew()
        {
            if (_isDisposed) return null;
            T instance = _diContainer.InstantiatePrefabForComponent<T>(_prefab);
            instance.OnSpawned(this);
            return instance;
        }

        public void Resize(int desiredPoolSize)
        {

        }

        public void ExpandBy(int numToAdd)
        {
        }

        public void ShrinkBy(int numToRemove)
        {
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            Application.quitting -= Dispose;
            _despawnObjectsParent = null;
            _prefab = null;
            _isDisposed = true;
            _pool?.Clear();
        }
    }
}