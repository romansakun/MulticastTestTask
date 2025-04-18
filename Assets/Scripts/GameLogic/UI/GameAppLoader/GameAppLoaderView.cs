using Cysharp.Threading.Tasks;
using Factories;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace GameLogic.UI.GameAppLoader
{
    public class GameAppLoaderView : View
    {
        [Inject] private ViewModelFactory _viewModelFactory;

        [SerializeField] private TextMeshProUGUI _loadingText;

        private GameAppLoaderViewModel _viewModel;


        public async void Start()
        {
            var viewModel = _viewModelFactory.Create<GameAppLoaderViewModel>();
            await _viewManager.AddView(this, viewModel);
        }

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);
            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
            _viewModel.ProgressText.SubscribeToText(_loadingText);
        }

        protected override void Unsubscribes()
        {
        }
    }
}