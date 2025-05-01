using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Infrastructure.Services
{
    public interface IAssetsLoader: IDisposable
    {
        public UniTask<T> LoadPrefabAsync<T>(string name = null) where T : MonoBehaviour;
        public UniTask<T> LoadAsync<T>(string name) where T : Object;
        public UniTask<Sprite> LoadSpriteAsync(string name, string spriteAtlasName = null);
    }
}