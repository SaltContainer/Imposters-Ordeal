using System;
using System.Collections.Generic;

namespace ImpostersOrdeal.Utils
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns a random element in a list, uniformly.
        /// </summary>
        public static T RandomElement<T>(this Random self, IList<T> list)
        {
            return list[self.Next(list.Count)];
        }

        /// <summary>
        /// Rolls against a percentage (0-100) and returns true if it is under that percentage.
        /// </summary>
        public static bool Percent(this Random self, double percent)
        {
            return self.NextDouble() * 100 < percent;
        }

        /// <summary>
        /// Shuffles a list uniformly in-place.
        /// </summary>
        public static void Shuffle<T>(this Random self, IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = self.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}
