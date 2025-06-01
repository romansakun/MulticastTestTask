using Cysharp.Threading.Tasks;
using GameLogic.Helpers;
using Infrastructure;
using Zenject;

namespace GameLogic.Bootstrapper
{
    public class RatingLoading : IAsyncOperation
    {
        [Inject] private DiContainer _diContainer;

        public UniTask ProcessAsync()
        {
            var ratingHelper = _diContainer.Instantiate<UserContextRatingHelper>();
            ratingHelper.CalculateAllScores();

            _diContainer.Bind<UserContextRatingHelper>().FromInstance(ratingHelper).AsSingle();
            return UniTask.CompletedTask;
        }

    }
}