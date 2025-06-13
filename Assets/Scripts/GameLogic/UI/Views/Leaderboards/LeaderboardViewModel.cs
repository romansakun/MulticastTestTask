using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameLogic.GptChats;
using GameLogic.Model.DataProviders;
using GameLogic.UI.MainMenu;
using Infrastructure;
using Infrastructure.Services;
using Infrastructure.Services.Yandex.Leaderboards;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.Leaderboards
{
    public class LeaderboardViewModel : ViewModel
    {
        [Inject] private ViewManager _viewManager;
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private IYandexLeaderboards _yandexLeaderboards;
        [Inject] private PlayerLine.Factory _playerLineFactory;
        [Inject] private IGptChat _gptChat;

        public IReactiveProperty<bool> IsLeaderboardLoaded => _isLeaderboardLoaded;
        private readonly ReactiveProperty<bool> _isLeaderboardLoaded = new(false);

        public IReactiveProperty<string> DescriptionText => _descriptionText;
        private readonly ReactiveProperty<string> _descriptionText = new();

        private readonly List<PlayerLine> _playerLines = new();
        private LBData _leaderboardData;

        private static readonly Dictionary<string, Dictionary<int, string>> _gptAnswers = new();

        public override void Initialize()
        {
        }

        public async UniTask CreatePlayerLines(RectTransform playersContainer, RectTransform myPlayerContainer)
        {
            var lang = _gameDefs.Localizations[_userContext.LocalizationDefId.Value].Description;
            _leaderboardData = await _yandexLeaderboards.GetLeaderboard(lang);
            if (_leaderboardData == null || playersContainer == null || myPlayerContainer == null)
            {
                TryAddFakePlayerLines(playersContainer);
            }
            else
            {
                CreateRealPlayerLines(playersContainer);
                TryAddFakePlayerLines(playersContainer);
                CreateUserLine(myPlayerContainer);
            }

            await UniTask.Yield();
            _isLeaderboardLoaded.Value = true;
        }

        private void TryAddFakePlayerLines(RectTransform playersContainer)
        {
            for (int i = _playerLines.Count; i < 10; i++)
            {
                var playerLine = _playerLineFactory.Create();
                playerLine.transform.SetParent(playersContainer, false);
                playerLine.UpdateEntries(new LBPlayerData()
                {
                    rank = i + 1,
                    name = $"Player {i+1}",
                    score = 10,
                });
                _playerLines.Add(playerLine);
            }
        }

        private void CreateRealPlayerLines(RectTransform playersContainer)
        {
            if (_leaderboardData.currentPlayer != null)
            {
                var userRank = _leaderboardData.currentPlayer.rank;
                foreach (var playerData in _leaderboardData.players)
                {
                    var playerLine = _playerLineFactory.Create();
                    if (playerData.rank == userRank)
                        playerLine.UpdateEntries(_leaderboardData.currentPlayer);
                    else
                        playerLine.UpdateEntries(playerData);

                    playerLine.transform.SetParent(playersContainer, false);
                    _playerLines.Add(playerLine);
                }
            }
        }

        private void CreateUserLine(RectTransform myPlayerContainer)
        {
            var userLine = _playerLineFactory.Create();
            userLine.UpdateEntries(_leaderboardData.currentPlayer);
            userLine.transform.SetParent(myPlayerContainer, false);
            _playerLines.Add(userLine);
        }

        public async void OnCloseButtonClicked()
        {
            if (_viewManager.TryGetView<MainMenuView> (out var view))
            {
                view.gameObject.SetActive(true);
            }
            await _viewManager.Close<LeaderboardView>(false, false);
        }

        public async UniTask TrySetGeminiComment()
        {
            if (_leaderboardData == null || _leaderboardData.currentPlayer == null)
            {
                _descriptionText.Value = _userContext.GetLocalizedText("LEADERBOARD_LOADING_FAILED");
                return;
            }
            
            var resultText = string.Empty;
            var rank = _leaderboardData.currentPlayer.rank;
            var local = _userContext.LocalizationDefId.Value;
            if (_gptAnswers.ContainsKey(local) == false)
            {
                _gptAnswers[local] = new Dictionary<int, string>();
            }
            if (_gptAnswers[local].ContainsKey(rank))
            {
                resultText = _gptAnswers[local][rank];
            }
            else
            {
                var prompt = _userContext.GetLocalizedText("LEADERBOARD_PROMPT", rank);
                resultText = await _gptChat.Ask(prompt);
                if (_descriptionText.IsDisposed)
                    return;

                _gptAnswers[local][rank] = resultText;
            }

            _descriptionText.Value = string.IsNullOrEmpty(resultText)
                ? _userContext.GetLocalizedText("LEADERBOARD_DEFAULT") 
                : resultText;
        }

        public override void Dispose()
        {
            _playerLines.ForEach(p => p.Dispose());
            _descriptionText.Dispose();
            _isLeaderboardLoaded.Dispose();
        }
    }
}