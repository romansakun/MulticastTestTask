using Cysharp.Threading.Tasks;
using Infrastructure;
using Infrastructure.Services;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class YandexAuthLoader : IAsyncOperation
    {
        [Inject] private IYandexAuthLoader _auth;
        public async UniTask ProcessAsync()
        {
            await _auth.WaitWhileAuth();
        }
    }
}