using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameLogic.UI.Components
{
    public class ViewContentAnimations : MonoBehaviour
    {
        [SerializeField] private RectTransform _viewContent;
        [SerializeField] private CanvasGroup _viewContentCanvasGroup;
        [SerializeField] float _duration = 0.5f;

        private Sequence _animation;

        public async UniTask ShowFromLeft()
        {
            _viewContent.anchoredPosition = new Vector2(_viewContent.rect.width, 0);
            _viewContentCanvasGroup.alpha = 0;
            _viewContentCanvasGroup.blocksRaycasts = false;

            _animation?.Kill();
            _animation = DOTween.Sequence()
                .Append(_viewContent.DOLocalMoveX(0, _duration))
                .Join(_viewContentCanvasGroup.DOFade(1, _duration));

            await _animation.AsyncWaitForCompletion();
            _viewContentCanvasGroup.blocksRaycasts = true;
        }

        public async UniTask HideToRight()
        {
            _animation?.Kill();
            _animation = DOTween.Sequence()
                .Append(_viewContent.DOLocalMoveX(-_viewContent.rect.width, _duration))
                .Join(_viewContentCanvasGroup.DOFade(0, _duration));

            await _animation.AsyncWaitForCompletion();
        }

        public async UniTask ShowByScaleAndAlpha()
        {
            _viewContent.localScale = Vector3.one / 2;
            _viewContentCanvasGroup.alpha = 0;
            _viewContentCanvasGroup.blocksRaycasts = false;

            _animation?.Kill();
            _animation = DOTween.Sequence()
                .Append(_viewContent.DOScale(Vector3.one, _duration).SetEase(Ease.OutBack))
                .Join(_viewContentCanvasGroup.DOFade(1, _duration));

            await _animation.AsyncWaitForCompletion();
            _viewContentCanvasGroup.blocksRaycasts = true;
        }

        public async UniTask HideByScaleAndAlpha()
        {
            _animation?.Kill();
            _animation = DOTween.Sequence()
                .Append(_viewContent.DOScale(Vector3.one / 2, _duration).SetEase(Ease.InBack))
                .Join(_viewContentCanvasGroup.DOFade(0, _duration));

            await _animation.AsyncWaitForCompletion();
            _viewContentCanvasGroup.blocksRaycasts = true;
        }

        private void OnDestroy()
        {
            _animation?.Kill();
        }
    }
}
