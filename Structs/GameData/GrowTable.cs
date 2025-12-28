using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class GrowTable : ScriptableObject
    {
        public List<SheetData> Rates = new List<SheetData>();

        public class SheetData
        {
            public int id;
            public List<uint> exps = new List<uint>();
        }
    }
}
