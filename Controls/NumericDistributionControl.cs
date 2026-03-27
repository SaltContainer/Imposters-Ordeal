using System.Collections.Generic;
using static ImpostersOrdeal.Distributions;
using System.Windows.Forms;

namespace ImpostersOrdeal
{
    public class NumericDistributionControl : GroupBox
    {
        public IDistribution[] distributions = new IDistribution[]
        {
                new UniformConstant(100, 0, 100),
                new UniformRelative(100, -25, 25),
                new UniformProportional(100, 0.5, 1.5),
                new NormalConstant(100, 50, 25),
                new NormalRelative(100, 25),
                new NormalProportional(100, 0.25)
        }; // Just some example data for testing purposes, don't worry about it ;)
        public int idx;

        public IDistribution Get()
        {
            return distributions[idx];
        }

        public void SetCurrent(IDistribution d)
        {
            distributions[idx] = d;
        }

        public void Initialize((IDistribution[], int) config)
        {
            if (config.Item1 != null)
                distributions = config.Item1;
            idx = config.Item2;
            UpdateTextBox();
        }

        public void UpdateTextBox()
        {
            List<Control> l = new();
            for (int i = 0; i < Controls.Count; i++)
                l.Add(Controls[i]);
            l.Find(c => c is TextBox).Text = Get().GetString();
        }
    }
}
