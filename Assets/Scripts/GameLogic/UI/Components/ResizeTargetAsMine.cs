using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameLogic.UI.Components
{
    public class ResizeTargetAsMine : UIBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _target;

        private Vector2 _lastSizeDelta;


        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            var newSizeDelta = _rectTransform.sizeDelta;
            if (newSizeDelta != _lastSizeDelta)
            {
                _lastSizeDelta = newSizeDelta;
                _target.sizeDelta = newSizeDelta;
                LayoutRebuilder.MarkLayoutForRebuild(_target);
            }
        }
    }
}