using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameLogic.UI.Gameplay
{
    public class Cluster : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
    {
        [SerializeField] private Image _backImage;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _leftPointTransform;
        [SerializeField] private RectTransform _rightPointTransform;

        private IMemoryPool _memoryPool;

        public void SetText(string text)
        {
            _valueText.text = text;
        }

        public void SetBackgroundColor(Color color)
        {
            _backImage.color = color;
        }

        public void SetTextColor(Color color)
        {
            _valueText.color = color;
        }

        public void SetPosition(Vector2 position)
        {
            _rectTransform.anchoredPosition = position;
        }

        public void OnSpawned(IMemoryPool memoryPool)
        {
            _memoryPool = memoryPool;
        }

        public void OnDespawned()
        {
            _memoryPool = null;
        }

        public void Dispose()
        {
            if (this) _memoryPool?.Despawn(this);
        }

        public class Pool : MonoPoolableMemoryPool<IMemoryPool, Cluster>
        {
        }
    }
}