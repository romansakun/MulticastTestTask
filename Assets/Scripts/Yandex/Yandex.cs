using Cysharp.Threading.Tasks;
using GameLogic.Model.Contexts;
using Infrastructure.Services;
using Newtonsoft.Json;
using UnityEngine;

namespace YG
{
    public class Yandex : MonoBehaviour, IFileService, IYandexAuthLoader, IYandexGRA
    {
        public bool TryReadAllText(string path, out string content)
        {
            content = JsonConvert.SerializeObject(YG2.saves.userContext);
            return string.IsNullOrEmpty(content) == false;
        }

        public void WriteAllText(string path, string content)
        {
            YG2.saves.userContext = JsonConvert.DeserializeObject<UserContext>(content);;
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
}