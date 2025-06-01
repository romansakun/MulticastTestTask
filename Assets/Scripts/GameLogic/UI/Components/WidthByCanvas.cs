using UnityEngine;
using Zenject;

namespace GameLogic.UI.Components
{
    public class WidthByCanvas : MonoBehaviour
    {
        [Inject] private Canvas _canvas;

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private float _minWidth;
        [SerializeField] private float _maxWidth;

        // private void Awake()
        // {
            // var canvasSizeDelta = _canvas.GetComponent<RectTransform>().sizeDelta;
            // var sizeDeltaX = canvasSizeDelta.x < _maxWidth ? _minWidth : _maxWidth;
            // if (canvasSizeDelta.x > canvasSizeDelta.y) 
            //     sizeDeltaX = 1100;
            //
            // var sizeDeltaY = _rectTransform.sizeDelta.y;
            // _rectTransform.sizeDelta = new Vector2(sizeDeltaX, sizeDeltaY);
            
            //Debug.Log($"WidthByCanvas:  Awake {canvasSizeDelta}");
        //}
    }
}