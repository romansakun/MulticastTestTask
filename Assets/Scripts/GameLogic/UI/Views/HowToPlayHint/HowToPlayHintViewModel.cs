using GameLogic.Model.Operators;
using Zenject;

namespace GameLogic.UI.HowToPlayHint
{
    public class HowToPlayHintViewModel : ViewModel
    {
        [Inject] private UserContextOperator _userContextOperator;
        [Inject] private ViewManager _viewManager;

        public override void Initialize()
        {
            
        }

        public async void OkButtonClicked()
        {
            _userContextOperator.SetHowToPlayHintShown();
            await _viewManager.Close<HowToPlayHintView>();
        }
    }
}