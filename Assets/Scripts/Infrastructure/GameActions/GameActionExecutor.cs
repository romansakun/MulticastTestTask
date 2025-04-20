using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.GameActions
{
    public class GameActionExecutor : IDisposable
    {
        public IReactiveProperty<IGameAction> JustPerformedGameAction => _justPerformedGameAction;
        private readonly ReactiveProperty<IGameAction> _justPerformedGameAction = new();

        private readonly Queue<IGameAction> _gameActions = new();
        private bool _isExecuting;


        public void Execute(IGameAction gameAction)
        {
            _gameActions.Enqueue(gameAction);
            if (_isExecuting == false)
            {
                ExecuteNext();
            }
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
                    _justPerformedGameAction.SetValueAndForceNotify(gameAction);
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
            }

            if (_gameActions.Count > 0)
            {
                ExecuteNext();
            }
        }

        public void Dispose()
        {
            _gameActions.Clear();
        }

    }
}