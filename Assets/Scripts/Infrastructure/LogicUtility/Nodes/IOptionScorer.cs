using System;
using Cysharp.Threading.Tasks;

namespace Infrastructure.LogicUtility
{
    public interface IOptionScorer<T> where T : class, IContext
    {
        UniTask<float> ScoreAsync<TO>(T context, TO option) where TO : IEquatable<TO>;
    }
}