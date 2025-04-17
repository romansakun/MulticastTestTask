using Cysharp.Threading.Tasks;

namespace Infrastructure.LogicUtility
{
    public interface IAction<T> : INode<T> where T : class, IContext
    {
        UniTask ExecuteAsync(T context);
    }
}