using UnityEngine;

namespace Infrastructure.Extensions
{
    public static class TransformExtensions
    {
        public static void DestroyChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                Object.Destroy(child.gameObject);
            }
        }

    }
}