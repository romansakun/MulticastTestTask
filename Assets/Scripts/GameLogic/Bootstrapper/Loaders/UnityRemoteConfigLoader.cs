// using System;
// using Cysharp.Threading.Tasks;
// using GameLogic.Model.DataProviders;
// using GameLogic.Model.Definitions;
// using Infrastructure;
// using Newtonsoft.Json.Linq;
// using UnityEngine;
// using Zenject;
//
// namespace GameLogic.Bootstrapper
// {
//     public class UnityRemoteConfigLoader : IAsyncOperation
//     {
//         [Inject] private GameDefsDataProvider _gameDefs;
//
//         private struct UserAttributes {}
//         private struct AppAttributes {}
//
//
//         public async UniTask ProcessAsync()
//         {
//             if (Utilities.CheckForInternetConnection())
//             {
//                 await InitializeRemoteConfigAsync();
//             }
//             else
//             {
//                 return;
//             }
//
//             RuntimeConfig runtimeConfig;
//             try
//             {
//                 runtimeConfig = await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
//             }
//             catch (Exception er)
//             {
//                 Debug.LogError(er.Message);
//                 return;
//             }
//
//             if (runtimeConfig != null && runtimeConfig.config.TryGetValue(nameof(GameDefs), out var gameDefs))
//             { 
//                 TryApplyRemoteSettings(gameDefs);
//             }
//         }
//
//         private async UniTask InitializeRemoteConfigAsync()
//         {
//             await UnityServices.InitializeAsync();
//             if (AuthenticationService.Instance.IsSignedIn == false)
//             {
//                 await AuthenticationService.Instance.SignInAnonymouslyAsync();
//             }
//         }
//
//         private void TryApplyRemoteSettings(JToken gameDefsToken)
//         {
//             var gameDefs = Newtonsoft.Json.JsonConvert.DeserializeObject<GameDefs>(gameDefsToken.ToString());
//             if (gameDefs == null)
//             {
//                 Debug.LogError("Failed to deserialize remote config");
//                 return;
//             }
//
//             _gameDefs.SetGameDefs(gameDefs);
//             Debug.Log("Remote config loaded successfully");
//         }
//
//     }
// }