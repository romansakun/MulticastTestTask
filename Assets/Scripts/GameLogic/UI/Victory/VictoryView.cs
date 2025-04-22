using Cysharp.Threading.Tasks;
using GameLogic.Audio;
using GameLogic.Bootstrapper;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameLogic.UI.Victory
{
    public class VictoryView : View
    {
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private SoundsSettings _soundsSettings;
        
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private RectTransform _wordsHolder;

        private VictoryViewModel _viewModel;

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);

            _viewModel.LoadResolvedWords(_wordsHolder);

            _audioPlayer.PlaySound(_soundsSettings.SuccessSound);

            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
            _nextLevelButton.onClick.AddListener(_viewModel.OnNextLevelButtonClicked);
            _mainMenuButton.onClick.AddListener(_viewModel.OnMainMenuButtonClicked);
        }

        protected override void Unsubscribes()
        {
            _nextLevelButton.onClick.AddListener(_viewModel.OnNextLevelButtonClicked);
            _mainMenuButton.onClick.AddListener(_viewModel.OnMainMenuButtonClicked);
        }

    }
}