using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public class FieldEncountTable : ScriptableObject
    {
        public List<Sheettable> table = new List<Sheettable>();
        public List<Sheeturayama> urayama = new List<Sheeturayama>();
        public List<Sheetmistu> mistu = new List<Sheetmistu>();
        public List<Sheethoneytree> honeytree = new List<Sheethoneytree>();
        public List<Sheetsafari> safari = new List<Sheetsafari>();
        public List<Sheetmvpoke> mvpoke = new List<Sheetmvpoke>();
        public List<Sheetlegendpoke> legendpoke = new List<Sheetlegendpoke>();
        public List<Sheetzui> zui = new List<Sheetzui>();

        public class Sheettable
        {
            public ZoneID zoneID;
            public int encRate_gr;
            public List<MonsLv> ground_mons = new List<MonsLv>();
            public List<MonsLv> tairyo = new List<MonsLv>();
            public List<MonsLv> day = new List<MonsLv>();
            public List<MonsLv> night = new List<MonsLv>();
            public List<MonsLv> swayGrass = new List<MonsLv>();
            public List<int> FormProb = new List<int>();
            public List<int> Nazo = new List<int>();
            public List<int> AnnoonTable = new List<int>();
            public List<MonsLv> gbaRuby = new List<MonsLv>();
            public List<MonsLv> gbaSapp = new List<MonsLv>();
            public List<MonsLv> gbaEme = new List<MonsLv>();
            public List<MonsLv> gbaFire = new List<MonsLv>();
            public List<MonsLv> gbaLeaf = new List<MonsLv>();
            public int encRate_wat;
            public List<MonsLv> water_mons = new List<MonsLv>();
            public int encRate_turi_boro;
            public List<MonsLv> boro_mons = new List<MonsLv>();
            public int encRate_turi_ii;
            public List<MonsLv> ii_mons = new List<MonsLv>();
            public int encRate_sugoi;
            public List<MonsLv> sugoi_mons = new List<MonsLv>();

            public class MonsLv
            {
                public int maxlv;
                public int minlv;
                public int monsNo;

                public double GetAvgLevel()
                {
                    return (minlv + maxlv) / 2.0;
                }
            }

            public List<List<MonsLv>> GetAllTables()
            {
                return new List<List<MonsLv>>()
                {
                    ground_mons, tairyo, day, night, swayGrass,
                    gbaRuby, gbaSapp, gbaEme, gbaFire, gbaLeaf,
                    water_mons, boro_mons, ii_mons, sugoi_mons
                };
            }

            public double GetAvgLevel()
            {
                return GetAllTables()
                    .Take(5)
                    .SelectMany(l => l)
                    .Where(e => e.monsNo != 0)
                    .Select(e => e.GetAvgLevel())
                    .DefaultIfEmpty()
                    .Average();
            }
        }

        public class Sheeturayama
        {
            public int monsNo;
        }

        public class Sheetmistu
        {
            public int Rate;
            public int Normal;
            public int Rare;
            public int SuperRare;
        }

        public class Sheethoneytree
        {
            public int Normal;
            public int Rare;
        }

        public class Sheetsafari
        {
            public int MonsNo;
        }

        public class Sheetmvpoke
        {
            public ZoneID zoneID;
            public int nextCount;
            public List<ZoneID> nextZoneID = new List<ZoneID>();
        }

        public class Sheetlegendpoke
        {
            public int monsNo;
            public int formNo;
            public bool isFixedEncSeq;
            public string encSeq;
            public bool isFixedBGM;
            public string bgmEvent;
            public bool isFixedBtlBg;
            public int btlBg; // ArenaID
            public bool isFixedSetupEffect;
            public int setupEffect; // BattleSetupEffectId
            public int waza1;
            public int waza2;
            public int waza3;
            public int waza4;
        }

        public class Sheetzui
        {
            public ZoneID zoneID;
            public List<bool> form = new List<bool>();
        }
    }
}
