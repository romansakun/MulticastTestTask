using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.Victory
{
    public class VictoryView : View
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private RectTransform _wordsHolder;

        private VictoryViewModel _viewModel;

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);

            _viewModel.LoadResolvedWords(_wordsHolder);

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