using Cysharp.Threading.Tasks;
using DG.Tweening;
using Infrastructure.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI.Components
{
    public class AlphaAnimation : AnimationComponent
    {
        [SerializeField] private Graphic _graphic;
        [SerializeField] private Ease _ease;
        [SerializeField] private float _startAlpha = 0f;
        [SerializeField] private float _endAlpha = 0.5f;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private float _beforeInterval = 0.5f;
        [SerializeField] private bool _onStart = true;

        private Sequence _animation;

        private async void Start()
        {
            if (_onStart) 
                await Animate();
        }
        
        public override async UniTask Animate()
        {
            _animation?.Kill();
            _animation = DOTween.Sequence();
            if (_beforeInterval > 0)
                _animation.PrependInterval(_beforeInterval);

            _graphic.color = _graphic.color.WithAlpha(_startAlpha);
            _animation.Append(_graphic.DOFade(_endAlpha, _duration).SetEase(_ease));

            await _animation.AsyncWaitForCompletion();
        }

        private void OnDestroy()
        {
            _animation?.Kill();
        }
    }
}