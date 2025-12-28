using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class PokemonDataTable : ScriptableObject
    {
        public List<PokemonData> Data = new List<PokemonData>();

        public class PokemonData
        {
            public SheetPersonal personal = new SheetPersonal();
            public SheetWazaOboe levelUpMoves = new SheetWazaOboe();
            public SheetTamagoWaza eggMoves = new SheetTamagoWaza();
            public SheetEvolve evolutionPaths = new SheetEvolve();

            public ushort formID;
            public (ushort wild, ushort trainer) pastEvoLvs;
            public (ushort wild, ushort trainer) nextEvoLvs;
            public List<PokemonData> pastPokemon;
            public List<PokemonData> nextPokemon;
            public List<PokemonData> inferiorForms;
            public List<PokemonData> superiorForms;
            public bool legendary;

            public class SheetPersonal
            {
                public bool valid_flag;
                public ushort id;
                public ushort monsno;
                public ushort form_index;
                public byte form_max;
                public byte color;
                public ushort gra_no;
                public byte basic_hp;
                public byte basic_atk;
                public byte basic_def;
                public byte basic_agi;
                public byte basic_spatk;
                public byte basic_spdef;
                public byte type1;
                public byte type2;
                public byte get_rate;
                public byte rank;
                public ushort exp_value;
                public ushort item1;
                public ushort item2;
                public ushort item3;
                public byte sex;
                public byte egg_birth;
                public byte initial_friendship;
                public byte egg_group1;
                public byte egg_group2;
                public byte grow;
                public ushort tokusei1;
                public ushort tokusei2;
                public ushort tokusei3;
                public ushort give_exp;
                public ushort height;
                public ushort weight;
                public ushort chihou_zukan_no;
                public uint machine1;
                public uint machine2;
                public uint machine3;
                public uint machine4;
                public uint hiden_machine;
                public ushort egg_monsno;
                public ushort egg_formno;
                public ushort egg_formno_kawarazunoishi;
                public bool egg_form_inherit_kawarazunoishi;
            }

            public class SheetWazaOboe
            {
                public List<LearnedMove> moves = new List<LearnedMove>();

                public class LearnedMove
                {
                    public ushort level;
                    public ushort move;
                }
            }

            public class SheetTamagoWaza
            {
                public List<ushort> wazaNo = new List<ushort>();
            }

            public class SheetEvolve
            {
                public List<EvolutionPath> paths = new List<EvolutionPath>();

                public class EvolutionPath
                {
                    public ushort method;
                    public ushort param;
                    public ushort toMonsno;
                    public ushort toFormno;
                    public ushort level;
                }
            }
        }
    }
}
