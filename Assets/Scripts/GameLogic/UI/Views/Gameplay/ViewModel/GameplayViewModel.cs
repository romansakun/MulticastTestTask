using Cysharp.Threading.Tasks;
using GameLogic.Bootstrapper;
using GameLogic.Factories;
using GameLogic.Model.DataProviders;
using GameLogic.UI.MainMenu;
using Infrastructure;
using Infrastructure.LogicUtility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class GameplayViewModel : ViewModel
    {
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private LogicBuilderFactory _logicBuilderFactory;
        [Inject] private GameAppReloader _gameAppReloader;
        [Inject] private ViewModelFactory _viewModelFactory;
        [Inject] private ViewManager _viewManager;

        public IReactiveProperty<float> UndistributedClustersScrollRectNormalizedPosition => _logicAgent.Context.UndistributedClustersScrollRectNormalizedPosition;
        public IReactiveProperty<bool> IsUndistributedClustersScrollRectActive => _logicAgent.Context.IsUndistributedClustersScrollRectActive;
        public IReactiveProperty<bool> IsHintClusterInUndistributedClusters => _logicAgent.Context.IsHintClusterInUndistributedClusters;
        public IReactiveProperty<bool> IsFailedCompleteLevel => _logicAgent.Context.IsFailedCompleteLevel;
        public IReactiveProperty<string> LevelNameText => _levelNameText;
        public IReactiveProperty<string> DescriptionLevelText => _descriptionLevelText;

        private readonly ReactiveProperty<string> _levelNameText = new();
        private readonly ReactiveProperty<string> _descriptionLevelText = new();

        private LogicAgent<GameplayViewModelContext> _logicAgent;

        public override void Initialize()
        {
            var logicBuilder = _logicBuilderFactory.Create<GameplayViewModelContext>();

            var loadLevelAction = logicBuilder
                .AddAction<ResolveLevelProgress>()
                .JoinAction<LoadWordRows>()
                .JoinAction<LoadWordDistributedClusters>()
                .JoinAction<LoadUndistributedClusters>()
                .JoinAction<TryShowHowToPlayHint>();

            var atTipAction = logicBuilder
                .AddAction<ApplyAdTip>();

            var prepareDragUndistributedClusterAction = logicBuilder.AddAction<PrepareDragUndistributedCluster>();
            var prepareDragDistributedClusterAction = logicBuilder.AddAction<PrepareDragDistributedCluster>();
            var dragUndistributedClusterAction = logicBuilder.AddAction<DragUndistributedCluster>();
            var dragDistributedClusterAction = logicBuilder.AddAction<DragDistributedCluster>();
            var endDragUndistributedClusterAction = logicBuilder.AddAction<EndDragUndistributedCluster>();
            var endDragDistributedClusterAction = logicBuilder.AddAction<EndDragDistributedCluster>();

            var clickUndistributedClusterAction = logicBuilder.AddAction<ClickUndistributedCluster>();
            var clickWordRowWithHintClusterAction = logicBuilder
                .AddAction<ClickWordRowWithHint>()
                .JoinAction<ResetClickContext>();
            var tryReturnClickedClusterAction = logicBuilder
                .AddAction<TryReturnClickedUndistributedCluster>()
                .JoinAction<ResetClickContext>();

            var trySaveLevelProgressAction = logicBuilder.AddAction<TrySaveLevelProgress>();
            var tryCompleteLevelAction = logicBuilder.AddAction<TryCompleteLevel>();

            var swipeInputSelector = logicBuilder.AddSelector<FirstScoreSelector<GameplayViewModelContext>>()
                .AddQualifier<OnBeginDragUndistributedCluster>(prepareDragUndistributedClusterAction)
                .AddQualifier<OnBeginDragDistributedCluster>(prepareDragDistributedClusterAction)
                .AddQualifier<OnDragUndistributedCluster>(dragUndistributedClusterAction)
                .AddQualifier<OnDragDistributedCluster>(dragDistributedClusterAction)
                .AddQualifier<OnEndDragUndistributedCluster>(endDragUndistributedClusterAction)
                .AddQualifier<OnEndDragDistributedCluster>(endDragDistributedClusterAction);
            var clickInputSelector = logicBuilder.AddSelector<FirstScoreSelector<GameplayViewModelContext>>()
                .AddQualifier<OnClickUndistributedCluster>(clickUndistributedClusterAction)
                .AddQualifier<OnClickWordRowWithHintCluster>(clickWordRowWithHintClusterAction)
                .AddQualifier<DefaultQualifier<GameplayViewModelContext>>(tryReturnClickedClusterAction);
            var rootSelector = logicBuilder.AddSelector<FirstScoreSelector<GameplayViewModelContext>>()
                .AddQualifier<IsLevelNotLoaded>(loadLevelAction)
                .AddQualifier<IsAdTip>(atTipAction)
                .AddQualifier<IsUserCheckCompleteLevel>(tryCompleteLevelAction)
                .AddQualifier<IsSwipeInput>(swipeInputSelector)
                .AddQualifier<IsClickInput>(clickInputSelector)
                .SetAsRoot();

            endDragUndistributedClusterAction.DirectTo(trySaveLevelProgressAction);
            endDragDistributedClusterAction.DirectTo(trySaveLevelProgressAction);
            clickWordRowWithHintClusterAction.DirectTo(trySaveLevelProgressAction);
            atTipAction.DirectTo(trySaveLevelProgressAction);

            _logicAgent = logicBuilder.Build();
            _logicAgent.OnCatchError += OnLogicFailed;
        }

        private void OnLogicFailed(string errorMessage)
        {
            Debug.Log(errorMessage);

            //todo dialog view with reload button
            _gameAppReloader.ReloadGame();
        }

        public async UniTask StartLevelLoading(RectTransform wordsHolder, RectTransform undistributedClustersHolder)
        {
            _logicAgent.Context.WordRowsHolder = wordsHolder;
            _logicAgent.Context.UndistributedClustersHolder = undistributedClustersHolder;
            await _logicAgent.ExecuteAsync();

            var wordLength = _gameDefs.LevelSettings.WordLengthsRange.Max;
            var wordCount = _gameDefs.LevelSettings.WordsRange.Max;
            var rulesDescKey = _gameDefs.LevelSettings.RulesDescriptionLocalizationKey;
            var localizedText= _userContext.GetLocalizedText(rulesDescKey);
            _descriptionLevelText.Value = string.Format(localizedText, wordLength, wordCount);

            var levelDefId = _logicAgent.Context.LevelProgress.LevelDefId;
            var levelNumber = _gameDefs.GetLevelNumber(levelDefId, _userContext.LocalizationDefId.Value);
            var levelNumberKey = _gameDefs.LevelSettings.LevelNumberLocalizationKey;
            var localizedLevelName = _userContext.GetLocalizedText(levelNumberKey);
            _levelNameText.Value = string.Format(localizedLevelName, levelNumber);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_logicAgent.IsExecuting || eventData.dragging) return;
            _logicAgent.Context.Input = (UserInputType.OnPointerClick, eventData);
            _logicAgent.Execute();
        }

        public void OnBeginDrag(PointerEventData eventData)
        { 
            if (_logicAgent.IsExecuting) return;
            _logicAgent.Context.Input = (UserInputType.OnBeginDrag, eventData);
            _logicAgent.Execute();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_logicAgent.IsExecuting) return;
            _logicAgent.Context.Input = (UserInputType.OnDrag, eventData);
            _logicAgent.Execute();
            Debug.Log(_logicAgent.GetLog());
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_logicAgent.IsExecuting) return;
            _logicAgent.Context.Input = (UserInputType.OnEndDrag, eventData);
            _logicAgent.Execute(true);
        }

        public void OnCheckWordsButtonClicked()
        {
            if (_logicAgent.IsExecuting) return;
            _logicAgent.Context.CheckCompleteLevel = true;
            _logicAgent.Execute(true);
        }

        public void OnAdTipButtonClicked()
        {
            if (_logicAgent.IsExecuting) return;
            _logicAgent.Context.AdTip.IsAdTip = true;
            _logicAgent.Execute(true);
        }

        public async void OnMainMenuButtonClicked()
        {
            if (_logicAgent.IsExecuting) return;
            _viewManager.Close<GameplayView>();
            var viewModel = _viewModelFactory.Create<MainMenuViewModel>();
            var view = await _viewManager.ShowAsync<MainMenuView, MainMenuViewModel>(viewModel);
        }

        public override void Dispose()
        {
            LevelNameText.Dispose();
            DescriptionLevelText.Dispose();
            IsFailedCompleteLevel.Dispose();

            _logicAgent.OnCatchError -= OnLogicFailed;
            _logicAgent.Dispose();
            base.Dispose();
        }
    }
}