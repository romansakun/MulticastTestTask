using GameLogic.Factories;
using GameLogic.Model.DataProviders;
using GameLogic.UI.Gameplay;
using GameLogic.UI.HowToPlayHint;
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

        // public IReactiveProperty<int> CompletedLevelsCount => _completedLevelsCount;
        // private readonly ReactiveProperty<int> _completedLevelsCount = new(0);
        public IReactiveProperty<bool> IsLocalizationGameOver => _isLocalizationGameOver;
        private readonly ReactiveProperty<bool> _isLocalizationGameOver = new();

        public IReactiveProperty<Sprite> LeagueWreathIcon => _leagueWreathIcon;
        private readonly ReactiveProperty<Sprite> _leagueWreathIcon = new(); 
        public IReactiveProperty<Sprite> LeagueRomanNumberIcon => _leagueRomanNumberIcon;
        private readonly ReactiveProperty<Sprite> _leagueRomanNumberIcon = new();
        public IReactiveProperty<string> LeagueLevelsLabel => _leagueLevelsLabel;
        private readonly ReactiveProperty<string> _leagueLevelsLabel = new();

        //private Tween _animation;
        //private int _wordsCounter;

        public override void Initialize()
        {
            //AnimateCompletedLevelsCount();
            InitLeague();

            _userContext.LocalizationDefId.Subscribe(OnLocalizationDefIdChanged);
        }

        private async void InitLeague()
        {
            var leagues = _gameDefs.GetLocalizationLeagues(_userContext.LocalizationDefId.Value);
            var currentLeagueDef = leagues[^1];
            var currentLevelDefId = string.Empty;
            if (_userContext.TryGetLastCompletedLevelProgress(out var lastCompletedLevelProgress))
            {
                currentLevelDefId = lastCompletedLevelProgress.LevelDefId;
            }
            if (_userContext.TryGetLastUncompletedLevelProgress(out var levelProgress))
            {
                currentLeagueDef = leagues.Find(x => x.Levels.Contains(levelProgress.LevelDefId));
                currentLevelDefId = levelProgress.LevelDefId;
            }
            else if (_userContext.TryGetNewNextLevelDefId(out var nextLevelDefId))
            {
                currentLeagueDef = leagues.Find(x => x.Levels.Contains(nextLevelDefId));
                currentLevelDefId = nextLevelDefId;
            }

            var allLeagueLevelsCount = currentLeagueDef.Levels.Count;
            var currentLevelNumber = currentLeagueDef.Levels.IndexOf(currentLevelDefId) + 1;
            var wreathIcon = await _assetsLoader.LoadSpriteAsync($"{currentLeagueDef.WreathIcon}");
            var romanNumberIcon = await _assetsLoader.LoadSpriteAsync($"{currentLeagueDef.RomanNumberIcon}");

            _leagueWreathIcon.Value = wreathIcon;
            _leagueRomanNumberIcon.Value = romanNumberIcon;
            _leagueLevelsLabel.Value = $"{currentLevelNumber}/{allLeagueLevelsCount}";
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

        public override void Dispose()
        {
            //_animation?.Kill();
            //_completedLevelsCount.Dispose();
            _isLocalizationGameOver.Dispose();
            _leagueWreathIcon.Dispose();
            _leagueRomanNumberIcon.Dispose();
            _leagueLevelsLabel.Dispose();

            _userContext.LocalizationDefId.Unsubscribe(OnLocalizationDefIdChanged);
        }
    }
}