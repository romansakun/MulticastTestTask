using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameLogic.Model.Operators;
using Infrastructure.GameActions;
using Zenject;

namespace GameLogic.Model.Actions
{
    public class SaveLevelProgressGameAction : IGameAction  
    {
        [Inject] private UserContextOperator _userContextOperator;

        private readonly string _levelDefId;
        private readonly List<string> _undistributedClusters;
        private readonly List<List<string>> _distributedClusters;

        public SaveLevelProgressGameAction(string levelDefId, 
            List<string> undistributedClusters, 
            List<List<string>> distributedClusters)
        {
            _levelDefId = levelDefId;
            _undistributedClusters = undistributedClusters;
            _distributedClusters = distributedClusters;
        }

        public UniTask ExecuteAsync()
        {
            _userContextOperator.AddOrUpdateLevelProgress(_levelDefId, _undistributedClusters, _distributedClusters);

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