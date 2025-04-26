using Cysharp.Threading.Tasks;

namespace Infrastructure.LogicUtility
{
    public class DefaultQualifier<T> : IQualifier<T> where T : class, IContext
    {
        public INode<T> Next { get; set; }
        public string GetLog()
        {
            return GetType().Name;
        }

        public UniTask<float> ScoreAsync(T context)
        {
            return UniTask.FromResult(1f);
        }
    }
}