using System;
using GameLogic.UI;
using Zenject;

namespace Factories
{
    public class ViewModelFactory
    {
        [Inject] private DiContainer _diContainer;

        public T Create<T>() where T : ViewModel
        {
            var viewModel = _diContainer.Instantiate<T>(Array.Empty<object>());
            viewModel.Initialize();
            return viewModel;
        }
    }
}