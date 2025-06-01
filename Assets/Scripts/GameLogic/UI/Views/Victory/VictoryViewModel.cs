using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using GameLogic.Factories;
using GameLogic.GptChats;
using GameLogic.Helpers;
using GameLogic.Model.DataProviders;
using GameLogic.UI.Gameplay;
using GameLogic.UI.MainMenu;
using Infrastructure;
using Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.Victory
{
    public class VictoryViewModel : ViewModel
    {
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private UserContextRatingHelper _ratingHelper;
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private WordRow.Factory _wordRowFactory;
        [Inject] private Cluster.Factory _clusterFactory;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private IGptChat _gptChat;
        [Inject] private IYandexLeaderboards _yandexLeaderboards;

        private readonly List<WordRow> _wordRows = new();
        private readonly List<Cluster> _clusters = new();

        public IReactiveProperty<bool> VisibleNextLevelButton => _visibleNextLevelButton;
        private readonly ReactiveProperty<bool> _visibleNextLevelButton = new(true);

        public IReactiveProperty<string> CongratulationsText => _congratulationsText;
        private readonly ReactiveProperty<string> _congratulationsText = new();

        public IReactiveProperty<string> ScoreText => _scoreText;
        private readonly ReactiveProperty<string> _scoreText = new();

        public override void Initialize()
        {
            _visibleNextLevelButton.Value = _userContext.IsCurrentLocalizationLevelsCompleted() == false;
        }

        public void LoadResolvedWords(RectTransform wordsHolder)
        {
            if (_userContext.TryGetLastCompletedLevelProgress(out var lastLevelProgress) == false)
                return;

            var sb = new StringBuilder();
            for (var i = 0; i < lastLevelProgress.DistributedClusters.Count; i++)
            {
                var wordRow = _wordRowFactory.Create();
                wordRow.SetParent(wordsHolder);

                var clusters = lastLevelProgress.DistributedClusters[i];
                sb.Clear();
                foreach (var clusterText in clusters)
                {
                    sb.Append(clusterText);
                }
                var cluster = _clusterFactory.Create();
                cluster.SetText(sb.ToString());
                wordRow.SetClusterAsChild(cluster);
                cluster.SetColorAlpha(1);

                _wordRows.Add(wordRow);
                _clusters.Add(cluster);
            }
            
            TrySetCongratulationsText(lastLevelProgress);
            SetRating(lastLevelProgress);
        }

        private async void TrySetCongratulationsText(LevelProgressContextDataProvider levelProgress)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            _clusters.ForEach(c => sb.Append($" {c.GetText()} "));
            sb.Append("]");
            
            var levelDefId = levelProgress.LevelDefId;
            var levelNumber = _gameDefs.GetLevelNumber(levelDefId, _userContext.LocalizationDefId.Value);

            var prompt = _userContext.GetLocalizedText("VICTORY_PROMPT", sb.ToString(), levelNumber);
            var congratulationsText = await _gptChat.Ask(prompt);
            if (_congratulationsText.IsDisposed)
                return;

            if (string.IsNullOrEmpty(congratulationsText) == false)
            {
                _congratulationsText.Value = congratulationsText;
            }
        }

        private void SetRating(LevelProgressContextDataProvider levelProgress)
        {
            var localizationDefId = _userContext.LocalizationDefId.Value;
            var score = _ratingHelper.AddLevelScore(localizationDefId, levelProgress);

            _scoreText.Value = $"+{score}";

            var lang = _gameDefs.Localizations[localizationDefId].Description;
            _yandexLeaderboards.SetLeaderboard(lang, _ratingHelper.GetRating(localizationDefId));
        }

        public async void OnNextLevelButtonClicked()
        {
            var closing = _viewManager.Close<VictoryView>(false, false);
            var viewModel = _viewModelFactory.Create<GameplayViewModel>();
            var showing = _viewManager.ShowAsync<GameplayView, GameplayViewModel>(viewModel);
            await UniTask.WhenAll(closing, showing);
        }

        public async void OnMainMenuButtonClicked()
        {
            var viewModel = _viewModelFactory.Create<MainMenuViewModel>();
            var view = await _viewManager.ShowAsync<MainMenuView, MainMenuViewModel>(viewModel);
            await _viewManager.Close<VictoryView>(false, false);
        }

        public override void Dispose()
        {
            foreach (var wordRow in _wordRows)
            {
                wordRow.Dispose();
            }
            foreach (var cluster in _clusters)
            {
                cluster.Dispose();
            }
            _wordRows.Clear();
            _clusters.Clear();
            _visibleNextLevelButton.Dispose();
            _congratulationsText.Dispose();
        }

    }
}