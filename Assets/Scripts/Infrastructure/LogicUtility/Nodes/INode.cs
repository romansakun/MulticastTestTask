namespace Infrastructure.LogicUtility
{
    public interface INode<T>  where T : class, IContext
    {
        INode<T> Next {get; set;}
        string GetLog();
    }  
}