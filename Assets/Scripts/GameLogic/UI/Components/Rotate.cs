using DG.Tweening;
using UnityEngine;

namespace GameLogic.UI.Components
{
    [RequireComponent(typeof(RectTransform))]
    public class Rotate : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private float _rotationSpeed = 1f;

        private Tween _tween;

        private void OnValidate()
        {
            _rectTransform ??= GetComponent<RectTransform>();
        }

        private void Awake()
        {
            _rectTransform ??= GetComponent<RectTransform>();
            StartRotate();
        }

        private void OnEnable()
        {
            StartRotate();
        }

        private void StartRotate()
        {
            _tween?.Kill();
            _tween = _rectTransform
                .DOLocalRotate(new Vector3(0, 0, -360), _rotationSpeed, RotateMode.LocalAxisAdd)
                .SetLoops(-1);
        }

        private void OnDisable()
        {
            _tween?.Kill();
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}