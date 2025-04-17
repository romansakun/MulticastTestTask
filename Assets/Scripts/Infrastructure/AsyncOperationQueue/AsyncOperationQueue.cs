using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UniRx;

namespace Infrastructure 
{
    public class AsyncOperationQueue: IDisposable
    {
        public IReactiveProperty<float> Progress => _progress;
        private ReactiveProperty<float> _progress { get; } = new(0f);

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
            _progress.SetValueAndForceNotify(0f);
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
                var progressValue = 1f - _operations.Count / _allOperationsCount;
                _progress.SetValueAndForceNotify(progressValue);
            }
            _isProcessing = false;
        }

        public void Dispose()
        {
            _operations.Clear();
            _progress.Dispose();
        }
    }
}