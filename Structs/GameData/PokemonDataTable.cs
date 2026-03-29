using ImpostersOrdeal.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

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

                public bool Valid => valid_flag && id > 0;

                public bool[] TMFlags
                {
                    get => [
                        ..machine1.GetBitArray(),
                        ..machine2.GetBitArray(),
                        ..machine3.GetBitArray(),
                        ..machine4.GetBitArray(),
                        ..hiden_machine.GetBitArray(),
                    ];
                    set
                    {
                        machine1 = value.Skip(0).Take(32).ToArray().ConvertBitArrayToUint();
                        machine2 = value.Skip(32).Take(32).ToArray().ConvertBitArrayToUint();
                        machine3 = value.Skip(64).Take(32).ToArray().ConvertBitArrayToUint();
                        machine4 = value.Skip(96).Take(32).ToArray().ConvertBitArrayToUint();
                        hiden_machine = value.Skip(128).Take(32).ToArray().ConvertBitArrayToUint();
                    }
                }

                public int[] CompatibleTMs
                {
                    get
                    {
                        var tmFlags = TMFlags;
                        return Enumerable.Range(0, tmFlags.Length).Where(i => tmFlags[i]).ToArray();
                    }
                }

                public byte[] EVYields
                {
                    get => Enumerable.Range(0, 6)
                        .Select(i => (byte)((exp_value & (3 << (2 * i))) >> (2 * i)))
                        .ToArray();
                    set
                    {
                        exp_value = 0;
                        for (int i=0; i<6; i++)
                            exp_value |= (ushort)(value[i] << (2 * i));
                    }
                }

                public int EVYieldTotal
                {
                    get
                    {
                        var yields = EVYields;
                        return yields[0] + yields[1] + yields[2] + yields[3] + yields[4] + yields[5];
                    }
                }

                public byte[] BaseStats
                {
                    get => [basic_hp, basic_atk, basic_def, basic_agi, basic_spatk, basic_spdef];
                    set
                    {
                        basic_hp = value[0];
                        basic_atk = value[1];
                        basic_def = value[2];
                        basic_agi = value[3];
                        basic_spatk = value[4];
                        basic_spdef = value[5];
                    }
                }

                public int BST => basic_hp + basic_atk + basic_def + basic_agi + basic_spatk + basic_spdef;

                public ushort[] Abilities
                {
                    get => [tokusei1, tokusei2, tokusei3];
                    set
                    {
                        tokusei1 = value[0];
                        tokusei2 = value[1];
                        tokusei3 = value[2];
                    }
                }

                public byte[] Types
                {
                    get => (type1 == type2) ? [type1] : [type1, type2];
                    set
                    {
                        type1 = value[0];
                        type2 = value.Length >= 2 ? value[1] : value[0];
                    }
                }

                public ushort[] HeldItems
                {
                    get => [item1, item2, item3];
                    set
                    {
                        item1 = value[0];
                        item2 = value[1];
                        item3 = value[2];
                    }
                }
            }

            public class SheetWazaOboe
            {
                public List<LearnedMove> moves = new List<LearnedMove>();

                public class LearnedMove
                {
                    public ushort level;
                    public ushort move;

                    public LearnedMove Clone()
                    {
                        return new LearnedMove()
                        {
                            level = level,
                            move = move,
                        };
                    }
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

            // TODO: clean this up a bit?
            /// <summary>
            /// Finds the levels the specified pokemon is likely to evolve: (wildLevel, trainerLevel).
            /// </summary>
            public (ushort, ushort) GetEvoLvs()
            {
                (ushort, ushort) evoLvs = (0, 0);
                for (int evolutionIdx = 0; evolutionIdx < evolutionPaths.paths.Count; evolutionIdx++)
                {
                    var evo = evolutionPaths.paths[evolutionIdx];
                    if (personal.monsno == evo.toMonsno)
                        continue;

                    switch (evo.method)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 21:
                        case 29:
                        case 43:
                            for (int pastEvos = 0; pastEvos < pastPokemon.Count; pastEvos++)
                            {
                                (ushort, ushort) pastPokemonEvoLvs = pastPokemon[pastEvos].GetEvoLvs();
                                evoLvs.Item1 = Math.Max(evoLvs.Item1, pastPokemonEvoLvs.Item1);
                                evoLvs.Item2 = Math.Max(evoLvs.Item2, pastPokemonEvoLvs.Item2);
                            }
                            if (evoLvs == (0, 0))
                                evoLvs = (1, 1);
                            evoLvs.Item1 += 16;
                            evoLvs.Item2 += 16;
                            break;
                        case 4:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
                        case 23:
                        case 24:
                        case 28:
                        case 32:
                        case 33:
                        case 34:
                        case 36:
                        case 37:
                        case 38:
                        case 40:
                        case 41:
                        case 46:
                        case 47:
                            evoLvs.Item1 = evo.level;
                            evoLvs.Item2 = evo.level;
                            break;
                        case 5:
                        case 8:
                        case 17:
                        case 18:
                        case 22:
                        case 25:
                        case 26:
                        case 27:
                        case 39:
                        case 42:
                        case 44:
                        case 45:
                            for (int pastEvos = 0; pastEvos < pastPokemon.Count; pastEvos++)
                            {
                                (ushort, ushort) pastPokemonEvoLvs = pastPokemon[pastEvos].GetEvoLvs();
                                evoLvs.Item1 = Math.Max(evoLvs.Item1, pastPokemonEvoLvs.Item1);
                                evoLvs.Item2 = Math.Max(evoLvs.Item2, pastPokemonEvoLvs.Item2);
                            }
                            if (evoLvs == (0, 0))
                                evoLvs = (1, 1);
                            evoLvs.Item1 += 32;
                            evoLvs.Item2 += 16;
                            break;
                        case 6:
                        case 7:
                            for (int pastEvos = 0; pastEvos < pastPokemon.Count; pastEvos++)
                            {
                                (ushort, ushort) pastPokemonEvoLvs = pastPokemon[pastEvos].GetEvoLvs();
                                evoLvs.Item1 = Math.Max(evoLvs.Item1, pastPokemonEvoLvs.Item1);
                                evoLvs.Item2 = Math.Max(evoLvs.Item2, pastPokemonEvoLvs.Item2);
                            }
                            if (evoLvs == (0, 0))
                                evoLvs = (1, 1);
                            evoLvs.Item1 += 48;
                            evoLvs.Item2 += 16;
                            break;
                        case 16:
                            for (int pastEvos = 0; pastEvos < pastPokemon.Count; pastEvos++)
                            {
                                (ushort, ushort) pastPokemonEvoLvs = pastPokemon[pastEvos].GetEvoLvs();
                                evoLvs.Item1 = Math.Max(evoLvs.Item1, pastPokemonEvoLvs.Item1);
                                evoLvs.Item2 = Math.Max(evoLvs.Item2, pastPokemonEvoLvs.Item2);
                            }
                            if (evoLvs == (0, 0))
                                evoLvs = (1, 1);
                            evoLvs.Item1 += 48;
                            evoLvs.Item2 += 32;
                            break;
                        case 19:
                        case 20:
                        case 30:
                        case 31:
                            evoLvs.Item1 = (ushort)(evo.level + 16);
                            evoLvs.Item2 = evo.level;
                            break;
                    }
                }
                return evoLvs;
            }
        }

        public PokemonData GetDataForPokemon(int monsno, int formno)
        {
            var baseForm = Data[monsno];
            if (formno == 0)
                return baseForm;
            else
                return Data[baseForm.personal.form_index + formno - 1];
        }

        public List<PokemonData> GetAllFormsForPokemon(int monsno)
        {
            var baseForm = Data[monsno];
            if (baseForm.personal.form_max > 1)
                return Data.Skip(baseForm.personal.form_index).Take(baseForm.personal.form_max - 1).Prepend(baseForm).ToList();
            else
                return new List<PokemonData>() { baseForm };
        }

        /// <summary>
        /// Overwrites and updates all Pokémon's evolution info for easier BST logic.
        /// </summary>
        public void SetFamilies()
        {
            foreach (var basePokemon in Data.Where(p => p.formID == 0))
            {
                var forms = GetAllFormsForPokemon(basePokemon.personal.monsno);
                foreach (var form in forms)
                {
                    foreach (var evoPath in form.evolutionPaths.paths)
                    {
                        var next = GetDataForPokemon(evoPath.toMonsno, evoPath.toFormno);

                        if (form.personal.monsno == next.personal.monsno)
                            continue;

                        form.nextPokemon.Add(next);
                        next.pastPokemon.Add(form);
                    }

                    foreach (var otherForm in forms)
                    {
                        if (otherForm.personal.BST - form.personal.BST >= 30)
                        {
                            otherForm.inferiorForms.Add(form);
                            form.superiorForms.Add(otherForm);
                        }
                    }
                }
            }

            foreach (var basePokemon in Data.Where(p => p.formID == 0))
            {
                var forms = GetAllFormsForPokemon(basePokemon.personal.monsno);
                foreach (var form in forms)
                {
                    (ushort, ushort) evoLvs = form.GetEvoLvs();
                    if (evoLvs == (0, 0))
                        continue;

                    form.nextEvoLvs = evoLvs;

                    foreach (var evoPath in form.evolutionPaths.paths)
                    {
                        var next = GetDataForPokemon(evoPath.toMonsno, evoPath.toFormno);

                        if (form.personal.monsno == next.personal.monsno)
                            continue;

                        next.pastEvoLvs.Item1 = Math.Max(next.pastEvoLvs.Item1, evoLvs.Item1);
                        next.pastEvoLvs.Item2 = Math.Max(next.pastEvoLvs.Item2, evoLvs.Item2);
                    }
                }
            }
        }
    }
}
