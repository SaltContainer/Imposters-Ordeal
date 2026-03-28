using System;
using System.Collections.Generic;
using System.Linq;
using static ImpostersOrdeal.Distributions;

namespace ImpostersOrdeal
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Calculates the standard deviation of a sequence of double values.
        /// </summary>
        public static double StandardDeviation(this IEnumerable<double> observations)
        {
            double sum = 0;
            if (observations.Count() > 1)
            {
                double avg = observations.Average();
                foreach (var obs in observations)
                    sum += Math.Pow(avg - obs, 2) / (observations.Count() - 1);
            }
            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Calculates the standard deviation of a sequence of integer values.
        /// </summary>
        public static double StandardDeviation(this IEnumerable<int> observations)
        {
            return observations.Select(i => (double)i).StandardDeviation();
        }

        /// <summary>
        /// Calculates the frequency at which a certian predicate is true for items in an IList.
        /// </summary>
        public static double GetOccurrencePercent<T>(this IEnumerable<T> self, Func<T, bool> f)
        {
            int occurrences = self.Where(f).Count();
            return 100.0 * occurrences / self.Count();
        }

        /// <summary>
        /// Generates numeric distribution objects.
        /// </summary>
        public static (IDistribution[], int) GetNumericDistributionConfig<T>(this IEnumerable<T> self, Func<T, int> valueFunc, AbsoluteBoundaries.Boundary ab = AbsoluteBoundaries.Boundary.None)
        {
            List<int> observations = new();
            foreach (var item in self)
                if (AbsoluteBoundaries.IsWithin(ab, valueFunc.Invoke(item)))
                    observations.Add(valueFunc.Invoke(item));
            return ToNumericDistributionConfig(observations);
        }

        /// <summary>
        /// Generates numeric distribution objects from a list of observations.
        /// </summary>
        private static (IDistribution[], int) ToNumericDistributionConfig(this IEnumerable<int> self)
        {
            double min = self.Count() > 0 ? self.Min() : 0;
            double max = self.Count() > 0 ? self.Max() : 0;
            double uniAvg = (min + max) / 2.0;
            double avg = self.Count() > 0 ? self.Average() : 0;
            double std = self.StandardDeviation();

            IDistribution[] distributions = new IDistribution[]
            {
                new UniformConstant(100, min, max),
                new UniformRelative(100, (min-uniAvg)/2, (max-uniAvg)/2),
                new UniformProportional(100, (min+uniAvg)/(2*uniAvg == 0 ? 1 : 2*uniAvg), (max+uniAvg)/(2*uniAvg == 0 ? 1 : 2*uniAvg)),
                new NormalConstant(100, avg, std),
                new NormalRelative(100, std/2),
                new NormalProportional(100, std/(2*avg == 0 ? 1 : 2*avg))
            };
            return (distributions, 4);
        }

        /// <summary>
        /// Generates item distribution objects.
        /// </summary>
        public static (IDistribution[], List<string>, int) GetItemDistributionConfig<T, E>(this IEnumerable<T> self, Func<T, int> idFunc, IEnumerable<E> entities, Func<E, bool> validityFunc, Func<E, string> nameFunc)
        {
            int[] instances = new int[entities.Count()];
            foreach (var item in self)
                instances[idFunc.Invoke(item)]++;
            return ToItemDistributionConfig(instances, entities, validityFunc, nameFunc);
        }

        /// <summary>
        /// Generates item distribution objects from an array of instances.
        /// </summary>
        private static (IDistribution[], List<string>, int) ToItemDistributionConfig<E>(int[] instances, IEnumerable<E> entities, Func<E, bool> validityFunc, Func<E, string> nameFunc)
        {
            IDistribution[] distributions = new IDistribution[]
            {
                new Empirical(100, instances.ToList()),
                new UniformSelection(100, entities.Select(validityFunc).ToList())
            };
            List<string> names = entities.Select(nameFunc).ToList();
            return (distributions, names, 0);
        }
    }
}
