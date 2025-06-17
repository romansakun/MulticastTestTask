using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.GameInfo
{
    public class GameInfoView : View
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _adHintInfoText;
        
        private GameInfoViewModel _viewModel;
        
        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);
            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
            _backButton.onClick.AddListener(_viewModel.OnBackButtonClicked);
            _viewModel.AdHintInfoText.Subscribe(OnAdHintInfoTextChanged);
        }

        protected override void Unsubscribes()
        {
            _backButton.onClick.RemoveAllListeners();
            _viewModel.AdHintInfoText.Unsubscribe(OnAdHintInfoTextChanged);
        }

        private void OnAdHintInfoTextChanged(string text)
        {
            _adHintInfoText.text = text;
        }
    }
}