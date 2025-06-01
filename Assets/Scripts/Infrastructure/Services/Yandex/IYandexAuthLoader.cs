using Cysharp.Threading.Tasks;
using Infrastructure.Services.Yandex.Leaderboards;

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

    public interface IYandexLocalization
    {
        void SetLocalization(string lang);
    }

    public interface IYandexLeaderboards
    {
        void SetLeaderboard(string lang, int score);
        UniTask<LBData> GetLeaderboard(string lang);
    }
}