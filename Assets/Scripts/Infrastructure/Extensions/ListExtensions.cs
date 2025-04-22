using System;
using System.Collections.Generic;

namespace Infrastructure.Extensions
{
    public static class ListExtensions 
    {
        public static T Find<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
        {
            foreach (var item in list)
            {
                if (item == null) 
                    continue;

                if (predicate(item))
                    return item;
            }
            return default;
        }

        public static void Shuffle<T> (this List<T> list)
        {
            var random = new Random();
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = random.Next(i, list.Count);
                if (randomIndex == i) 
                    continue;

                list.Swap(i, randomIndex);
            }
        }

        private static void Swap<T> (this IList<T> list, int index1, int index2)
        {
            (list[index1], list[index2]) = (list[index2], list[index1]);
        }

    }
}