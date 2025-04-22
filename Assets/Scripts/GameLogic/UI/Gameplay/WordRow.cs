using System;
using Infrastructure.Extensions;
using Infrastructure.Pools;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class WordRow : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        [SerializeField] private Image _backImage;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _clustersHolder;
        [SerializeField] private Animator _blinkAnimator;

        private IMemoryPool _memoryPool;

        public RectTransform ClustersHolder => _clustersHolder;

        public void SetBackgroundColor(Color color)
        {
            _backImage.color = color;
        }

        public bool IsContainsScreenPoint(Vector2 position)
        {
            return _rectTransform.IsContainsScreenPoint( position);
        }

        public void SetParent(Transform parent)
        {
            _rectTransform.SetParent(parent, false);
        }

        public void OnSpawned(IMemoryPool memoryPool)
        {
            _memoryPool = memoryPool;
            SetEnabledBlinking(false);
            gameObject.SetActive(true);
        }

        public void SetEnabledBlinking(bool isEnabled)
        {
            _backImage.color = _backImage.color.WithAlpha(1f);
            if (!_blinkAnimator) return;
            _blinkAnimator.Rebind();
            _blinkAnimator.enabled = isEnabled;
        }

        public void OnDespawned()
        {
            _memoryPool = null;
            if (!this) return;
            SetEnabledBlinking(false);
            gameObject.SetActive(false);
        }

        public void Dispose()
        {
            _memoryPool.Despawn(this);
        }

        public class Factory : PlaceholderFactory<WordRow>
        {
            [Inject] private DeferredMonoPool<WordRow> _deferredMonoPool;

            public override WordRow Create()
            {
                return _deferredMonoPool.Spawn();
            }
        }

    }
}