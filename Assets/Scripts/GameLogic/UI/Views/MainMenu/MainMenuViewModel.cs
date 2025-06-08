using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameLogic.Factories;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Definitions;
using GameLogic.UI.Gameplay;
using GameLogic.UI.Leaderboards;
using GameLogic.UI.Settings;
using Infrastructure;
using Infrastructure.Services;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.MainMenu
{
    public class MainMenuViewModel : ViewModel
    {
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private IAssetsLoader _assetsLoader;

        public IReactiveProperty<bool> IsLocalizationGameOver => _isLocalizationGameOver;
        private readonly ReactiveProperty<bool> _isLocalizationGameOver = new();

        public IReactiveProperty<Sprite> LeagueWreathIcon => _leagueWreathIcon;
        private readonly ReactiveProperty<Sprite> _leagueWreathIcon = new(); 
        public IReactiveProperty<Sprite> LeagueRomanNumberIcon => _leagueRomanNumberIcon;
        private readonly ReactiveProperty<Sprite> _leagueRomanNumberIcon = new();
        public IReactiveProperty<bool> LeftLeagueButtonVisible => _leftLeagueButtonVisible;
        private readonly ReactiveProperty<bool> _leftLeagueButtonVisible = new(true);
        public IReactiveProperty<bool> RightLeagueButtonVisible => _rightLeagueButtonVisible;
        private readonly ReactiveProperty<bool> _rightLeagueButtonVisible = new(true);

        public IReactiveProperty<string> LeagueLevelsLabel => _leagueLevelsLabel;
        private readonly ReactiveProperty<string> _leagueLevelsLabel = new();

        private string _currentLevelDefId;
        private LeagueDef _currentLeagueDef;
        private LeagueDef _currentShowedLeagueDef;

        public override void Initialize()
        {
            //AnimateCompletedLevelsCount();
            InitLeague();

            _userContext.LocalizationDefId.Subscribe(OnLocalizationDefIdChanged);
        }

        private async void InitLeague()
        {
            var leagues = _gameDefs.GetLocalizationLeagues(_userContext.LocalizationDefId.Value);
            _currentLeagueDef = leagues[^1];
            _currentLevelDefId = string.Empty;
            if (_userContext.TryGetLastCompletedLevelProgress(out var lastCompletedLevelProgress))
            {
                _currentLevelDefId = lastCompletedLevelProgress.LevelDefId;
            }
            if (_userContext.TryGetLastUncompletedLevelProgress(out var levelProgress))
            {
                _currentLeagueDef = leagues.Find(x => x.Levels.Contains(levelProgress.LevelDefId));
                _currentLevelDefId = levelProgress.LevelDefId;
            }
            else if (_userContext.TryGetNewNextLevelDefId(out var nextLevelDefId))
            {
                _currentLeagueDef = leagues.Find(x => x.Levels.Contains(nextLevelDefId));
                _currentLevelDefId = nextLevelDefId;
            }

            await ShowLeagueDef(_currentLeagueDef);
        }

        private async UniTask ShowLeagueDef(LeagueDef leagueDef)
        {
            var allLeagueLevelsCount = leagueDef.Levels.Count;
            var currentLevelNumber = leagueDef.Levels.IndexOf(_currentLevelDefId) + 1;
            if (currentLevelNumber <= 0)
                currentLevelNumber = leagueDef.Levels.Count;
            var wreathIcon = await _assetsLoader.LoadSpriteAsync($"{leagueDef.WreathIcon}");
            var romanNumberIcon = await _assetsLoader.LoadSpriteAsync($"{leagueDef.RomanNumberIcon}");
            var leagues = _gameDefs.GetLocalizationLeagues(_userContext.LocalizationDefId.Value);

            _leagueWreathIcon.Value = wreathIcon;
            _leagueRomanNumberIcon.Value = romanNumberIcon;
            _leagueLevelsLabel.Value = $"{currentLevelNumber}/{allLeagueLevelsCount}";
            _leftLeagueButtonVisible.Value = leagues.IndexOf(leagueDef) > 0;
            _rightLeagueButtonVisible.Value = leagues.IndexOf(leagueDef) < leagues.IndexOf(_currentLeagueDef);
            _currentShowedLeagueDef = leagueDef;
        }

        // private void AnimateCompletedLevelsCount()
        // {
        //     var count = _userContext.GetAllCompletedLevels();
        //     _completedLevelsCount.SetValueAndForceNotify(count);
        //     _animation?.Kill();
        //     _animation = DOTween.To(() => _wordsCounter, showingValue =>
        //     {
        //         _wordsCounter = showingValue;
        //         _completedLevelsCount.SetValueAndForceNotify(showingValue);
        //     }, count, 1f).SetEase(Ease.OutQuart);
        // }

        private void OnLocalizationDefIdChanged(string defId)
        {
            InitLeague();
            _isLocalizationGameOver.Value = _userContext.IsLocalizationLevelsCompleted(defId);
        }

        public async void OnPlayButtonClicked()
        {
            var viewModel = _viewModelFactory.Create<GameplayViewModel>();
            await _viewManager.ShowAsync<GameplayView, GameplayViewModel>(viewModel);
            await _viewManager.Close<MainMenuView>();
        }

        public async void OnSettingsButtonClicked()
        {
            _viewManager.TryGetView<MainMenuView>(out var view);
            view.gameObject.SetActive(false);
            var viewModel = _viewModelFactory.Create<SettingsViewModel>();
            await _viewManager.ShowAsync<SettingsView, SettingsViewModel>(viewModel);
        }

        public async void OnLeaderboardsButtonClicked()
        {
            _viewManager.TryGetView<MainMenuView>(out var view);
            view.gameObject.SetActive(false);
            var viewModel = _viewModelFactory.Create<LeaderboardViewModel>();
            await _viewManager.ShowAsync<LeaderboardView, LeaderboardViewModel>(viewModel);
        }

        public async void OnLeftLeagueButtonClicked()
        {
            var leagues = _gameDefs.GetLocalizationLeagues(_userContext.LocalizationDefId.Value);
            var index = leagues.IndexOf(_currentShowedLeagueDef);
            if (index > 0)
            {
                await ShowLeagueDef(leagues[index-1]);
            }
        }

        public async void OnRightLeagueButtonClicked()
        {
            var leagues = _gameDefs.GetLocalizationLeagues(_userContext.LocalizationDefId.Value);
            var index = leagues.IndexOf(_currentShowedLeagueDef);
            if (index < leagues.Count-1)
            {
                await ShowLeagueDef(leagues[index+1]);
            }
        }

        public override void Dispose()
        {
            //_animation?.Kill();
            //_completedLevelsCount.Dispose();
            _isLocalizationGameOver.Dispose();
            _leagueWreathIcon.Dispose();
            _leagueRomanNumberIcon.Dispose();
            _leagueLevelsLabel.Dispose();
            _leftLeagueButtonVisible.Dispose();
            _rightLeagueButtonVisible.Dispose();

            _userContext.LocalizationDefId.Unsubscribe(OnLocalizationDefIdChanged);
        }

    }
}