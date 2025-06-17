using Cysharp.Threading.Tasks;

namespace GameLogic.UI.Background
{
    public class BackgroundView : View 
    {
        public override UniTask Initialize(ViewModel viewModel)
        {
            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {

        }

        protected override void Unsubscribes()
        {

        }
    }
}