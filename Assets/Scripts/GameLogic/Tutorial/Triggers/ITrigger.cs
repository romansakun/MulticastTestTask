namespace GameLogic.Tutorial
{
    public interface ITrigger
    {
        void TryTake (ITutorialComponent component);
        bool Check();
    }
}