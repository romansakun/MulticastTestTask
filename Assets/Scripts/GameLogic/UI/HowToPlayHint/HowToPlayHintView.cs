using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.HowToPlayHint
{
    public class HowToPlayHintView : View
    {
        [SerializeField] private Button _okButton;

        private HowToPlayHintViewModel _viewModel;

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);

            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
            _okButton.onClick.AddListener(_viewModel.OkButtonClicked);
        }

        protected override void Unsubscribes()
        {
            _okButton.onClick.RemoveAllListeners();
        }
    }
}