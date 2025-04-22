using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class GameAppReloader
    {
        [Inject] private DiContainer _container;

        public async void ReloadGame()
        {
            _container.UnbindAll();

            var scene = SceneManager.GetActiveScene();
            await Resources.UnloadUnusedAssets();

            SceneManager.LoadScene(scene.name);
        }
    }
}