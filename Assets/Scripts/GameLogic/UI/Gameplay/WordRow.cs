using System;
using GameLogic.Extensions;
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
            gameObject.SetActive(true);
            SetEnabledBlinking(false);
        }

        public void SetEnabledBlinking(bool isEnabled)
        {
            _backImage.color = _backImage.color.WithAlpha(1f);
            _blinkAnimator.Rebind();
            _blinkAnimator.enabled = isEnabled;
        }

        public void OnDespawned()
        {
            _memoryPool = null;
            gameObject.SetActive(false);
        }

        public void Dispose()
        {
            if (this) _memoryPool?.Despawn(this);
        }

        public class Factory : PlaceholderFactory<WordRow>
        {
        }
    }
}