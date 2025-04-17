using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Infrastructure.LogicUtility
{
    public abstract class ActionWithOptions<T> : IAction<T> where T : class, IContext
    {
        private List<IOptionScorer<T>> _scorers = new ();

        private readonly List<KeyValuePair<object, float>> _allScoresLog = new ();

        public INode<T> Next { get; set; }


        public void SetOptions(List<IOptionScorer<T>> optionScorers)
        {
            _scorers = optionScorers;
        }

        public abstract UniTask ExecuteAsync(T context);

        protected async UniTask<TO> GetBest<TO>(T context, IEnumerable<TO> options) where TO : IEquatable<TO>
        {
            var allScores = await GetAllScores(context, options);

            var best = default(TO);
            var maxScore = float.MinValue;
            foreach (var pair in allScores)
            {
                if (pair.Value >= maxScore)
                {
                    best = pair.Key;
                    maxScore = pair.Value;
                }
            }
            return best;
        }

        protected async UniTask<TO> GetWorst<TO>(T context, IEnumerable<TO> options) where TO : IEquatable<TO>
        {
            var allScores = await GetAllScores(context, options);

            var worst = default(TO);
            var minScore = float.MaxValue;
            foreach (var pair in allScores)
            {
                if (pair.Value < minScore)
                {
                    worst = pair.Key;
                    minScore = pair.Value;
                }
            }
            return worst;
        }

        protected async UniTask<List<KeyValuePair<TO, float>>> GetAllScores<TO>(T context, IEnumerable<TO> options) where TO : IEquatable<TO>
        {
            var allScorers = new List<KeyValuePair<TO, float>>();
            foreach (var option in options)
            {
                var accumulator = 0f;
                for (int j = 0; j < _scorers.Count; j++)
                {
                    var scorer = _scorers[j];
                    var score = await scorer.ScoreAsync(context, option);
                    accumulator += score;
                }
                allScorers.Add(new KeyValuePair<TO, float>(option, accumulator));
            }

            _allScoresLog.Clear();
            foreach (var pair in allScorers)
            {
                _allScoresLog.Add(new KeyValuePair<object, float>(pair.Key, pair.Value));
            }

            return allScorers;
        }

        public virtual string GetLog()
        {
            var sb = new StringBuilder();
            foreach (var pair in _allScoresLog)
            {
                sb.AppendLine($"\t{pair.Key}: {pair.Value}");
            }

            return $"{GetType().Name}:\n{sb}";
        }
    }
}