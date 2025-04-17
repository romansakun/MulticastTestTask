using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UniRx;

namespace Infrastructure 
{
    public class AsyncOperationQueue: IDisposable
    {
        private readonly Queue<IAsyncOperation> _operations = new Queue<IAsyncOperation>();
        public ReactiveProperty<float> Progress { get; } = new(0);

        private float _operationsCount;
        private bool _isProcessing;

        public void Add (IAsyncOperation operation)
        {
            if (_isProcessing)
            {
                throw new InvalidOperationException("AsyncOperationQueue is already processing.");
            }
            _operations.Enqueue(operation);
        }

        public async UniTask ProcessAsync()
        {
            if (_isProcessing)
            {
                throw new InvalidOperationException("AsyncOperationQueue is already processing.");
            }
            _isProcessing = true;
            _operationsCount = _operations.Count;
            Progress.SetValueAndForceNotify(0f);
            while (_operations.Count > 0)
            {
                var operation = _operations.Dequeue();
                try
                {
                    await operation.ProcessAsync();
                }
                catch (Exception ex) 
                {
                    throw new Exception($"AsyncOperationQueue [{operation.GetType().Name}]: {ex}");
                }
                Progress.SetValueAndForceNotify(1f - _operations.Count / _operationsCount);
            }
            _isProcessing = false;
        }

        public void Dispose()
        {
            _operations.Clear();
            Progress.Dispose();
        }
    }
}