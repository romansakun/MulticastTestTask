using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Infrastructure.LogicUtility
{
    public class HighScoreSelector<T> : ISelector<T> where T : class, IContext
    {
        public INode<T> Next { get; set; }
        public List<IQualifier<T>> Qualifiers { get; } = new List<IQualifier<T>>();

        public string GetLog() => nameof(HighScoreSelector<T>);

        public virtual async UniTask<INode<T>> SelectAsync(T context)
        {
            IQualifier<T> highScoreQualifier = null;

            var maxScore = 0f;
            for (int i = 0; i < Qualifiers.Count; i++)
            {
                var qualifier = Qualifiers[i];
                var qualifierScore = await qualifier.ScoreAsync(context);
                if (qualifierScore > maxScore)
                {
                    maxScore = qualifierScore;
                    highScoreQualifier = qualifier;
                }
            }

            return highScoreQualifier ?? Next;
        }
    }
}