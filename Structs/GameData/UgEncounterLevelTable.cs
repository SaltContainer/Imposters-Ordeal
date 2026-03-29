using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class UgEncounterLevelTable : ScriptableObject
    {
        public List<SheetData> Ranges = new List<SheetData>();

        public class SheetData
        {
            public int MinLv;
            public int MaxLv;

            public double AverageLevel => (MinLv + MaxLv) / 2;
        }
    }
}
