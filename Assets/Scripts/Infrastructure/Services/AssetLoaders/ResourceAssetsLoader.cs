using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Infrastructure.Services
{
    public class ResourceAssetsLoader : IAssetsLoader
    {
        public async UniTask<T> LoadPrefabAsync<T>(string name = null) where T : MonoBehaviour
        {
            var prefabName = name ?? typeof(T).Name;
            var path = $"Prefabs/{prefabName}";
            var request = Resources.LoadAsync<T>(path);
            while (request.isDone == false)
            {
                await UniTask.Yield();
            }
            return request.asset as T;
        }

        public UniTask<T> LoadAsync<T>(string name) where T : Object
        {
            throw new NotImplementedException();
        }

        public UniTask<Sprite> LoadSpriteAsync(string name, string spriteAtlasName = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}