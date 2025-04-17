using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using Object = UnityEngine.Object;

namespace Infrastructure
{
    public class AssetLoader : IDisposable
    {
        private readonly Dictionary<(string name, Type type), AsyncOperationHandle> _objectOperationHandles = new();
        private readonly Dictionary<(string name, Type type), UniTask> _objectOperationTasks = new();
        private readonly Dictionary<(string name, Type type), Exception> _errors = new();

        private bool _isInitialized = false;

        private async UniTask InitializeAddressables()
        {
            if (_isInitialized) return;
            try
            {
                await Addressables.InitializeAsync().Task;
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AssetLoader] Addressables initialization failed: {ex.Message}");
                throw;
            }
        }

        public async UniTask<T> LoadPrefabAsync<T> (string name = null) where T : MonoBehaviour
        {
            var prefabName = name ?? typeof(T).Name;
            var gameObject = await GetOrLoadAsync<T>(prefabName);
            if (typeof(T) == typeof(GameObject))
                return gameObject;

            var component = gameObject.GetComponent<T>();
            if (component == null)
                throw new ArgumentException($"Unable to load prefab from {prefabName}. It don't contains {typeof(T)}");

            return component;
        }

        public async UniTask<T> LoadAsync<T> (string name) where T : Object
        {
            return await GetOrLoadAsync<T>(name);
        }

        public async UniTask<Sprite> LoadSpriteAsync (string name, string spriteAtlasName = null)
        {
            if (string.IsNullOrEmpty(spriteAtlasName) == false)
            {
                var spriteAtlas = await GetOrLoadAsync<SpriteAtlas>(spriteAtlasName);
                return spriteAtlas.GetSprite(name);
            }
            var sprite = await GetOrLoadAsync<Sprite>(name);
            if (sprite == null)
            {
                throw new ArgumentException($"Sprite '{name}' not found in atlas '{spriteAtlasName}'");
            }
            return sprite;
        }

        private async UniTask<T> GetOrLoadAsync<T>(string name)  where T : Object
        {
            await InitializeAddressables();

            var key = (name, typeof(T));
            if (_errors.TryGetValue(key, out var cachedError))
            {
                throw cachedError;
            }

            try
            {
                if (_objectOperationTasks.TryGetValue(key, out var task))
                {
                    await task;
                }
                if (_objectOperationHandles.TryGetValue(key, out var handle))
                {
                    if (handle.Result == null)
                        throw new Exception($"[AssetLoader] Asset {name} was loaded but Result is null");

                    return handle.Result as T;
                }

                var loadAssetAsync = Addressables.LoadAssetAsync<T>(name);
                var uniTask = loadAssetAsync.Task.AsUniTask();
                _objectOperationTasks.TryAdd(key, uniTask);
                await uniTask;

                if (loadAssetAsync.Status != AsyncOperationStatus.Succeeded )
                {
                    throw new Exception($"[AssetLoader] Failed to load {typeof(T).Name} '{name}': {loadAssetAsync.Status}, exception: {loadAssetAsync.OperationException}");
                }
                if (loadAssetAsync.Result == null)
                {
                    throw new Exception($"[AssetLoader] Failed to load {typeof(T).Name} '{name}': Result is null");
                }

                _objectOperationHandles.TryAdd(key, loadAssetAsync);
                _objectOperationTasks.Remove(key);

                return loadAssetAsync.Result;
            }
            catch (Exception ex)
            {
                var error = new Exception($"[AssetLoader] Failed to load {typeof(T).Name} '{name}': {ex.Message}");
                _errors.TryAdd(key, error);
                throw error;
            }
        }

        public void Dispose()
        {
            foreach (var pair in _objectOperationHandles)
            {
                Addressables.Release(pair.Value);
            }
            _objectOperationHandles.Clear();
            _objectOperationTasks.Clear();
            _errors.Clear();
        }

    }
}