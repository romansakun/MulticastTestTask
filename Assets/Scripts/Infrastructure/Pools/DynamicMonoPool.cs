using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Infrastructure.Pools
{
    public class DynamicMonoPool<T> : IMemoryPool, IDisposable where T : MonoBehaviour, IPoolable<IMemoryPool>
    {
        [Inject] private DiContainer _diContainer;

        private readonly Stack<T> _pool = new();
        private T _prefab;

        public int NumTotal { get; private set; }
        public int NumActive { get; private set;}
        public int NumInactive { get; private set;}
        public Type ItemType { get; private set;} 


        public void InitPool(T prefab, int initialSize)
        {
            _prefab = prefab;
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
            item.OnDespawned();
            _pool.Push(item);
        }

        public void Despawn(T item)
        {
            item.OnDespawned();
            _pool.Push(item);
        }

        public T Spawn()
        {
            if (_prefab == null)
                return null;

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
            _prefab = null;
            _pool.Clear();
        }
    }
}