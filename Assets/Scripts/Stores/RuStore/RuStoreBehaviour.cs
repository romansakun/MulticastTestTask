using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameLogic.UI;
using Infrastructure.Services;
using Infrastructure.Services.Leaderboards;
using Stores;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;

namespace RuStore
{
    public class RuStoreBehaviour : StoreBehaviour, IFileService, IStoreAuthLoader, IStoreGRA, IStoreLocalization, IStoreLeaderboards
    {
        private readonly FileService _fileService = new();

        protected override async void Awake()
        {
            base.Awake();
            _viewManager.Views.Subscribe(OnViewsChanged);
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _viewManager.Views.Unsubscribe(OnViewsChanged);
        }

        private void OnViewsChanged(IReadOnlyList<View> views)
        {
            TrySetActiveLeaderboardsButton();
        }

        public bool TryReadAllText(string path, out string content)
        {
            return _fileService.TryReadAllText(path, out content);
        }

        public void WriteAllText(string path, string content)
        {
            _fileService.WriteAllText(path, content);
        }

        public UniTask WaitWhileAuth()
        {
            throw new System.NotImplementedException();
        }

        public UniTask GameReady()
        {
            return UniTask.CompletedTask;
        }

        public void SetLocalization(string lang)
        {
            var localizationDefId = GetLocalizationDefId(lang);
            _userContextOperator?.UpdateLocalization(localizationDefId);
        }

        public async void SetLeaderboard(string lang, int score)
        {
            var playerScores = 0;
            try
            {
                var scoreResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync($"leaderboard_{lang}");
                playerScores = (int)scoreResponse.Score;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            try
            {
                await LeaderboardsService.Instance.AddPlayerScoreAsync($"leaderboard_{lang}", score - playerScores);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public async UniTask<LBData> GetLeaderboard(string lang)
        {
            var result = new LBData();
            try
            {
                var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync($"leaderboard_{lang}");
                result.players = new LBPlayerData[scoresResponse.Results.Count];
                for (var index = 0; index < scoresResponse.Results.Count; index++)
                {
                    var entry = scoresResponse.Results[index];
                    result.players[index] = new LBPlayerData
                    {
                        rank = entry.Rank + 1,
                        name = entry.PlayerName.Split("#")[0],
                        score = (int) entry.Score
                    };
                }

                var scoreResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync($"leaderboard_{lang}");
                result.currentPlayer = new LBCurrentPlayerData
                {
                    rank = scoreResponse.Rank + 1,
                    score = (int) scoreResponse.Score,
                    name = scoreResponse.PlayerName.Split("#")[0],
                };
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return result;
        }
    }
}