using System.Collections.Generic;
using System.Text;
using GameLogic.Factories;
using GameLogic.GptChats;
using GameLogic.Model.DataProviders;
using GameLogic.UI.Gameplay;
using GameLogic.UI.MainMenu;
using Infrastructure;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.Victory
{
    public class VictoryViewModel : ViewModel
    {
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private WordRow.Factory _wordRowFactory;
        [Inject] private Cluster.Factory _clusterFactory;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private IGptChat _gptChat;

        private readonly List<WordRow> _wordRows = new();
        private readonly List<Cluster> _clusters = new();

        public IReactiveProperty<bool> VisibleNextLevelButton => _visibleNextLevelButton;
        private readonly ReactiveProperty<bool> _visibleNextLevelButton = new(true);

        public IReactiveProperty<string> CongratulationsText => _congratulationsText;
        private readonly ReactiveProperty<string> _congratulationsText = new();

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
            
            TrySetCongratulationsText();
        }

        private async void TrySetCongratulationsText()
        {
            var sb = new StringBuilder();
            sb.Append("\"");
            _clusters.ForEach(c => sb.Append($" {c.GetText()} "));
            sb.Append("\"");

            var prompt = _userContext.GetLocalizedText("VICTORY_PROMPT", sb.ToString());
            var congratulationsText = await _gptChat.Ask(prompt);

            if (string.IsNullOrEmpty(congratulationsText) == false)
            {
                _congratulationsText.Value = congratulationsText;
            }
        }

        public async void OnNextLevelButtonClicked()
        {
            _viewManager.Close<VictoryView>();
            var viewModel = _viewModelFactory.Create<GameplayViewModel>();
            var view = await _viewManager.ShowAsync<GameplayView, GameplayViewModel>(viewModel);
        }

        public async void OnMainMenuButtonClicked()
        {   
            _viewManager.Close<VictoryView>();
            var viewModel = _viewModelFactory.Create<MainMenuViewModel>();
            var view = await _viewManager.ShowAsync<MainMenuView, MainMenuViewModel>(viewModel);
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
        }

    }
}