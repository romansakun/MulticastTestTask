using Cysharp.Threading.Tasks;
using GameLogic.Factories;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameLogic.UI.GameAppLoader
{
    public class GameAppLoaderView : View
    {
        [Inject] private ViewModelFactory _viewModelFactory;

        [SerializeField] private TextMeshProUGUI _loadingText;
        [SerializeField] private Image _loadingImage;

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
            _viewModel.Progress.Subscribe(OnProgressChanged);
        }

        protected override void Unsubscribes()
        {
            _viewModel.Progress.Unsubscribe(OnProgressChanged);
        }

        private void OnProgressChanged(float progress)
        {
            _loadingText.text = $"{(int)(progress * 100)}%";
            _loadingImage.fillAmount = progress;
        }
    }
}