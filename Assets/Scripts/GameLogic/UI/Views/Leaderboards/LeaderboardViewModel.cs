using System.Collections.Generic;
using GameLogic.Model.DataProviders;
using GameLogic.UI.MainMenu;
using Infrastructure;
using Infrastructure.Services;
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

        public IReactiveProperty<bool> IsLeaderboardLoaded => _isLeaderboardLoaded;
        private readonly ReactiveProperty<bool> _isLeaderboardLoaded = new(false);

        private readonly List<PlayerLine> _playerLines = new();

        public override void Initialize()
        {
        }

        public async void CreatePlayerLines(RectTransform playersContainer, RectTransform myPlayerContainer)
        {
            var lang = _gameDefs.Localizations[_userContext.LocalizationDefId.Value].Description;
            var leaderboardData = await _yandexLeaderboards.GetLeaderboard(lang);
            if (leaderboardData == null || playersContainer == null || myPlayerContainer == null)
                return;

            _isLeaderboardLoaded.Value = true;
            if (leaderboardData.currentPlayer != null)
            {
                var userRank = leaderboardData.currentPlayer.rank;
                foreach (var playerData in leaderboardData.players)
                {
                    var playerLine = _playerLineFactory.Create();
                    if (playerData.rank == userRank)
                        playerLine.UpdateEntries(leaderboardData.currentPlayer);
                    else
                        playerLine.UpdateEntries(playerData);

                    playerLine.transform.SetParent(playersContainer, false);
                    _playerLines.Add(playerLine);
                }
            }

            var userLine = _playerLineFactory.Create();
            userLine.UpdateEntries(leaderboardData.currentPlayer);
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

        public override void Dispose()
        {
            _playerLines.ForEach(p => p.Dispose());
        }
    }
}