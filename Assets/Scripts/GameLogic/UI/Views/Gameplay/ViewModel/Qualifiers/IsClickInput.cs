namespace GameLogic.UI.Gameplay
{
    public class IsClickInput : BaseGameplayViewModelQualifier
    {
        public override float Score(GameplayViewModelContext context)
        {
            if (context.Swipe.IsSwipeInputNow)
                return 0;

            var isClick = context.Input.Type == UserInputType.OnPointerClick
                          || context.Click.IsClickInputNow;

            return isClick ? 1 : 0;
        }

    }
}