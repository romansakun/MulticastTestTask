using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Infrastructure.LogicUtility
{
    public interface ISelector<T> : INode<T> where T : class, IContext
    {
        List<IQualifier<T>> Qualifiers { get; }

        UniTask<INode<T>> SelectAsync(T context);
    }
}