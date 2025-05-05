namespace GameLogic.UI.Gameplay
{
    public class IsSwipeInput : BaseGameplayViewModelQualifier
    {
        public override float Score(GameplayViewModelContext context)
        {
            if (context.Click.IsClickInputNow)
                return 0;

            var isSwipe = context.Input.Type == UserInputType.OnBeginDrag 
                          || context.Input.Type == UserInputType.OnDrag 
                          || context.Input.Type == UserInputType.OnEndDrag;

            return isSwipe ? 1 : 0;
        }

    }
}