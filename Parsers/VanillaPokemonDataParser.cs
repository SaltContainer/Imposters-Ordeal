using AssetsTools.NET;
using ImpostersOrdeal.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public class VanillaPokemonDataParser : IParser<PokemonDataTable>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (PokemonDataTable)data);

        public IEnumerable<Type> GetRequiredDataSources() => [typeof(PersonalMasterdatasBundle)];

        // Monos
        private static readonly string EVOLVETABLE_MONONAME = "EvolveTable";
        private static readonly string PERSONALTABLE_MONONAME = "PersonalTable";
        private static readonly string TAMAGOWAZATABLE_MONONAME = "TamagoWazaTable";
        private static readonly string WAZAOBOETABLE_MONONAME = "WazaOboeTable";

        // Arrays
        private static readonly string AR_FIELD = "ar";
        private static readonly string DATA_FIELD = "Data";
        private static readonly string EVOLVE_FIELD = "Evolve";
        private static readonly string PERSONAL_FIELD = "Personal";
        private static readonly string WAZANO_FIELD = "wazaNo";
        private static readonly string WAZAOBOE_FIELD = "WazaOboe";

        // Fields
        private static readonly string BASICAGI_FIELD = "basic_agi";
        private static readonly string BASICATK_FIELD = "basic_atk";
        private static readonly string BASICDEF_FIELD = "basic_def";
        private static readonly string BASICHP_FIELD = "basic_hp";
        private static readonly string BASICSPATK_FIELD = "basic_spatk";
        private static readonly string BASICSPDEF_FIELD = "basic_spdef";
        private static readonly string CHIHOUZUKANNO_FIELD = "chihou_zukan_no";
        private static readonly string COLOR_FIELD = "color";
        private static readonly string EGGBIRTH_FIELD = "egg_birth";
        private static readonly string EGGFORMNO_FIELD = "egg_formno";
        private static readonly string EGGFORMNOINHERITKAWARAZUNOISHI_FIELD = "egg_form_inherit_kawarazunoishi";
        private static readonly string EGGFORMNOKAWARAZUNOISHI_FIELD = "egg_formno_kawarazunoishi";
        private static readonly string EGGGROUP1_FIELD = "egg_group1";
        private static readonly string EGGGROUP2_FIELD = "egg_group2";
        private static readonly string EGGMONSNO_FIELD = "egg_monsno";
        private static readonly string EXPVALUE_FIELD = "exp_value";
        private static readonly string FORMINDEX_FIELD = "form_index";
        private static readonly string FORMMAX_FIELD = "form_max";
        private static readonly string FORMNO_FIELD = "formNo";
        private static readonly string GETRATE_FIELD = "get_rate";
        private static readonly string GIVEEXP_FIELD = "give_exp";
        private static readonly string GRANO_FIELD = "gra_no";
        private static readonly string GROW_FIELD = "grow";
        private static readonly string HEIGHT_FIELD = "height";
        private static readonly string HIDENMACHINE_FIELD = "hiden_machine";
        private static readonly string ID_FIELD = "id";
        private static readonly string INITIALFRIENDSHIP_FIELD = "initial_friendship";
        private static readonly string ITEM1_FIELD = "item1";
        private static readonly string ITEM2_FIELD = "item2";
        private static readonly string ITEM3_FIELD = "item3";
        private static readonly string MNAME_FIELD = "m_Name";
        private static readonly string MACHINE1_FIELD = "machine1";
        private static readonly string MACHINE2_FIELD = "machine2";
        private static readonly string MACHINE3_FIELD = "machine3";
        private static readonly string MACHINE4_FIELD = "machine4";
        private static readonly string MONSNO_FIELD = "monsno";
        private static readonly string NO_FIELD = "no";
        private static readonly string RANK_FIELD = "rank";
        private static readonly string SEX_FIELD = "sex";
        private static readonly string TOKUSEI1_FIELD = "tokusei1";
        private static readonly string TOKUSEI2_FIELD = "tokusei2";
        private static readonly string TOKUSEI3_FIELD = "tokusei3";
        private static readonly string TYPE1_FIELD = "type1";
        private static readonly string TYPE2_FIELD = "type2";
        private static readonly string VALIDFLAG_FIELD = "valid_flag";
        private static readonly string WEIGHT_FIELD = "weight";

        // Exceptions
        private static readonly string MISSING_POKEMON_DATA_PARSER_ERROR = "Oh my, this {0} is missing some stuff...\n" +
                                                                           "I don't feel so good...\n" +
                                                                           "PersonalTable entries: {1}\n" +
                                                                           "{0} entries: {2}??";
        private static readonly string MISSING_POKEMON_DATA_MSG = "Missing Pokémon Data";

        public PokemonDataTable ParseFromSources(FileManager fileManager)
        {
            var data = new PokemonDataTable();

            var personalMasterdatasBundle = fileManager.GetPersonalMasterdatasBundle();
            var (personalTablePathId, personalTableMono) = personalMasterdatasBundle.GetMonoByName(PERSONALTABLE_MONONAME);
            var (wazaOboePathId, wazaOboeMono) = personalMasterdatasBundle.GetMonoByName(WAZAOBOETABLE_MONONAME);
            var (tamagoWazaPathId, tamagoWazaMono) = personalMasterdatasBundle.GetMonoByName(TAMAGOWAZATABLE_MONONAME);
            var (evolveTablePathId, evolveTableMono) = personalMasterdatasBundle.GetMonoByName(EVOLVETABLE_MONONAME);

            data.pathID = personalTablePathId;
            data.m_Name = personalTableMono[MNAME_FIELD].AsString;

            var personalTableFields = personalTableMono[PERSONAL_FIELD].GetArrayElements();
            var wazaOboeFields = wazaOboeMono[WAZAOBOE_FIELD].GetArrayElements();
            var tamagoWazaFields = tamagoWazaMono[DATA_FIELD].GetArrayElements();
            var evolveTableFields = evolveTableMono[EVOLVE_FIELD].GetArrayElements();

            var wazaOboeIncomplete = CheckForPersonalEntryCount(WAZAOBOETABLE_MONONAME, personalTableFields, wazaOboeFields);
            var tamagoWazaIncomplete = CheckForPersonalEntryCount(TAMAGOWAZATABLE_MONONAME, personalTableFields, tamagoWazaFields);
            var evolveTableIncomplete = CheckForPersonalEntryCount(EVOLVETABLE_MONONAME, personalTableFields, evolveTableFields);

            if (wazaOboeIncomplete || tamagoWazaIncomplete || evolveTableIncomplete)
                throw new Exception(MISSING_POKEMON_DATA_MSG);

            data.Data = new();
            for (int i=0; i<personalTableFields.Count; i++)
            {
                var personalTableField = personalTableFields[i];
                var wazaOboeField = wazaOboeFields[i];
                var tamagoWazaField = tamagoWazaFields[i];
                var evolveTableField = evolveTableFields[i];

                var mon = new PokemonDataTable.PokemonData();

                mon.personal = new();
                mon.personal.valid_flag = personalTableField[VALIDFLAG_FIELD].AsBool;
                mon.personal.id = personalTableField[ID_FIELD].AsUShort;
                mon.personal.monsno = personalTableField[MONSNO_FIELD].AsUShort;
                mon.personal.form_index = personalTableField[FORMINDEX_FIELD].AsUShort;
                mon.personal.form_max = personalTableField[FORMMAX_FIELD].AsByte;
                mon.personal.color = personalTableField[COLOR_FIELD].AsByte;
                mon.personal.gra_no = personalTableField[GRANO_FIELD].AsUShort;
                mon.personal.basic_hp = personalTableField[BASICHP_FIELD].AsByte;
                mon.personal.basic_atk = personalTableField[BASICATK_FIELD].AsByte;
                mon.personal.basic_def = personalTableField[BASICDEF_FIELD].AsByte;
                mon.personal.basic_agi = personalTableField[BASICAGI_FIELD].AsByte;
                mon.personal.basic_spatk = personalTableField[BASICSPATK_FIELD].AsByte;
                mon.personal.basic_spdef = personalTableField[BASICSPDEF_FIELD].AsByte;
                mon.personal.type1 = personalTableField[TYPE1_FIELD].AsByte;
                mon.personal.type2 = personalTableField[TYPE2_FIELD].AsByte;
                mon.personal.get_rate = personalTableField[GETRATE_FIELD].AsByte;
                mon.personal.rank = personalTableField[RANK_FIELD].AsByte;
                mon.personal.exp_value = personalTableField[EXPVALUE_FIELD].AsUShort;
                mon.personal.item1 = personalTableField[ITEM1_FIELD].AsUShort;
                mon.personal.item2 = personalTableField[ITEM2_FIELD].AsUShort;
                mon.personal.item3 = personalTableField[ITEM3_FIELD].AsUShort;
                mon.personal.sex = personalTableField[SEX_FIELD].AsByte;
                mon.personal.egg_birth = personalTableField[EGGBIRTH_FIELD].AsByte;
                mon.personal.initial_friendship = personalTableField[INITIALFRIENDSHIP_FIELD].AsByte;
                mon.personal.egg_group1 = personalTableField[EGGGROUP1_FIELD].AsByte;
                mon.personal.egg_group2 = personalTableField[EGGGROUP2_FIELD].AsByte;
                mon.personal.grow = personalTableField[GROW_FIELD].AsByte;
                mon.personal.tokusei1 = personalTableField[TOKUSEI1_FIELD].AsUShort;
                mon.personal.tokusei2 = personalTableField[TOKUSEI2_FIELD].AsUShort;
                mon.personal.tokusei3 = personalTableField[TOKUSEI3_FIELD].AsUShort;
                mon.personal.give_exp = personalTableField[GIVEEXP_FIELD].AsUShort;
                mon.personal.height = personalTableField[HEIGHT_FIELD].AsUShort;
                mon.personal.weight = personalTableField[WEIGHT_FIELD].AsUShort;
                mon.personal.chihou_zukan_no = personalTableField[CHIHOUZUKANNO_FIELD].AsUShort;
                mon.personal.machine1 = personalTableField[MACHINE1_FIELD].AsUInt;
                mon.personal.machine2 = personalTableField[MACHINE2_FIELD].AsUInt;
                mon.personal.machine3 = personalTableField[MACHINE3_FIELD].AsUInt;
                mon.personal.machine4 = personalTableField[MACHINE4_FIELD].AsUInt;
                mon.personal.hiden_machine = personalTableField[HIDENMACHINE_FIELD].AsUInt;
                mon.personal.egg_monsno = personalTableField[EGGMONSNO_FIELD].AsUShort;
                mon.personal.egg_formno = personalTableField[EGGFORMNO_FIELD].AsUShort;
                mon.personal.egg_formno_kawarazunoishi = personalTableField[EGGFORMNOKAWARAZUNOISHI_FIELD].AsUShort;
                mon.personal.egg_form_inherit_kawarazunoishi = personalTableField[EGGFORMNOINHERITKAWARAZUNOISHI_FIELD].AsBool;

                mon.formID = 0;
                if (mon.personal.id != mon.personal.monsno)
                    mon.formID = (ushort)(mon.personal.id - mon.personal.form_index + 1);
                mon.nextEvoLvs = (ushort.MaxValue, ushort.MaxValue);
                mon.pastPokemon = new();
                mon.nextPokemon = new();
                mon.inferiorForms = new();
                mon.superiorForms = new();

                mon.levelUpMoves = new();
                mon.levelUpMoves.moves = new();
                var monWazaOboeFields = wazaOboeField[AR_FIELD].GetArrayElements();
                for (int j=0; j<monWazaOboeFields.Count; j+=2)
                {
                    var levelUpMove = new PokemonDataTable.PokemonData.SheetWazaOboe.LearnedMove();

                    levelUpMove.level = monWazaOboeFields[j].AsUShort;
                    levelUpMove.move = monWazaOboeFields[j+1].AsUShort;

                    mon.levelUpMoves.moves.Add(levelUpMove);
                }

                mon.eggMoves = new();
                mon.eggMoves.wazaNo = new();
                var monTamagoWazaFields = tamagoWazaField[WAZANO_FIELD].GetArrayElements();
                for (int j=0; j<monTamagoWazaFields.Count; j++)
                {
                    mon.eggMoves.wazaNo.Add(monTamagoWazaFields[j].AsUShort);
                }

                mon.evolutionPaths = new();
                mon.evolutionPaths.paths = new();
                var monEvolveTableFields = evolveTableField[AR_FIELD].GetArrayElements();
                for (int j=0; j<monEvolveTableFields.Count; j+=5)
                {
                    var evolutionPath = new PokemonDataTable.PokemonData.SheetEvolve.EvolutionPath();

                    evolutionPath.method = monEvolveTableFields[j].AsUShort;
                    evolutionPath.param = monEvolveTableFields[j+1].AsUShort;
                    evolutionPath.toMonsno = monEvolveTableFields[j+2].AsUShort;
                    evolutionPath.toFormno = monEvolveTableFields[j+3].AsUShort;
                    evolutionPath.level = monEvolveTableFields[j+4].AsUShort;

                    mon.evolutionPaths.paths.Add(evolutionPath);
                }

                data.Data.Add(mon);
            }

            data.SetFamilies();

            return data;
        }

        public void SaveToSources(FileManager fileManager, PokemonDataTable data)
        {
            var personalMasterdatasBundle = fileManager.GetPersonalMasterdatasBundle();
            var (personalTablePathId, personalTableMono) = personalMasterdatasBundle.GetMonoByName(PERSONALTABLE_MONONAME);
            var (wazaOboePathId, wazaOboeMono) = personalMasterdatasBundle.GetMonoByName(WAZAOBOETABLE_MONONAME);
            var (tamagoWazaPathId, tamagoWazaMono) = personalMasterdatasBundle.GetMonoByName(TAMAGOWAZATABLE_MONONAME);
            var (evolveTablePathId, evolveTableMono) = personalMasterdatasBundle.GetMonoByName(EVOLVETABLE_MONONAME);

            personalTableMono[PERSONAL_FIELD].SetArrayElementsAndInit(data.Data, (personalTableField, mon) =>
            {
                personalTableField[VALIDFLAG_FIELD].AsBool = mon.personal.valid_flag;
                personalTableField[ID_FIELD].AsUShort = mon.personal.id;
                personalTableField[MONSNO_FIELD].AsUShort = mon.personal.monsno;
                personalTableField[FORMINDEX_FIELD].AsUShort = mon.personal.form_index;
                personalTableField[FORMMAX_FIELD].AsByte = mon.personal.form_max;
                personalTableField[COLOR_FIELD].AsByte = mon.personal.color;
                personalTableField[GRANO_FIELD].AsUShort = mon.personal.gra_no;
                personalTableField[BASICHP_FIELD].AsByte = mon.personal.basic_hp;
                personalTableField[BASICATK_FIELD].AsByte = mon.personal.basic_atk;
                personalTableField[BASICDEF_FIELD].AsByte = mon.personal.basic_def;
                personalTableField[BASICAGI_FIELD].AsByte = mon.personal.basic_agi;
                personalTableField[BASICSPATK_FIELD].AsByte = mon.personal.basic_spatk;
                personalTableField[BASICSPDEF_FIELD].AsByte = mon.personal.basic_spdef;
                personalTableField[TYPE1_FIELD].AsByte = mon.personal.type1;
                personalTableField[TYPE2_FIELD].AsByte = mon.personal.type2;
                personalTableField[GETRATE_FIELD].AsByte = mon.personal.get_rate;
                personalTableField[RANK_FIELD].AsByte = mon.personal.rank;
                personalTableField[EXPVALUE_FIELD].AsUShort = mon.personal.exp_value;
                personalTableField[ITEM1_FIELD].AsUShort = mon.personal.item1;
                personalTableField[ITEM2_FIELD].AsUShort = mon.personal.item2;
                personalTableField[ITEM3_FIELD].AsUShort = mon.personal.item3;
                personalTableField[SEX_FIELD].AsByte = mon.personal.sex;
                personalTableField[EGGBIRTH_FIELD].AsByte = mon.personal.egg_birth;
                personalTableField[INITIALFRIENDSHIP_FIELD].AsByte = mon.personal.initial_friendship;
                personalTableField[EGGGROUP1_FIELD].AsByte = mon.personal.egg_group1;
                personalTableField[EGGGROUP2_FIELD].AsByte = mon.personal.egg_group2;
                personalTableField[GROW_FIELD].AsByte = mon.personal.grow;
                personalTableField[TOKUSEI1_FIELD].AsUShort = mon.personal.tokusei1;
                personalTableField[TOKUSEI2_FIELD].AsUShort = mon.personal.tokusei2;
                personalTableField[TOKUSEI3_FIELD].AsUShort = mon.personal.tokusei3;
                personalTableField[GIVEEXP_FIELD].AsUShort = mon.personal.give_exp;
                personalTableField[HEIGHT_FIELD].AsUShort = mon.personal.height;
                personalTableField[WEIGHT_FIELD].AsUShort = mon.personal.weight;
                personalTableField[CHIHOUZUKANNO_FIELD].AsUShort = mon.personal.chihou_zukan_no;
                personalTableField[MACHINE1_FIELD].AsUInt = mon.personal.machine1;
                personalTableField[MACHINE2_FIELD].AsUInt = mon.personal.machine2;
                personalTableField[MACHINE3_FIELD].AsUInt = mon.personal.machine3;
                personalTableField[MACHINE4_FIELD].AsUInt = mon.personal.machine4;
                personalTableField[HIDENMACHINE_FIELD].AsUInt = mon.personal.hiden_machine;
                personalTableField[EGGMONSNO_FIELD].AsUShort = mon.personal.egg_monsno;
                personalTableField[EGGFORMNO_FIELD].AsUShort = mon.personal.egg_formno;
                personalTableField[EGGFORMNOKAWARAZUNOISHI_FIELD].AsUShort = mon.personal.egg_formno_kawarazunoishi;
                personalTableField[EGGFORMNOINHERITKAWARAZUNOISHI_FIELD].AsBool = mon.personal.egg_form_inherit_kawarazunoishi;
            });

            wazaOboeMono[WAZAOBOE_FIELD].SetArrayElementsAndInit(data.Data, (wazaOboeField, mon) =>
            {
                wazaOboeField[ID_FIELD].AsInt = mon.personal.id;
                wazaOboeField[AR_FIELD].SetArrayElementsAndInit(mon.levelUpMoves.moves.SelectMany(m => new List<ushort>(){ m.level, m.move }), (f, v) => f.AsUShort = v);
            });

            tamagoWazaMono[DATA_FIELD].SetArrayElementsAndInit(data.Data, (tamagoWazaField, mon) =>
            {
                tamagoWazaField[NO_FIELD].AsUShort = mon.personal.monsno;
                tamagoWazaField[FORMNO_FIELD].AsUShort = mon.formID;
                tamagoWazaField[WAZANO_FIELD].SetArrayElementsAndInit(mon.eggMoves.wazaNo, (f, v) => f.AsUShort = v);
            });

            evolveTableMono[EVOLVE_FIELD].SetArrayElementsAndInit(data.Data, (evolveTableField, mon) =>
            {
                evolveTableField[ID_FIELD].AsInt = mon.personal.id;
                evolveTableField[AR_FIELD].SetArrayElementsAndInit(mon.evolutionPaths.paths.SelectMany(p => new List<ushort>() { p.method, p.param, p.toMonsno, p.toFormno, p.level }), (f, v) => f.AsUShort = v);
            });

            personalMasterdatasBundle.SetMonoByPathID(personalTablePathId, personalTableMono);
            personalMasterdatasBundle.SetMonoByPathID(wazaOboePathId, wazaOboeMono);
            personalMasterdatasBundle.SetMonoByPathID(tamagoWazaPathId, tamagoWazaMono);
            personalMasterdatasBundle.SetMonoByPathID(evolveTablePathId, evolveTableMono);
        }

        private bool CheckForPersonalEntryCount(string mono, List<AssetTypeValueField> personalEntries, List<AssetTypeValueField> otherEntries)
        {
            var missingEntries = otherEntries.Count < personalEntries.Count;
            
            if (missingEntries)
                MainForm.ShowParserError(string.Format(MISSING_POKEMON_DATA_PARSER_ERROR, mono, personalEntries.Count, otherEntries.Count));

            return missingEntries;
        }
    }
}
