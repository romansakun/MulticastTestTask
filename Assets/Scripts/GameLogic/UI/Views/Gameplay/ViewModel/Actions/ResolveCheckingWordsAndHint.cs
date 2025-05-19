using GameLogic.Model.DataProviders;
using GameLogic.Model.Operators;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class ResolveCheckingWordsAndHint : BaseGameplayViewModelAction
    {
        [Inject] private GameDefsDataProvider _gameDefs;
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private UserContextOperator _userContextOperator;

        public override void Execute(GameplayViewModelContext context)
        {
            _userContextOperator.TryUpdateFreeConsumablesCount();

            var checkingWordsCount = _userContext.CheckingWordsCount.Value;
            var checkingWordsState = ConsumableButtonState.State(checkingWordsCount, _gameDefs.DefaultSettings.ConsumablesFreeCount);
            context.CheckingWordsButtonState.SetValueAndForceNotify(checkingWordsState);

            var tipCount = _userContext.AdsTipsCount.Value;
            var tipState = ConsumableButtonState.State(tipCount, _gameDefs.DefaultSettings.ConsumablesFreeCount);
            context.TipButtonState.SetValueAndForceNotify(tipState);
        }
    }
}