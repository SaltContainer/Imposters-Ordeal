using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public class FieldEncountTable : ScriptableObject
    {
        public Sheettable[] table;
        public Sheeturayama[] urayama;
        public Sheetmistu[] mistu;
        public Sheethoneytree[] honeytree;
        public Sheetsafari[] safari;
        public Sheetmvpoke[] mvpoke;
        public Sheetlegendpoke[] legendpoke;
        public Sheetzui[] zui;

        public Sheettable this[int index] => table[index];

        [Serializable]
        public class Sheettable
        {
            public ZoneID zoneID;
            public int encRate_gr;
            public MonsLv[] ground_mons;
            public MonsLv[] tairyo;
            public MonsLv[] day;
            public MonsLv[] night;
            public MonsLv[] swayGrass;
            public int[] FormProb;
            public int[] Nazo;
            public int[] AnnoonTable;
            public MonsLv[] gbaRuby;
            public MonsLv[] gbaSapp;
            public MonsLv[] gbaEme;
            public MonsLv[] gbaFire;
            public MonsLv[] gbaLeaf;
            public int encRate_wat;
            public MonsLv[] water_mons;
            public int encRate_turi_boro;
            public MonsLv[] boro_mons;
            public int encRate_turi_ii;
            public MonsLv[] ii_mons;
            public int encRate_sugoi;
            public MonsLv[] sugoi_mons;

            public List<MonsLv[]> GetAllTables()
            {
                return new List<MonsLv[]>()
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

        [Serializable]
        public class Sheeturayama
        {
            public int monsNo;
        }

        [Serializable]
        public class Sheetmistu
        {
            public int Rate;
            public int Normal;
            public int Rare;
            public int SuperRare;
        }

        [Serializable]
        public class Sheethoneytree
        {
            public int Normal;
            public int Rare;
        }

        [Serializable]
        public class Sheetsafari
        {
            public int MonsNo;
        }

        [Serializable]
        public class Sheetmvpoke
        {
            public ZoneID zoneID;
            public int nextCount;
            public ZoneID[] nextZoneID;
        }

        [Serializable]
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

        [Serializable]
        public class Sheetzui
        {
            public ZoneID zoneID;
            public bool[] form;
        }
    }
}
