using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class PickupTable : ScriptableObject
    {
        public List<PickupItem> PickupItems = new List<PickupItem>();

        public class PickupItem
        {
            public ushort ID;
            public List<byte> Ratios = new List<byte>();
        }
    }
}
