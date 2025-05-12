using Cysharp.Threading.Tasks;
using GameLogic.Audio;
using GameLogic.Bootstrapper;
using GameLogic.GptChats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameLogic.UI.Victory
{
    public class VictoryView : View
    {
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private SoundsSettings _soundsSettings;
        [Inject] private IGptChat _gptChat;
        
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private RectTransform _wordsHolder;
        [SerializeField] private TextMeshProUGUI _congratulationsText;
        [SerializeField] private GameObject _gptChatLoadingCircle;

        private VictoryViewModel _viewModel;

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);

            _viewModel.LoadResolvedWords(_wordsHolder);

            _audioPlayer.PlaySound(_soundsSettings.SuccessSound);
            
           // _gptChat.Ask()

            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
            _nextLevelButton.onClick.AddListener(_viewModel.OnNextLevelButtonClicked);
            _mainMenuButton.onClick.AddListener(_viewModel.OnMainMenuButtonClicked);
            _viewModel.VisibleNextLevelButton.Subscribe(OnVisibleNextLevelButtonChanged);
            _viewModel.CongratulationsText.Subscribe(OnCongratulationsTextChanged);
        }

        protected override void Unsubscribes()
        {
            _nextLevelButton.onClick.AddListener(_viewModel.OnNextLevelButtonClicked);
            _mainMenuButton.onClick.AddListener(_viewModel.OnMainMenuButtonClicked);
            _viewModel.VisibleNextLevelButton.Unsubscribe(OnVisibleNextLevelButtonChanged);
            _viewModel.CongratulationsText.Unsubscribe(OnCongratulationsTextChanged);
        }

        private void OnCongratulationsTextChanged(string congratulationsText)
        {
            if (string.IsNullOrEmpty(congratulationsText) == false)
            {
                _congratulationsText.text = congratulationsText;
                _congratulationsText.gameObject.SetActive(true);
                _gptChatLoadingCircle.SetActive(false);
            }
            else
            {
                _congratulationsText.gameObject.SetActive(false);
            }
        }

        private void OnVisibleNextLevelButtonChanged(bool state)
        {
            _nextLevelButton.gameObject.SetActive(state);
        }
    }
}