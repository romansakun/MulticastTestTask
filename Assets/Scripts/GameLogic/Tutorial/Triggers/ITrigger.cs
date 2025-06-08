namespace GameLogic.Tutorial
{
    public interface ITrigger
    {
        void TryTake (TutorialCommand command);
        bool Check();
    }
}