using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;

namespace Infrastructure.LogicUtility
{
    internal class LogicUtilityClient<TContext>: IDisposable where TContext : class, IContext
    {
        public event Action<string> OnCatchError = delegate { };

        private readonly LogicAgent<TContext> _agent;
        private readonly List<INode<TContext>> _executedNodesChain;
        private readonly StringBuilder _errorsLog;
        private readonly StringBuilder _log;
        private readonly bool _safeMode;
        private bool _isDisposed;


        public LogicUtilityClient(LogicAgent<TContext> agent, bool safeMode)
        {
            _agent = agent;
            _safeMode = safeMode;
            _executedNodesChain = new List<INode<TContext>>(15);
            _errorsLog = new StringBuilder();
            _log = new StringBuilder();
        }

        public async UniTask ExecuteAsync(INode<TContext> rootNode)
        {
            _executedNodesChain.Clear();
            _errorsLog.Clear();

            var currentNode = rootNode;
            while (currentNode != null && !_isDisposed)
            {
                _executedNodesChain.Add(currentNode);
                try
                {
                    switch (currentNode)
                    {
                        case ISelector<TContext> selector:
                            currentNode = await selector.SelectAsync(_agent.Context);
                            break;
                        case IAction<TContext> action:
                            await action.ExecuteAsync(_agent.Context);
                            currentNode = action.Next;
                            break;
                        default:
                            currentNode = currentNode.Next;
                            break;
                    }
                    if (_executedNodesChain.Contains(currentNode))
                    {
                        throw new Exception($"Logic looped!");
                    }
                }
                catch (Exception error)
                {
                    var errorMsg = $"{error.Message}\n{error.StackTrace}\n{GetNodeLogs()}";
                    OnCatchError.Invoke(errorMsg);
                    if (_safeMode)
                    {
                        _errorsLog.AppendLine(errorMsg);
                        break;
                    }
                    else
                    {
                        throw new Exception(errorMsg);
                    }
                }
            }
        }

        public string GetNodeLogs()
        {
            if (_errorsLog.Length > 0)
                return _errorsLog.ToString();

            if (_executedNodesChain.Count == 0)
                return string.Empty;

            _log.Clear();
            foreach (var node in _executedNodesChain)
            {
                if (node == null) 
                    continue;

                _log.AppendLine($"-> {node.GetLog()}");
            }
            return _log.ToString();
        }

        public void Dispose()
        {
            OnCatchError = null;
            _isDisposed = true;
        }
    }
}