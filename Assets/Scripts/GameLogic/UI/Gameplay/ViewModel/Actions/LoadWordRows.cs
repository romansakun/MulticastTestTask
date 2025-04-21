using System.Collections.Generic;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class LoadWordRows : BaseGameplayViewModelAction
    {
        [Inject] private WordRow.Factory _wordRowFactory;

        public override void Execute(GameplayViewModelContext context)
        {
            var levelProgress = context.LevelProgress;
            for (var i = 0; i < levelProgress.DistributedClusters.Count; i++)
            {
                var wordRow = _wordRowFactory.Create();
                wordRow.SetParent(context.WordRowsHolder);
                context.WordRows.Add(wordRow);
                context.WordRowsClusters.Add(wordRow, new List<Cluster>());
            }
        }
    }
}