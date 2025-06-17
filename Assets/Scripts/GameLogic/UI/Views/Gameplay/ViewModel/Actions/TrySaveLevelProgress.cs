using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameLogic.Factories;
using GameLogic.Helpers;
using GameLogic.Model.Actions;
using GameLogic.Model.DataProviders;
using Infrastructure.GameActions;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class TrySaveLevelProgress : BaseGameplayViewModelAction
    {
        [Inject] private GameActionExecutor _gameActionExecutor;
        [Inject] private GameActionFactory _gameActionFactory;
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private UserContextRatingHelper _ratingHelper;

        public override async UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            // TODO: replace it
            context.IsTipVisible.Value = context.UndistributedClusters.Count > 0;
            //

            var distributedClustersInView = new List<List<string>>();
            foreach (var row in context.WordRows)
            {
                var rowClusters = new List<string>();
                distributedClustersInView.Add(rowClusters);
                var clusters = context.WordRowsClusters[row];
                clusters.Sort((c1, c2) =>
                {
                    var siblingIndexOne = c1.GetSiblingIndex();
                    var siblingIndexTwo = c2.GetSiblingIndex();
                    return siblingIndexOne.CompareTo(siblingIndexTwo);
                });
                foreach (var cluster in clusters)
                {
                    rowClusters.Add(cluster.GetText());
                }
            }

            var isDifferent = IsDistributedClustersDifferent(context, distributedClustersInView);
            if (isDifferent == false)
                return;

            var undistributedClustersInView = new List<string>();
            foreach (var cluster in context.UndistributedClusters)
            {
                undistributedClustersInView.Add(cluster.GetText());
            }

            var gameAction = _gameActionFactory.Create<SaveLevelProgressGameAction>(context.LevelProgress.LevelDefId,
                undistributedClustersInView,
                distributedClustersInView);
            await _gameActionExecutor.ExecuteAsync(gameAction);
            if (context.IsDisposed) return;

            _userContext.TryGetLevelProgress(context.LevelProgress.LevelDefId, out var levelProgress);
            context.LevelProgress = levelProgress;

            context.CupsCountText.Value = $"+{_ratingHelper.GetLevelScore(context.LevelProgress)}";
        }

        private bool IsDistributedClustersDifferent(GameplayViewModelContext context, List<List<string>> distributedClustersInView)
        {
            var distributedClusters = context.LevelProgress.DistributedClusters;
            for (var i = 0; i < distributedClusters.Count; i++)
            {
                var row = distributedClusters[i];
                var rowInView = distributedClustersInView[i];
                if (row.Count != rowInView.Count)
                    return true;

                for (var j = 0; j < row.Count; j++)
                {
                    if (row[j] == rowInView[j]) 
                        continue;
                    
                    return true;
                }
            }
            return false;
        }

    }
}