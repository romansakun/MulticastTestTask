using System;
using GameLogic.Model.DataProviders;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class IsAdTip : BaseGameplayViewModelQualifier
    {
        [Inject] private GameDefsDataProvider _gameDefs;

        public override float Score(GameplayViewModelContext context)
        {
            if (context.AdTip.IsAdTip == false)
                return 0;

            if (context.Click.IsClickInputNow)
                return 0;

            if (context.Swipe.IsSwipeInputNow)
                return 0;

            var levelDef = _gameDefs.Levels[context.LevelProgress.LevelDefId];
            foreach (var pair in levelDef.Words)
            {
                var word = pair.Key;
                foreach (var wordRow in context.WordRowsClusters)
                {
                    var playerWord = context.WordRowsClusters.GetWord(wordRow.Key);
                    if (playerWord.Equals(word, StringComparison.CurrentCultureIgnoreCase) == false) 
                        continue;

                    context.AdTip.FormedWords.Add(word);
                    context.AdTip.ImmutableWordRows.Add(wordRow.Key);
                }
                if (context.AdTip.FormedWords.Contains(word))
                    continue;

                context.AdTip.NotFormedWords.Add(word);
            }
            if (context.AdTip.FormedWords.Count == levelDef.Words.Count)
            {
                context.AdTip.Reset();
                return 0;
            }

            context.AdTip.SuitableWordRow = context.WordRows.Find(wr => context.AdTip.ImmutableWordRows.Contains(wr) == false);
            context.AdTip.IsAdTip = false;
            return 1;
        }

    }
}