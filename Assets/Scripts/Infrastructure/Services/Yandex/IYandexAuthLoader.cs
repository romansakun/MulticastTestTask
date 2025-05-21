using Cysharp.Threading.Tasks;

namespace Infrastructure.Services
{
    public interface IYandexAuthLoader
    {
        UniTask WaitWhileAuth();
    }

    public interface IYandexGRA
    {
        UniTask GameReady();
    }
}