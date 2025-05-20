using Cysharp.Threading.Tasks;

namespace GameLogic.Ads
{
    public interface IAdsShower
    {
        UniTask<bool> Show ();
    }
}