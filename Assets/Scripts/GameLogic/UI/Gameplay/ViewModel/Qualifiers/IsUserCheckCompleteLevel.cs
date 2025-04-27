namespace GameLogic.UI.Gameplay
{
    public class IsUserCheckCompleteLevel : BaseGameplayViewModelQualifier
    {
        public override float Score(GameplayViewModelContext context)
        {
            if (context.Click.IsClickInputNow)
                return 0;

            if (context.Swipe.IsSwipeInputNow)
                return 0;

            var score = context.CheckCompleteLevel ? 1 : 0;
            context.CheckCompleteLevel = false;
            return score;
        }

    }
}