using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class UgEncounterTable : ScriptableObject
    {
        public List<Sheettable> mons = new List<Sheettable>();

        public class Sheettable
        {
            public int monsno;
            public int version;
            public int zukanflag;
        }
    }
}
