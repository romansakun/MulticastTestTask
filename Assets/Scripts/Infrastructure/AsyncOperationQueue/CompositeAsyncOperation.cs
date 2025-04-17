using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Infrastructure
{
    public class CompositeAsyncOperation : IAsyncOperation
    {
        private readonly List<IAsyncOperation> _operations;
        private readonly List<UniTask> _operationTasks;

        public CompositeAsyncOperation(List<IAsyncOperation> operations)
        {
            if (operations == null || operations.Count == 0)
                throw new System.ArgumentNullException(nameof(operations));

            _operations = operations;
            _operationTasks = new List<UniTask>();
        }

        public async UniTask ProcessAsync()
        {
            foreach (var loadingItem in _operations)
            {
                _operationTasks.Add(loadingItem.ProcessAsync());
            }

            await UniTask.WhenAll(_operationTasks);
        }
    }
}