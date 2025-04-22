using System;
using Infrastructure.Pools;
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

        private IMemoryPool _memoryPool;
        private string _value;


        public void SetText(string text)
        {
            _value = text;
            _valueText.text = text;
        }

        public string GetText()
        {
            return _value;
        }

        public void SetBackgroundColor(Color color)
        {
            _backImage.color = color;
        }

        public void SetTextColor(Color color)
        {
            _valueText.color = color;
        }

        public void SetPosition(Vector3 position)
        {
            _rectTransform.position = position;
        }

        public void SetRotation(Vector3 rotation)
        {
            _rectTransform.localEulerAngles = rotation;
        }

        public void SetParent(Transform parent)
        {
            _rectTransform.SetParent(parent, false);
        }

        public int GetSiblingIndex()
        {
            return _rectTransform.GetSiblingIndex();
        }

        public void SetSiblingIndex(int siblingIndex)
        {
            _rectTransform.SetSiblingIndex(siblingIndex);
        }

        public void OnSpawned(IMemoryPool memoryPool)
        {
            _memoryPool = memoryPool;
            gameObject.SetActive(true);
        }

        public void OnDespawned()
        {
            SetRotation(Vector3.zero);
            gameObject.SetActive(false);
            _memoryPool = null;
        }

        public bool IsCloserToRightEdge(Vector2 point)
        {
            var screenPoint = _rectTransform.GetScreenPoint();
            return screenPoint.x < point.x; 
        }

        public void SetActive(bool state)
        {
            if (gameObject.activeSelf != state)
                gameObject.SetActive(state);
        }

        public Vector2 GetScreenPoint()
        {
            return _rectTransform.GetScreenPoint();
        }

        public bool IsContainsScreenPoint(Vector2 position)
        {
            return _rectTransform.IsContainsScreenPoint( position);
        }

        public void Dispose()
        {
            _memoryPool.Despawn(this);
        }

        public class Factory : PlaceholderFactory<Cluster>
        {
            [Inject] private DeferredMonoPool<Cluster> _deferredMonoPool;

            public override Cluster Create()
            {
                return _deferredMonoPool.Spawn();
            }
        }

    }
}