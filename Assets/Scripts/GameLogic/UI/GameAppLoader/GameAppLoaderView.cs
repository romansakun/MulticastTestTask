using Cysharp.Threading.Tasks;
using GameLogic.Factories;
using TMPro;
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
            await _viewManager.AddView(this, (GameAppLoaderViewModel)null);
        }

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);
            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
            _viewModel.ProgressText.Subscribe(OnLoadingTextChanged);
        }

        protected override void Unsubscribes()
        {
            _viewModel.ProgressText.Unsubscribe(OnLoadingTextChanged);
        }

        private void OnLoadingTextChanged(string text)
        {
            _loadingText.text = text;
        }
    }
}