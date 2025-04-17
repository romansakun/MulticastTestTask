using Cysharp.Threading.Tasks;
using GameLogic.UI;
using Infrastructure;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class GameAppReloader
    {
        //todo add other disposable dependencies
        
        [Inject] private DiContainer _container;
        [Inject] private AssetLoader _assetLoader;
        [Inject] private ViewManager _viewManager;

        public async void ReloadGame()
        {
            _assetLoader.Dispose();
            _viewManager.Dispose();
            _container.UnbindAll();

            var scene = SceneManager.GetActiveScene();
            await Resources.UnloadUnusedAssets();

            SceneManager.LoadScene(scene.name);
        }
    }
}