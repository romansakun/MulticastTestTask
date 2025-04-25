using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.MainMenu
{
    public class MainMenuView : View
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;

        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] FormedWordNumber _wordNumberFirst;
        [SerializeField] FormedWordNumber _wordNumberSecond;
        [SerializeField] private RectTransform _spawnWordNumberPoint;
        [SerializeField] private RectTransform _despawnWordNumberPoint;
        [SerializeField] private RectTransform _wordNumberPoint;

        private MainMenuViewModel _viewModel;
        private Sequence _animationNumber;

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);

            return UniTask.CompletedTask;
        }

        protected override void Subscribes()
        {
            _playButton.onClick.AddListener(_viewModel.OnPlayButtonClicked);
            _settingsButton.onClick.AddListener(_viewModel.OnSettingsButtonClicked);
            _viewModel.FormedWordCount.Subscribe(OnFormedWordNumberChanged);
        }

        protected override void Unsubscribes()
        {
            _playButton.onClick.RemoveListener(_viewModel.OnPlayButtonClicked);
            _settingsButton.onClick.RemoveListener(_viewModel.OnSettingsButtonClicked);
            _viewModel.FormedWordCount.Unsubscribe(OnFormedWordNumberChanged);

            _animationNumber?.Kill();
        }

        private void OnFormedWordNumberChanged(int count)
        {
            var spawnPos = _spawnWordNumberPoint.position;
            var despawnPosY = _despawnWordNumberPoint.position.y;
            _animationNumber = DOTween.Sequence();
            for (var i = 0; i < count; i += 2)
            {
                var duration = _animationDuration / Mathf.Clamp(count - i, 1, count);
                var number = i;
                _animationNumber
                    .AppendCallback(() =>
                    {
                        _wordNumberFirst.SetNumber(number);
                        _wordNumberFirst.RectTransform.position = spawnPos;
                    }).SetRecyclable(true)
                    .Join(_wordNumberFirst.RectTransform.DOMoveY(despawnPosY, duration).SetRecyclable(true))
                    .AppendInterval(duration / 2).SetRecyclable(true)
                    .AppendCallback(() =>
                    {
                        _wordNumberSecond.SetNumber(number + 1);
                        _wordNumberSecond.RectTransform.position = spawnPos;
                    }).SetRecyclable(true)
                    .Join(_wordNumberSecond.RectTransform.DOMoveY(despawnPosY, duration).SetRecyclable(true))
                    .AppendInterval(duration / 2).SetRecyclable(true);
            }
            _animationNumber
                .AppendCallback(() =>
                {
                    _wordNumberFirst.SetNumber(count);
                    _wordNumberFirst.RectTransform.position = spawnPos;
                })
                .Join(_wordNumberFirst.RectTransform
                    .DOMove(_wordNumberPoint.position, _animationDuration)
                    .SetEase(Ease.OutBack));

        }
    }
}