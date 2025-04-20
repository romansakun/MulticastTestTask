using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.UI.Gameplay
{
    public static class GameplayExtensions
    {
        public static bool IsContainsPoint(this RectTransform rt, Vector2 point)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(rt, point, null, Vector4.zero);
        }

        public static DummyCluster GetClosest (this List<DummyCluster> dummyClusters, Vector2 point)
        {
            var minSqrDistance = float.MaxValue;
            DummyCluster closest = null;
            for (var i = 0; i < dummyClusters.Count; i++)
            {
                var dummyCluster = dummyClusters[i];
                var distance = dummyCluster.RectTransform.anchoredPosition - point;
                if (distance.sqrMagnitude < minSqrDistance)
                {
                    minSqrDistance = distance.sqrMagnitude;
                    closest = dummyCluster;
                }
            }
            return closest;
        }

    }
}