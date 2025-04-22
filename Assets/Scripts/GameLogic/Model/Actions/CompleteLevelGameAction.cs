using System;
using Cysharp.Threading.Tasks;
using GameLogic.Model.DataProviders;
using GameLogic.Model.Operators;
using Infrastructure.GameActions;
using Zenject;

namespace GameLogic.Model.Actions
{
    public class CompleteLevelGameAction : IGameAction
    {
        [Inject] private UserContextOperator _userContextOperator;
        [Inject] private DiContainer _diContainer;

        private readonly string _levelDefId;

        public CompleteLevelGameAction(string levelDefId)
        {
            _levelDefId = levelDefId;
        }

        public UniTask ExecuteAsync()
        {
            _userContextOperator.CompleteLevel(_levelDefId);

            return UniTask.CompletedTask;
        }

        public IValidator GetValidator()
        {
            return _diContainer.Instantiate<Validator>(new object[] { _levelDefId });
        }

        private class Validator : IValidator
        {
            [Inject] private UserContextDataProvider _userContext;

            private readonly string _levelDefId;

            public Validator(string levelDefId)
            {
                _levelDefId = levelDefId;
            }

            public bool Check()
            {
                if (_userContext.TryGetLevelProgress(_levelDefId, out var prevLevelDef))
                {
                    if (_userContext.CheckUserGuessedWords(_levelDefId) == false)
                        throw new Exception($"[{nameof(CompleteLevelGameAction)}] level '{_levelDefId}' is not completed!");
                }
                else
                {
                    throw new Exception($"[{nameof(CompleteLevelGameAction)}] level '{_levelDefId}' is not started!");
                }
                return true;
            }

        }
    }
}