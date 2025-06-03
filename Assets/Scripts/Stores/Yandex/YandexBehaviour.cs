// using System.Collections.Generic;
// using Cysharp.Threading.Tasks;
// using GameLogic.Bootstrapper;
// using GameLogic.Model.Contexts;
// using GameLogic.Model.DataProviders;
// using GameLogic.Model.Operators;
// using GameLogic.UI;
// using GameLogic.UI.MainMenu;
// using Infrastructure.Services;
// using Newtonsoft.Json;
// using Stores;
// using UnityEngine;
// using UnityEngine.UI;
// using YG.Utils.LB;
// using Zenject;
//
// namespace YG
// {
//     public class YandexBehaviour : StoreBehaviour, IFileService, IStoreAuthLoader, IStoreGRA, IStoreLocalization, IStoreLeaderboards
//     {
//         [Inject] private GameAppReloader _reloader;
//         [Inject] private Canvas _canvas;
//
//
//         protected override void Awake()
//         {
//             base.Awake();
//             _viewManager.Views.Subscribe(OnViewsChanged);
//             YG2.onCorrectLang += OnCorrectLanguage;
//             YG2.onSwitchLang += OnLanguageChanged;
//             YG2.onGetLeaderboard += OnGetLeaderboard;
//
//             var canvasScaler = _canvas.GetComponent<CanvasScaler>();
//             canvasScaler.matchWidthOrHeight = .9f;
//         }
//
//         protected override void OnDestroy()
//         {
//             base.OnDestroy();
//             _viewManager.Views.Unsubscribe(OnViewsChanged);
//             YG2.onCorrectLang -= OnCorrectLanguage;
//             YG2.onSwitchLang -= OnLanguageChanged;
//             YG2.onGetLeaderboard -= OnGetLeaderboard;
//         }
//
//         private void OnViewsChanged(IReadOnlyList<View> views)
//         {
//             TrySetActiveLeaderboardsButton();
//         }
//
//         protected override void OnUserContextInitialized()
//         {
//             OnLanguageChanged(YG2.lang);
//         }
//
//         private void OnGetLeaderboard(LBData leaderboardData)
//         {
//             _leaderboardData = leaderboardData;
//         }
//
//         public bool TryReadAllText(string path, out string content)
//         {
//             content = JsonConvert.SerializeObject(YG2.saves.userContext);
//             return string.IsNullOrEmpty(content) == false;
//         }
//
//         public void WriteAllText(string path, string content)
//         {
//             YG2.saves.userContext = JsonConvert.DeserializeObject<UserContext>(content);
//             YG2.SaveProgress();
//         }
//
//         public async UniTask WaitWhileAuth()
//         {
//             YG2.OpenAuthDialog();
//             while (YG2.player.auth == false)
//             {
//                 await UniTask.Yield();
//             }
//         }
//
//         public UniTask GameReady()
//         {
//             YG2.GameReadyAPI();
//
//             return UniTask.CompletedTask;
//         }
//
//         public void SetLocalization(string lang)
//         {
//             YG2.lang = lang;
//             OnLanguageChanged(lang);
//         }
//
//         private void OnCorrectLanguage(string lang)
//         {
//             if (lang != "ru" && lang != "en")
//             {
//                 YG2.lang = "en";
//             }
//         }
//
//         private void OnLanguageChanged(string lang)
//         {
//             var localizationDefId = GetLocalizationDefId(lang);
//             _userContextOperator?.UpdateLocalization(localizationDefId);
//         }
//
//         private LBData _leaderboardData;
//
//         public void SetLeaderboard(string lang, int score)
//         {
//             var tableId = GetLeaderboardId(lang);
//             YG2.SetLeaderboard(tableId, score);
//         }
//
//         public async UniTask<Infrastructure.Services.Leaderboards.LBData> GetLeaderboard(string lang)
//         {
//             _leaderboardData = null;
//             var tableId = GetLeaderboardId(lang);
//             YG2.GetLeaderboard(tableId);
//             while (_leaderboardData == null)
//             {
//                 await UniTask.Yield();
//             }
//             return ConvertLeaderboardData(_leaderboardData);
//         }
//
//         private static string GetLeaderboardId(string lang)
//         {
// // #if UNITY_EDITOR
// //             return "test";
// // #else
//             return lang  == "ru" ? "RuBestPlayers" : "EnBestPlayers";
// //#endif
//         }
//
//         private Infrastructure.Services.Leaderboards.LBData ConvertLeaderboardData(LBData leaderboardData)
//         {
//             var json = JsonConvert.SerializeObject(leaderboardData);
//             var data = JsonConvert.DeserializeObject<Infrastructure.Services.Leaderboards.LBData>(json);
//             if (data.currentPlayer != null)
//             {
//                 data.currentPlayer.name = YG2.player.name;
//             }
//             return data;
//         }
//
//         [ContextMenu( "ClearProgress")]
//         private void ClearProgress()
//         {
//             YG2.saves.userContext = null;
//             YG2.SaveProgress();
//
//             _reloader.ReloadGame();
//         }
//     }
// }