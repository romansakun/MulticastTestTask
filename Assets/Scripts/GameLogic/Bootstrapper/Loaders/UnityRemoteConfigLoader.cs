using Cysharp.Threading.Tasks;
using GameLogic.Model.Definitions;
using GameLogic.Model.Proxy;
using Infrastructure;
using Newtonsoft.Json.Linq;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class UnityRemoteConfigLoader : IAsyncOperation
    {
        [Inject] private GameDefsProxy _gameDefs;

        private struct UserAttributes {}
        private struct AppAttributes {}


        public async UniTask ProcessAsync()
        {
            if (Utilities.CheckForInternetConnection())
            {
                await InitializeRemoteConfigAsync();
            }
            var runtimeConfig = await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
            if (runtimeConfig.config.TryGetValue(nameof(GameDefs), out var gameDefs) && TryApplyRemoteSettings(gameDefs))
            {
                Debug.Log("Remote config loaded successfully");
            }
        }

        private async UniTask InitializeRemoteConfigAsync()
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        private bool TryApplyRemoteSettings(JToken gameDefsToken)
        {
            var gameDefs = Newtonsoft.Json.JsonConvert.DeserializeObject<GameDefs>(gameDefsToken.ToString());
            if (gameDefs == null)
            {
                Debug.LogError("Failed to deserialize remote config");
                return false;
            }

            _gameDefs.SetGameDefs(gameDefs);
            return true;
        }
    }
}