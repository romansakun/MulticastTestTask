using Cysharp.Threading.Tasks;
using Infrastructure;
using Infrastructure.Services;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class YandexGraLoading : IAsyncOperation
    {
        [Inject] private IYandexGRA _gra;
        public async UniTask ProcessAsync()
        {
            await _gra.GameReady();
        }

    }
}