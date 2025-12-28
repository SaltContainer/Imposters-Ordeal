using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class UgPokemonDataTable : ScriptableObject
    {
        public List<Sheettable> table = new List<Sheettable>();

        public class Sheettable
        {
            public int monsno;
            public int type1ID;
            public int type2ID;
            public int size;
            public int movetype;
            public List<int> reactioncode = new List<int>();
            public List<int> move_rate = new List<int>();
            public List<int> submove_rate = new List<int>();
            public List<int> reaction = new List<int>();
            public List<int> flagrate = new List<int>();
            public int rateup;
        }
    }
}
