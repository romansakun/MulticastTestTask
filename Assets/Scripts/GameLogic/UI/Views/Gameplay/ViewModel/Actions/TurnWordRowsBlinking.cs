using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class TurnWordRowsBlinking : BaseGameplayViewModelAction
    {
        [Inject] private WordRow.Factory _wordRowFactory;

        public override void Execute(GameplayViewModelContext context)
        {
            var levelProgress = context.LevelProgress;
            for (var i = 0; i < levelProgress.DistributedClusters.Count; i++)
            {
                var guessWord = _wordRowFactory.Create();
                guessWord.SetParent(context.WordRowsHolder);
                context.WordRows.Add(guessWord);
            }
        }
    }
}