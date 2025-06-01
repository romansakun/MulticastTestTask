using DG.Tweening;
using UnityEngine;

namespace GameLogic.UI.Components
{
    public enum AnimationType
    {
        Scale,
        Move,
        Rotate
    }

    public class RectTransformAnimations : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private AnimationType _animationType;
        [SerializeField] private Vector3 _from;
        [SerializeField] private Vector3 _to;
        [SerializeField] private Ease _ease;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private float _beforeInterval = 0.5f;
        [SerializeField] private bool _onStart = true;

        private Sequence _animation;

        private void Start()
        {
            if (_onStart == false)
                return;

            Animate();
        }

        private void Animate()
        {
            _animation?.Kill();
            _animation = DOTween.Sequence();
            if (_beforeInterval > 0)
                _animation.AppendInterval(_beforeInterval);

            if (_animationType == AnimationType.Move)
            {
                _rectTransform.anchoredPosition = _from;
                _animation.Append(_rectTransform.DOLocalMove(_to, _duration).SetEase(_ease));
            }
            else if (_animationType == AnimationType.Rotate)
            {
                _rectTransform.localEulerAngles = _from;
                _animation.Append(_rectTransform.DOLocalRotate(_to, _duration).SetEase(_ease));
            }
            else if (_animationType == AnimationType.Scale)
            {
                _rectTransform.localScale = _from;
                _animation.Append(_rectTransform.DOScale(_to, _beforeInterval).SetEase(_ease));
            }
        }

        private void OnDestroy()
        {
            _animation?.Kill();
        }
    }
}