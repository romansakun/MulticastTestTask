using System.Collections.Generic;
using GameLogic.Model.DataProviders;
using UnityEngine;
using Zenject;

namespace GameLogic.Helpers
{
    public class UserContextRatingHelper
    {
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private UserContextDataProvider _userContext;

        private readonly Dictionary<string, int> _ratings = new();

        public void CalculateAllScores()
        {
            foreach (var localizationPair in _gameDefs.Localizations)
            {
                var localizationDef = localizationPair.Value;
                _ratings[localizationDef.Id] = 0;

                foreach (var levelPair in localizationDef.Levels)
                {
                    if (_userContext.TryGetLevelProgress(levelPair.Value, out var levelProgress) == false)
                        continue;

                    if (levelProgress.IsCompleted == false)
                        continue;

                    AddLevelScore(localizationDef.Id, levelProgress);
                }
            }
        }

        // 30 - is max score by level
        // 10 - is min score by level
        public int AddLevelScore(string localizationDefId, LevelProgressContextDataProvider levelProgress)
        {
            var needMovesCount = 0;
            foreach (var clusters in levelProgress.DistributedClusters)
            {
                needMovesCount += clusters.Count;
            }
            var movesCount = Mathf.Clamp(levelProgress.SavesCount - needMovesCount, 0, int.MaxValue);
            var extraScore = Mathf.Clamp(20 - movesCount, 0, 20);
            var score = 10 + extraScore;

            _ratings[localizationDefId] += score;

            return score;
        }

        public int GetRating(string localizationDefId)
        {
            return _ratings[localizationDefId];
        }
    }
}