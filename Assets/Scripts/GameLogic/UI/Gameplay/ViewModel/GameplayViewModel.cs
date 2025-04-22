using Cysharp.Threading.Tasks;
using GameLogic.Factories;
using Infrastructure;
using Infrastructure.LogicUtility;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class GameplayViewModel : ViewModel 
    {
        [Inject] private LogicBuilderFactory _logicBuilderFactory;

        public IReactiveProperty<bool> IsUndistributedClustersScrollRectActive => _logicAgent.Context.IsUndistributedClustersScrollRectActive;
        public IReactiveProperty<bool> IsHintClusterInUndistributedClusters => _logicAgent.Context.IsHintClusterInUndistributedClusters;
        public IReactiveProperty<bool> IsFailedCompleteLevel => _logicAgent.Context.IsFailedCompleteLevel;

        private LogicAgent<GameplayViewModelContext> _logicAgent;

        public override void Initialize()
        {
            var logicBuilder = _logicBuilderFactory.Create<GameplayViewModelContext>();

            var loadLevelAction = logicBuilder
                .AddAction<GetOrAddLevelProgress>()
                .JoinAction<LoadWordRows>()
                .JoinAction<LoadWordDistributedClusters>()
                .JoinAction<LoadUndistributedClusters>();

            var prepareDragUndistributedClusterAction = logicBuilder.AddAction<PrepareDragUndistributedCluster>();
            var prepareDragDistributedClusterAction = logicBuilder.AddAction<PrepareDragDistributedCluster>();
            var dragUndistributedClusterAction = logicBuilder.AddAction<DragUndistributedCluster>();
            var dragDistributedClusterAction = logicBuilder.AddAction<DragDistributedCluster>();
            var endDragUndistributedClusterAction = logicBuilder.AddAction<EndDragUndistributedCluster>();
            var endDragDistributedClusterAction = logicBuilder.AddAction<EndDragDistributedCluster>();

            var trySaveLevelProgressAction = logicBuilder.AddAction<TrySaveLevelProgress>();
            var tryCompleteLevelAction = logicBuilder.AddAction<TryCompleteLevel>();
 
            endDragUndistributedClusterAction.DirectTo(trySaveLevelProgressAction);
            endDragDistributedClusterAction.DirectTo(trySaveLevelProgressAction);
            var swipeInputSelector = logicBuilder.AddSelector<FirstScoreSelector<GameplayViewModelContext>>()
                .AddQualifier<OnBeginDragUndistributedCluster>(prepareDragUndistributedClusterAction)
                .AddQualifier<OnBeginDragDistributedCluster>(prepareDragDistributedClusterAction)
                .AddQualifier<OnDragUndistributedCluster>(dragUndistributedClusterAction)
                .AddQualifier<OnDragDistributedCluster>(dragDistributedClusterAction)
                .AddQualifier<OnEndDragUndistributedCluster>(endDragUndistributedClusterAction)
                .AddQualifier<OnEndDragDistributedCluster>(endDragDistributedClusterAction);

            var rootSelector = logicBuilder.AddSelector<FirstScoreSelector<GameplayViewModelContext>>()
                .AddQualifier<IsLevelNotLoaded>(loadLevelAction)
                .AddQualifier<IsUserCheckCompleteLevel>(tryCompleteLevelAction)
                .AddQualifier<IsSwipeInput>(swipeInputSelector)
                .SetAsRoot();

            _logicAgent = logicBuilder.Build();
        }

        public async UniTask StartLevelLoading(RectTransform wordsHolder, RectTransform undistributedClustersHolder)
        {
            _logicAgent.Context.WordRowsHolder = wordsHolder;
            _logicAgent.Context.UndistributedClustersHolder = undistributedClustersHolder;
            await _logicAgent.ExecuteAsync();
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

        public override void Dispose()
        {
            _logicAgent.Dispose();

            base.Dispose();
        }

    }
}