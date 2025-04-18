using Cysharp.Threading.Tasks;
using Infrastructure.GameActions;

namespace GameLogic.Model.Actions
{
    public class StartLevelGameAction : IGameAction  
    {
        public UniTask ExecuteAsync()
        {
            //throw new System.NotImplementedException("FUCK OFF");
            return UniTask.CompletedTask;
        }

        public IValidator GetValidator()
        {
            return new Validator();
        }

        private class Validator : IValidator
        {
            public Validator()
            {
                
            }

            public bool Check()
            {
                return true;
            }
        }

    }
}