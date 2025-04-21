using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.GameActions
{
    public class GameActionExecutor : IDisposable
    {
        public IReactiveProperty<IGameAction> JustPerformedGameAction => _justPerformedGameAction;
        private readonly ReactiveProperty<IGameAction> _justPerformedGameAction = new();

        private readonly Queue<IGameAction> _gameActions = new();
        private readonly List<IGameAction> _executedGameActions = new();
        private bool _isExecuting;

        public async UniTask ExecuteAsync(IGameAction gameAction)
        {
            _gameActions.Enqueue(gameAction);
            if (_isExecuting == false)
            {
                ExecuteNext();
            }
            while (_executedGameActions.Contains(gameAction) == false)
            {
                await UniTask.Yield();
            }
            _executedGameActions.Remove(gameAction);
        }

        private async void ExecuteNext()
        {
            var gameAction = _gameActions.Dequeue();
            try
            {
                var validator = gameAction.GetValidator();
                if (validator.Check())
                {
                    _isExecuting = true;
                    await gameAction.ExecuteAsync();
                    _isExecuting = false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"GameAction[{gameAction.GetType().Name}] error: {ex.Message}");
            }
            finally
            {
                _isExecuting = false;
                _executedGameActions.Add(gameAction);
                _justPerformedGameAction.SetValueAndForceNotify(gameAction);
            }

            if (_gameActions.Count > 0)
            {
                ExecuteNext();
            }
        }

        public void Dispose()
        {
            _gameActions.Clear();
            _executedGameActions.Clear();
        }

    }
}