using Cysharp.Threading.Tasks;
using GameLogic.Factories;
using GameLogic.Model.DataProviders;
using GameLogic.UI.HowToPlayHint;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class TryShowHowToPlayHint : BaseGameplayViewModelAction
    {
        [Inject] private UserContextDataProvider _userContext;
        [Inject] private ViewManager _viewManager;
        [Inject] private ViewModelFactory _viewModelFactory;

        public override async UniTask ExecuteAsync(GameplayViewModelContext context)
        {
            if (_userContext.IsHowToPlayHintShown()) 
                return;

            var viewModel = _viewModelFactory.Create<HowToPlayHintViewModel>();
            await _viewManager.ShowAsync<HowToPlayHintView, HowToPlayHintViewModel>(viewModel);
        }
    }
}