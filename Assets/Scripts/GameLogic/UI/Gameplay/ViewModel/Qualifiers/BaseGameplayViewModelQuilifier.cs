using Cysharp.Threading.Tasks;
using Infrastructure.LogicUtility;

namespace GameLogic.UI.Gameplay
{
    public class BaseGameplayViewModelQualifier : IQualifier<GameplayViewModelContext> 
    {
        public INode<GameplayViewModelContext> Next { get; set; }
        public string GetLog() => GetType().Name;

        public virtual UniTask<float> ScoreAsync(GameplayViewModelContext context)
        {
            var score = Score(context);
            return UniTask.FromResult(score);
        }

        public virtual float Score(GameplayViewModelContext context)
        {
            return 0f;
        }

    }
    
}