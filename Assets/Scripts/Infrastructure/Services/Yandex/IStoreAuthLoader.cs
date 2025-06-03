using Cysharp.Threading.Tasks;
using Infrastructure.Services.Leaderboards;

namespace Infrastructure.Services
{
    public interface IStoreAuthLoader
    {
        UniTask WaitWhileAuth();
    }

    public interface IStoreGRA
    {
        UniTask GameReady();
    }

    public interface IStoreLocalization
    {
        void SetLocalization(string lang);
    }

    public interface IStoreLeaderboards
    {
        void SetLeaderboard(string lang, int score);
        UniTask<LBData> GetLeaderboard(string lang);
    }
}