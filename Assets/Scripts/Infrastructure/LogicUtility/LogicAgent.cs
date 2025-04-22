using System;
using Cysharp.Threading.Tasks;

namespace Infrastructure.LogicUtility
{
    public class LogicAgent<TContext> : ILogicAgent where TContext : class, IContext
    {
        private readonly LogicUtilityClient<TContext> _client;
        private readonly INode<TContext> _root;

        private bool _isNeedNextExecutionAfterCurrent;
        private bool _isDisposed;

        public event Action<TContext> OnFinished = delegate { };
        public event Action<string> OnCatchError = delegate { };

        public IContext LogicContext => Context;
        public TContext Context { get; }
        public bool IsExecuting { get; private set; }


        public LogicAgent(TContext context, INode<TContext> rootNode, bool safeMode)
        {
            Context = context;
            _root = rootNode;
            _client = new LogicUtilityClient<TContext>(this, safeMode);
            _client.OnCatchError += OnLogicFailed;
        }

        public async UniTask ExecuteAsync(bool force = false)
        {
            await ExecuteInternal(force);
        }

        public async void Execute(bool force = false)
        {
            await ExecuteInternal(force);
        }

        private async UniTask ExecuteInternal(bool forceNext = false, bool needInvokeFinishEvent = true)
        {
            if (IsExecuting)
            {
                _isNeedNextExecutionAfterCurrent = _isNeedNextExecutionAfterCurrent || forceNext;
                return;
            }

            while (true)
            {
                IsExecuting = true;
                await _client.ExecuteAsync(_root);
                IsExecuting = false;

                if (_isNeedNextExecutionAfterCurrent == false || _isDisposed)
                    break;

                _isNeedNextExecutionAfterCurrent = false;
            }
            if (needInvokeFinishEvent && _isDisposed == false)
            {
                OnFinished.Invoke(Context);
            }
        }

        public string GetLog()
        {
            return $"LogicAgent<{typeof(TContext).Name}> execution log:\n{_client.GetNodeLogs()}";
        }

        private void OnLogicFailed(string error)
        {
            OnCatchError.Invoke(error);
        }

        public void Dispose()
        {
            _client.OnCatchError -= OnLogicFailed;
            OnFinished = null;
            _client.Dispose();
            Context.Dispose();
            _isDisposed = true;
        }
    }
}