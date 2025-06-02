using System;
using Cysharp.Threading.Tasks;
using GameLogic.Bootstrapper;
using GameLogic.Model.Contexts;
using GameLogic.Model.Operators;
using Infrastructure.Services;
using Newtonsoft.Json;
using UnityEngine;
using YG.Utils.LB;
using Zenject;

namespace YG
{
    public class Yandex : MonoBehaviour, IFileService, IYandexAuthLoader, IYandexGRA, IYandexLocalization, IYandexLeaderboards
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private DiContainer _diContainer;
        [Inject] private GameAppReloader _reloader;

        private UserContextOperator _userContextOperator;

        private void Awake()
        {
            _signalBus.Subscribe<UserContextInitializedSignal>(OnUserContextInitialized);
            YG2.onCorrectLang += OnCorrectLanguage;
            YG2.onSwitchLang += OnLanguageChanged;
            YG2.onGetLeaderboard += OnGetLeaderboard;
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<UserContextInitializedSignal>(OnUserContextInitialized);
            YG2.onCorrectLang -= OnCorrectLanguage;
            YG2.onSwitchLang -= OnLanguageChanged;
            YG2.onGetLeaderboard -= OnGetLeaderboard;
        }

        private void OnUserContextInitialized(UserContextInitializedSignal signal)
        {
            _userContextOperator = _diContainer.Resolve<UserContextOperator>();
            OnLanguageChanged(YG2.lang);
        }

        private void OnGetLeaderboard(LBData leaderboardData)
        {
            _leaderboardData = leaderboardData;
        }

        public bool TryReadAllText(string path, out string content)
        {
            content = JsonConvert.SerializeObject(YG2.saves.userContext);
            return string.IsNullOrEmpty(content) == false;
        }

        public void WriteAllText(string path, string content)
        {
            YG2.saves.userContext = JsonConvert.DeserializeObject<UserContext>(content);
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

        public void SetLocalization(string lang)
        {
            YG2.lang = lang;
            OnLanguageChanged(lang);
        }

        private void OnCorrectLanguage(string lang)
        {
            if (lang != "ru" && lang != "en")
            {
                YG2.lang = "en";
            }
        }

        private void OnLanguageChanged(string lang)
        {
            var localizationDefId = GetLocalizationDefId(lang);
            _userContextOperator?.UpdateLocalization(localizationDefId);
        }

        private string GetLocalizationDefId(string lang)
        {
            switch (lang)
            {
                case "ru": return SystemLanguage.Russian.ToString();
                default: return SystemLanguage.English.ToString();
            }
        }

        private LBData _leaderboardData;

        public void SetLeaderboard(string lang, int score)
        {
            var tableId = GetLeaderboardId(lang);
            YG2.SetLeaderboard(tableId, score);
        }

        public async UniTask<Infrastructure.Services.Yandex.Leaderboards.LBData> GetLeaderboard(string lang)
        {
            _leaderboardData = null;
            var tableId = GetLeaderboardId(lang);
            YG2.GetLeaderboard(tableId);
            while (_leaderboardData == null)
            {
                await UniTask.Yield();
            }
            return ConvertLeaderboardData(_leaderboardData);
        }

        private static string GetLeaderboardId(string lang)
        {
#if UNITY_EDITOR
            return "test";
#else
            return lang  == "ru" ? "RuBestPlayers" : "EnBestPlayers";
#endif
        }

        private Infrastructure.Services.Yandex.Leaderboards.LBData ConvertLeaderboardData(LBData leaderboardData)
        {
            var json = JsonConvert.SerializeObject(leaderboardData);
            var data = JsonConvert.DeserializeObject<Infrastructure.Services.Yandex.Leaderboards.LBData>(json);
            if (data.currentPlayer != null)
            {
                data.currentPlayer.name = YG2.player.name;
            }
            return data;
        }

        [ContextMenu( "ClearProgress")]
        private void ClearProgress()
        {
            YG2.saves.userContext = null;
            YG2.SaveProgress();

            _reloader.ReloadGame();
        }
    }
}