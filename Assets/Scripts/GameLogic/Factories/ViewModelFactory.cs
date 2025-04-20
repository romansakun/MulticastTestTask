using GameLogic.UI;
using Zenject;

namespace Factories
{
    public class ViewModelFactory
    {
        [Inject] private DiContainer _diContainer;

        public T Create<T>(params object[] extraArgs) where T : ViewModel
        {
            var viewModel = _diContainer.Instantiate<T>(extraArgs);
            viewModel.Initialize();
            return viewModel;
        }
    }
}