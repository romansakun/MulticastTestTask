using Cysharp.Threading.Tasks;
using GameLogic.Model.Operators;
using Infrastructure.GameActions;
using Infrastructure.Services;
using Zenject;

namespace GameLogic.Model.Actions
{
    public class ClearUserContextGameAction : IGameAction  
    {
        [Inject] private UserContextOperator _userContextOperator;
        [Inject] private DiContainer _diContainer;
        [Inject] private IFileService _fileService;

        public UniTask ExecuteAsync()
        {
            _userContextOperator.ClearAllProgress();

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