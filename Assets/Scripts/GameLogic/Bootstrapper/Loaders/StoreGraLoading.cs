using Cysharp.Threading.Tasks;
using Infrastructure;
using Infrastructure.Services;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class StoreGraLoading : IAsyncOperation
    {
        [Inject] private IStoreGRA _gra;
        public async UniTask ProcessAsync()
        {
            await _gra.GameReady();
        }

    }
}