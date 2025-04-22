using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameLogic.UI.Gameplay
{
    public static class GameplayExtensions
    {
        private static readonly StringBuilder _stringBuilder = new ();

        public static string GetWord(this Dictionary<WordRow, List<Cluster>> wordRows, WordRow wordRow)
        {
            _stringBuilder.Clear();
            var clusters = wordRows[wordRow];
            foreach (var cluster in clusters)
            {
                _stringBuilder.Append(cluster.GetText());
            }
            return _stringBuilder.ToString();
        }

        public static Vector2 GetScreenPoint(this RectTransform rt)
        {
            var point = rt.TransformPoint(rt.rect.center);
            return RectTransformUtility.WorldToScreenPoint(null, point);
        }

        public static bool IsContainsScreenPoint(this RectTransform rt, Vector2 point)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(rt, point, null, Vector4.zero);
        }

        public static int GetSiblingIndex(this Dictionary<WordRow, List<Cluster>> wordRows, WordRow wordRow, Vector2 point)
        {
            var sublimeIndex = 0;
            var clusters = wordRows[wordRow];
            if (clusters.Count <= 0)
                return sublimeIndex;

            var closestCluster = clusters.GetClosest(point);
            sublimeIndex = closestCluster.GetSiblingIndex();
            sublimeIndex += closestCluster.IsCloserToRightEdge(point) ? 1 : 0;
            return sublimeIndex;
        }

        public static Cluster GetClosest (this List<Cluster> clusters, Vector2 point)
        {
            var minSqrDistance = float.MaxValue;
            Cluster closest = null;
            for (var i = 0; i < clusters.Count; i++)
            {
                var cluster = clusters[i];
                var distance = cluster.GetScreenPoint() - point;
                if (distance.sqrMagnitude < minSqrDistance)
                {
                    minSqrDistance = distance.sqrMagnitude;
                    closest = cluster;
                }
            }
            return closest;
        }

    }
}