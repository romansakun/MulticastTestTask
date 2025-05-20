namespace GameLogic.UI.Gameplay
{
    public class OnClickWordRowWithHintCluster : BaseGameplayViewModelQualifier
    {
        public override float Score(GameplayViewModelContext context)
        {
            if (context.Click.IsClickInputNow == false)
                return 0;

            if (context.Input == default)
                return 0;

            var position = context.Input.Data.position;
            var needWordRow = context.WordRows.Find(w => w.IsContainsScreenPoint(position));
            if (needWordRow == null)
                return 0;

            if (context.Click.WordRowHintClusters.TryGetValue(needWordRow, out var hintCluster) == false || hintCluster == null)
                return 0;

            context.Click.ClickedHintWordRow = needWordRow;
            context.Input = default;

            return 1;
        }
    }
}