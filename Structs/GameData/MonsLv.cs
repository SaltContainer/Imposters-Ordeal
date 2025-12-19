using System;

namespace ImpostersOrdeal
{
    [Serializable]
    public struct MonsLv
    {
        public int maxlv;
        public int minlv;
        public int monsNo;

        public double GetAvgLevel()
        {
            return (minlv + maxlv) / 2.0;
        }
    }
}
