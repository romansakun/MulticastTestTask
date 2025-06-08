using DG.Tweening;
using GameLogic.UI;
using GameLogic.UI.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Components
{
    public class StoreHandHelper : MonoBehaviour
    {
        [Inject] ViewManager _viewManager;
        
        
        [SerializeField] private Image _cursor;
        [SerializeField] private Image _cursorTap;
        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private RectTransform _sourceRectTransform;
        [SerializeField] private RectTransform _targetRectTransform;

        
        

        [ContextMenu("FirstScript")]
        private void FirstScript()
        {
            _rectTransform.anchoredPosition = _sourceRectTransform.anchoredPosition;
            var seq = DOTween.Sequence();
            seq
                .Append(_cursor.DOFade(1, .15f))
                .Join(_rectTransform.DOMove(_targetRectTransform.position, .55f))
                .AppendCallback((() =>
                {
                    if (_viewManager.TryGetView<GameplayView>(out var view))
                    {
                        var eventData = new PointerEventData(null);
                        eventData.position = _targetRectTransform.position;
                        eventData.dragging = true;
                        view.OnBeginDrag(eventData);
                    }
                }));


        }
    }
}