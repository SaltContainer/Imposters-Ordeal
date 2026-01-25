using System;
using System.Collections.Generic;
using System.Linq;
using static ImpostersOrdeal.Distributions;
using static ImpostersOrdeal.GameDataTypes;
using static ImpostersOrdeal.GlobalData;

namespace ImpostersOrdeal
{
    public static class IListExtensions
    {
        /// <summary>
        /// Calculates the frequency at which a certian predicate is true for items in an IList.
        /// </summary>
        public static double GetOccurrencePercent<T>(this IList<T> self, Func<T, bool> f)
        {
            int occurrences = self.Where(f).Count();
            return 100.0 * occurrences / self.Count;
        }

        /// <summary>
        /// Generates numeric distribution objects.
        /// </summary>
        public static (IDistribution[], int) GetNumericDistributionConfig<T>(this IList<T> self, Func<T, int> f, AbsoluteBoundary ab = AbsoluteBoundary.None)
        {
            List<int> observations = new();
            for (int item = 0; item < self.Count; item++)
                if (IsWithin(ab, f.Invoke(self[item])))
                    observations.Add(f.Invoke(self[item]));
            return ToNumericDistributionConfig(observations);
        }

        /// <summary>
        /// Generates numeric distribution objects from a list of observations.
        /// </summary>
        private static (IDistribution[], int) ToNumericDistributionConfig(this IList<int> self)
        {
            double min = self.Count > 0 ? self.Min() : 0;
            double max = self.Count > 0 ? self.Max() : 0;
            double uniAvg = (min + max) / 2.0;
            double avg = self.Count > 0 ? self.Average() : 0;
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
        public static (IDistribution[], List<string>, int) GetItemDistributionConfig<T>(this IList<T> self, Func<T, int> f, IList<INamedEntity> entities)
        {
            int[] instances = new int[entities.Count];
            for (int item = 0; item < self.Count; item++)
            {
                int instanceID = f.Invoke(self[item]);
                instances[instanceID]++;
            }
            return ToItemDistributionConfig(instances, entities);
        }

        /// <summary>
        /// Generates item distribution objects from an array of instances.
        /// </summary>
        private static (IDistribution[], List<string>, int) ToItemDistributionConfig(int[] instances, IList<INamedEntity> entities)
        {
            IDistribution[] distributions = new IDistribution[]
            {
                new Empirical(100, instances.ToList()),
                new UniformSelection(100, entities.Select(e => e.IsValid()).ToList())
            };
            List<string> names = entities.Select(t => t.GetName()).ToList();
            return (distributions, names, 0);
        }
    }
}
