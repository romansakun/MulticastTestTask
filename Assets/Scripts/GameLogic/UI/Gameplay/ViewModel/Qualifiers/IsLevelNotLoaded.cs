namespace GameLogic.UI.Gameplay
{
    public class IsLevelNotLoaded : BaseGameplayViewModelQualifier
    {
        public override float Score(GameplayViewModelContext context)
        {
            return context.LevelProgress == null ? 1 : 0;
        }
    }
}