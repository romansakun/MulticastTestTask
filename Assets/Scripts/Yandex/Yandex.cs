using Cysharp.Threading.Tasks;
using Infrastructure.Services;
using UnityEngine;

namespace YG
{
    public class Yandex : MonoBehaviour, IFileService, IYandexAuthLoader, IYandexGRA
    {
        public bool TryReadAllText(string path, out string content)
        {
            content = YG2.saves.userData;
            return true;
        }

        public void WriteAllText(string path, string content)
        {
            YG2.saves.userData = content;
            YG2.SaveProgress();
        }

        public async UniTask WaitWhileAuth()
        {
            YG2.OpenAuthDialog();
            while (YG2.player.auth == false)
            {
                await UniTask.Yield();
            }
        }

        public UniTask GameReady()
        {
            YG2.GameReadyAPI();

            return UniTask.CompletedTask;
        }
    }

    public partial class SavesYG
    {
        public string userData;
    }

}