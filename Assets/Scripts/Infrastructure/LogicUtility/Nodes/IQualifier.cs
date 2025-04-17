using Cysharp.Threading.Tasks;

namespace Infrastructure.LogicUtility
{
    public interface IQualifier<T> : INode<T> where T : class, IContext
    {
        UniTask<float> ScoreAsync(T context);
    }
}