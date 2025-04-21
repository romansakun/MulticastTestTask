using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Infrastructure 
{
    public class AsyncOperationQueue: IDisposable
    {
        public IReactiveProperty<float> Progress => _progress;
        private readonly ReactiveProperty<float> _progress = new(0f);

        private readonly Queue<IAsyncOperation> _operations = new();
        private float _allOperationsCount;
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
            _allOperationsCount = _operations.Count;
            while (_operations.Count > 0)
            {
                var operation = _operations.Dequeue();
                var progressValue = 1f - _operations.Count / _allOperationsCount;
                _progress.SetValueAndForceNotify(progressValue);
                try
                {
                    await operation.ProcessAsync();
                }
                catch (Exception ex) 
                {
                    throw new Exception($"AsyncOperationQueue [{operation.GetType().Name}]:\n{ex}");
                }
            }
            _progress.SetValueAndForceNotify(1f);
            _isProcessing = false;
        }

        public void Dispose()
        {
            _operations.Clear();
            _progress.Dispose();
        }
    }
}