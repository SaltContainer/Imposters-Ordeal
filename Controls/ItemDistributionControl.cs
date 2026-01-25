using static ImpostersOrdeal.Distributions;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ImpostersOrdeal
{
    public class ItemDistributionControl : Button
    {
        public IDistribution[] distributions = new IDistribution[]
        {
                new Empirical(100, (new int[] { 1, 0, 2, 0, 3, 0, 4 }).ToList()),
                new UniformSelection(100, (new bool[] { true, false, true, false, true, false, true }).ToList())
        };
        public List<string> itemNames = new(new string[] { "Item0", "Item1", "Item2", "Item3", "Item4", "Item5", "Item6" });
        public int idx;

        public IDistribution Get()
        {
            return distributions[idx];
        }

        public void SetCurrent(IDistribution d)
        {
            distributions[idx] = d;
        }

        public void Initialize((IDistribution[], List<string>, int) config)
        {
            if (config.Item1 != null)
                distributions = config.Item1;

            if (config.Item2 != null)
                itemNames = config.Item2;

            idx = config.Item3;
        }
    }
}
