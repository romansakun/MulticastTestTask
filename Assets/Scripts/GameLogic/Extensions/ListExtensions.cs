using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
    public static class ListExtensions
    {
        public static void Shuffle<T> (this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = Random.Range(i, list.Count);
                if (randomIndex == i) 
                    continue;

                list.Swap(i, randomIndex);
            }
        }

        public static void Swap<T> (this IList<T> list, int index1, int index2)
        {
            (list[index1], list[index2]) = (list[index2], list[index1]);
        }
    }
}