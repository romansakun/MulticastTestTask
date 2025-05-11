using Cysharp.Threading.Tasks;
using GameLogic.Model.Operators;
using Infrastructure.GameActions;
using Zenject;

namespace GameLogic.Model.Actions
{
    public class UseCheckingWordsGameAction : IGameAction  
    {
        [Inject] private UserContextOperator _userContextOperator;

        public UniTask ExecuteAsync()
        {
            _userContextOperator.UseCheckingWords();

            return UniTask.CompletedTask;
        }

        public IValidator GetValidator()
        {
            return new Validator();
        }

        private class Validator : IValidator
        {
            public bool Check()
            {
                return true;
            }
        }

    }
}