using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameLogic.UI.Components
{
    public class Star : MonoBehaviour
    {
        public Image Image;
        public RectTransform RectTransform;
    }

    public class StarsTweenAnimation : MonoBehaviour
    {
        [SerializeField] private List<Star> _stars;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _topRectTransform;
        [SerializeField] private RectTransform _leftRectTransform;
        [SerializeField] private Vector2 _starMovingTimeRange;
        
        private Sequence _animationSequence;
        private Vector2 _startPosXRange;
        private Vector2 _startPosYRange;
        
        private void Awake()
        {
            // var top = _topRectTransform.rect;
            // var left = _leftRectTransform.rect;
            // _startPosXRange = new Vector2(top.xMin, top.xMax);
            // _startPosYRange = new Vector2(left.yMin, left.yMax);
            // CreateAnimationSequence();
        }

        // private void CreateAnimationSequence()
        // {
        //     _animationSequence?.Kill();
        //     _animationSequence = DOTween.Sequence();
        //     for (var i = 0; i < _stars.Count; i++)
        //     {
        //         var star = _stars[i];
        //         var isEven = (i % 2 == 0);
        //         var starPos = isEven 
        //             ? new Vector3(-100, Random.Range(_startPosYRange.x, _startPosYRange.y), 0)
        //             : new Vector3(Random.Range(_startPosXRange.x, _startPosXRange.y), 100, 0);
        //
        //         star.RectTransform.anchoredPosition = starPos;
        //         var dest = new Vector3(_startPosXRange.x, Random.Range(_startPosYRange.x, _startPosYRange.y), 0);
        //         _animationSequence.Append(star.RectTransform.DOMove(starPos, Random.Range(_starMovingTimeRange.x, _starMovingTimeRange.y)))
        //     }
        // }
    }
}