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

        public void OkButtonClicked()
        {
            _userContextOperator.SetHowToPlayHintShown();
            _viewManager.Close<HowToPlayHintView>();
        }
    }
}