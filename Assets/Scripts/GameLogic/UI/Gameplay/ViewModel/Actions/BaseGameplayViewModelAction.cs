using Cysharp.Threading.Tasks;
using Infrastructure.LogicUtility;

namespace GameLogic.UI.Gameplay
{
    public class BaseGameplayViewModelAction : IAction<GameplayViewModelContext> 
    {
        public INode<GameplayViewModelContext> Next { get; set; }
        public string GetLog() => GetType().Name;

        public virtual UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            Execute(context);
            return UniTask.CompletedTask; 
        }

        public virtual void Execute(GameplayViewModelContext context)
        {
          
        }
    }
}