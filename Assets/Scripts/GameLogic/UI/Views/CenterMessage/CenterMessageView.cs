using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GameLogic.UI.CenterMessage
{
    public class CenterMessageView : View
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private RectTransform _content;

        private CenterMessageViewModel _viewModel;
        private Sequence _animation;

        public override UniTask Initialize(ViewModel viewModel)
        {
            UpdateViewModel(ref _viewModel, viewModel);
            return UniTask.CompletedTask;
        }

        public async UniTask ShowAndClose()
        {
            _animation = DOTween.Sequence()
                .Append(_content.DOScaleY(1, 0.4f))
                .AppendInterval(1.2f)
                .Append(_content.DOScaleY(0, 0.4f));

            await _animation.AwaitForComplete();
            Close();
        }

        protected override void Subscribes()
        {
            _viewModel.Text.Subscribe(SetText);
        }

        protected override void Unsubscribes()
        {
            _viewModel.Text.Unsubscribe(SetText);
            _animation?.Kill();
        }

        private void SetText(string text)
        {
            _text.text = text;
        }
    }
}