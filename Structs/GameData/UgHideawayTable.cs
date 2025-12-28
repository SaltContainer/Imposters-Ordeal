using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public class UgHideawayTable
    {
        public UgHideawayData hideawayData = new UgHideawayData();
        public UgRareEncounterTable rareEncounterTable = new UgRareEncounterTable();

        public class UgHideawayData : ScriptableObject
        {
            public List<Sheettable> hideaways = new List<Sheettable>();

            public class Sheettable
            {
                public int id;
                public string FileName;
                public int size;
                public int min;
                public int max;
                public int smax;
                public int mmax;
                public int lmax;
                public int llmax;
                public int watermax;
                public List<int> typerate = new List<int>();
            }
        }
        
        public class UgRareEncounterTable : ScriptableObject
        {
            public List<UgRareEncounter> rareEncounters = new List<UgRareEncounter>();

            public class UgRareEncounter
            {
                public int id;
                public int monsno;
                public int version;
                public int Dspecialrate;
                public int Pspecialrate;
            }
        }

        /// <summary>
        /// Gets all rare encounters for a specific hideaway.
        /// </summary>
        public List<UgRareEncounterTable.UgRareEncounter> GetRareEncountersByHideawayID(int id)
        {
            return rareEncounterTable.rareEncounters.Where(e => e.id == id).ToList();
        }
    }
}
