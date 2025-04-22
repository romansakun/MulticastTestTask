using System.Collections.Generic;
using System.Text;
using GameLogic.Bootstrapper;
using GameLogic.Factories;
using GameLogic.Model.Actions;
using GameLogic.Model.DataProviders;
using GameLogic.UI.Gameplay;
using GameLogic.UI.MainMenu;
using Infrastructure.GameActions;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.Victory
{
    public class VictoryViewModel : ViewModel
    {
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private WordRow.Factory _wordRowFactory;
        [Inject] private Cluster.Factory _clusterFactory;
        [Inject] private ColorsSettings _colorsSettings;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private GameActionFactory _gameActionFactory;
        [Inject] private GameActionExecutor _gameActionExecutor;

        private readonly List<WordRow> _wordRows = new();
        private readonly List<Cluster> _clusters = new();

        public override void Initialize()
        {

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
                cluster.SetParent(wordRow.ClustersHolder);
                cluster.SetBackgroundColor(_colorsSettings.SelectedClusterBackColor);
                cluster.SetTextColor(_colorsSettings.SelectedClusterTextColor);
                
                _wordRows.Add(wordRow);
                _clusters.Add(cluster);
            }
        }

        public async void OnNextLevelButtonClicked()
        {
            if (_userContext.TryGetNewNextLevelDefId(out _) == false)
            {
                var gameAction = _gameActionFactory.Create<ClearUserContextGameAction>();
                await _gameActionExecutor.ExecuteAsync(gameAction);
            }

            _viewManager.CloseAll();
            var viewModel = _viewModelFactory.Create<GameplayViewModel>();
            var view = _viewManager.ShowAsync<GameplayView, GameplayViewModel>(viewModel);
        }

        public async void OnMainMenuButtonClicked()
        {
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
        }

    }
}