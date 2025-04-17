using Cysharp.Threading.Tasks;
using Infrastructure;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

namespace GameLogic.Bootstrapper
{
    public class UnityRemoteConfigLoader : IAsyncOperation 
    {
        private struct UserAttributes {}
        private struct AppAttributes {}


        public async UniTask ProcessAsync()
        {
            if (Utilities.CheckForInternetConnection())
            {
                await InitializeRemoteConfigAsync();
            }
            //RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
            var runtimeConfig = await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
            ApplyRemoteSettings(runtimeConfig);
        }

        private async UniTask InitializeRemoteConfigAsync()
        {
            // initialize handlers for unity game services
            await UnityServices.InitializeAsync();

            // remote config requires authentication for managing environment information
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }

        void ApplyRemoteSettings(RuntimeConfig runtimeConfig)
        {
            runtimeConfig.config.TryGetValue("Definitions", out var definintions);
            if (definintions == null)
            {
                Debug.Log("Definitions not found in remote config");
                return;
            }
            
            Debug.Log("RemoteConfigService.Instance.appConfig fetched: " + RemoteConfigService.Instance.appConfig.config.ToString());
            Debug.Log("RemoteConfigService.Instance.appConfig fetched: " + runtimeConfig.config.ToString());
        }
    }

}