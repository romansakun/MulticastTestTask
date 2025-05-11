using GameLogic.Model.DataProviders;
using GameLogic.Model.Operators;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class ResolveCheckingWordsAndHint : BaseGameplayViewModelAction
    {
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private UserContextOperator _userContextOperator;

        public override void Execute(GameplayViewModelContext context)
        {
            _userContextOperator.TryUpdateFreeConsumablesCount();

            var checkingWordsCount = _userContext.CheckingWordsCount.Value;
            context.CheckingWordsCount.Value = checkingWordsCount;
            context.IsCheckingWordsByAdsActive.Value = checkingWordsCount <= 0;

            var hintCount = _userContext.AdsTipsCount.Value;
            context.IsTipByAdsActive.Value = hintCount <= 0;

            context.IsTipVisible.Value = context.UndistributedClusters.Count > 0;
        }
    }
}