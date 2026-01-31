using AssetsTools.NET;
using ImpostersOrdeal.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public class VanillaEncounterTableParser : IParser<FieldEncountTableCollection>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (FieldEncountTableCollection)data);

        public IEnumerable<Type> GetRequiredDataSources() => [typeof(GameSettingsBundle)];

        // Monos
        private static readonly string FIELDENCOUNTTABLED_MONONAME = "FieldEncountTable_d";
        private static readonly string FIELDENCOUNTTABLEP_MONONAME = "FieldEncountTable_p";

        // Arrays
        private static readonly string ANNOONTABLE_FIELD = "AnnoonTable";
        private static readonly string BOROMONS_FIELD = "boro_mons";
        private static readonly string DAY_FIELD = "day";
        private static readonly string FORMPROB_FIELD = "FormProb";
        private static readonly string GBAEME_FIELD = "gbaEme";
        private static readonly string GBAFIRE_FIELD = "gbaFire";
        private static readonly string GBALEAF_FIELD = "gbaLeaf";
        private static readonly string GBARUBY_FIELD = "gbaRuby";
        private static readonly string GBASAPP_FIELD = "gbaSapp";
        private static readonly string GROUNDMONS_FIELD = "ground_mons";
        private static readonly string HONEYTREE_FIELD = "honeytree";
        private static readonly string IIMONS_FIELD = "ii_mons";
        private static readonly string LEGENDPOKE_FIELD = "legendpoke";
        private static readonly string MISTU_FIELD = "mistu";
        private static readonly string MVPOKE_FIELD = "mvpoke";
        private static readonly string NAZO_FIELD = "Nazo";
        private static readonly string NEXTZONEID_FIELD = "nextZoneID";
        private static readonly string NIGHT_FIELD = "night";
        private static readonly string SAFARI_FIELD = "safari";
        private static readonly string SUGOIMONS_FIELD = "sugoi_mons";
        private static readonly string SWAYGRASS_FIELD = "swayGrass";
        private static readonly string TABLE_FIELD = "table";
        private static readonly string TAIRYO_FIELD = "tairyo";
        private static readonly string URAYAMA_FIELD = "urayama";
        private static readonly string WATERMONS_FIELD = "water_mons";
        private static readonly string ZUI_FIELD = "zui";

        // Fields
        private static readonly string BGMEVENT_FIELD = "bgmEvent";
        private static readonly string BTLBG_FIELD = "btlBg";
        private static readonly string ENCRATEGR_FIELD = "encRate_gr";
        private static readonly string ENCRATESUGOI_FIELD = "encRate_sugoi";
        private static readonly string ENCRATETURIBORO_FIELD = "encRate_turi_boro";
        private static readonly string ENCRATETURIII_FIELD = "encRate_turi_ii";
        private static readonly string ENCRATEWAT_FIELD = "encRate_wat";
        private static readonly string ENCSEQ_FIELD = "encSeq";
        private static readonly string FORM_FIELD = "form";
        private static readonly string FORMNO_FIELD = "formNo";
        private static readonly string ISFIXEDBGM_FIELD = "isFixedBGM";
        private static readonly string ISFIXEDBTLBG_FIELD = "isFixedBtlBg";
        private static readonly string ISFIXEDENCSEQ_FIELD = "isFixedEncSeq";
        private static readonly string ISFIXEDSETUPEFFECT_FIELD = "isFixedSetupEffect";
        private static readonly string MNAME_FIELD = "m_Name";
        private static readonly string MAXLV_FIELD = "maxlv";
        private static readonly string MINLV_FIELD = "minlv";
        private static readonly string LOWERCASE_MONSNO_FIELD = "monsNo";
        private static readonly string MONSNO_FIELD = "MonsNo";
        private static readonly string NEXTCOUNT_FIELD = "nextCount";
        private static readonly string NORMAL_FIELD = "Normal";
        private static readonly string RARE_FIELD = "Rare";
        private static readonly string RATE_FIELD = "Rate";
        private static readonly string SETUPEFFECT_FIELD = "setupEffect";
        private static readonly string SUPERRARE_FIELD = "SuperRare";
        private static readonly string WAZA1_FIELD = "waza1";
        private static readonly string WAZA2_FIELD = "waza2";
        private static readonly string WAZA3_FIELD = "waza3";
        private static readonly string WAZA4_FIELD = "waza4";
        private static readonly string ZONEID_FIELD = "zoneID";

        public FieldEncountTableCollection ParseFromSources(FileManager fileManager)
        {
            var data = new FieldEncountTableCollection();

            var gameSettingsBundle = fileManager.GetGameSettingsBundle();
            var monos = new List<KeyValuePair<long, AssetTypeValueField>>()
            {
                gameSettingsBundle.GetMonoByName(FIELDENCOUNTTABLED_MONONAME),
                gameSettingsBundle.GetMonoByName(FIELDENCOUNTTABLEP_MONONAME),
            };

            foreach (var (pathID, mono) in monos)
            {
                FieldEncountTable encountTable = new();
                encountTable.pathID = pathID;
                encountTable.m_Name = mono[MNAME_FIELD].AsString;

                encountTable.table = new();
                var tableFields = mono[TABLE_FIELD].GetArrayElements();
                foreach (var tableField in tableFields)
                {
                    var table = new FieldEncountTable.Sheettable();

                    table.zoneID = (ZoneID)tableField[ZONEID_FIELD].AsInt;
                    table.encRate_gr = tableField[ENCRATEGR_FIELD].AsInt;
                    table.ground_mons = tableField[GROUNDMONS_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();
                    table.tairyo = tableField[TAIRYO_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();
                    table.day = tableField[DAY_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();
                    table.night = tableField[NIGHT_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();
                    table.swayGrass = tableField[SWAYGRASS_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();
                    table.FormProb = tableField[FORMPROB_FIELD].GetArrayElements().Select(f => f.AsInt).ToList();
                    table.Nazo = tableField[NAZO_FIELD].GetArrayElements().Select(f => f.AsInt).ToList();
                    table.AnnoonTable = tableField[ANNOONTABLE_FIELD].GetArrayElements().Select(f => f.AsInt).ToList();
                    table.gbaRuby = tableField[GBARUBY_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();
                    table.gbaSapp = tableField[GBASAPP_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();
                    table.gbaEme = tableField[GBAEME_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();
                    table.gbaFire = tableField[GBAFIRE_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();
                    table.gbaLeaf = tableField[GBALEAF_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();
                    table.encRate_wat = tableField[ENCRATEWAT_FIELD].AsInt;
                    table.water_mons = tableField[WATERMONS_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();
                    table.encRate_turi_boro = tableField[ENCRATETURIBORO_FIELD].AsInt;
                    table.boro_mons = tableField[BOROMONS_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();
                    table.encRate_turi_ii = tableField[ENCRATETURIII_FIELD].AsInt;
                    table.ii_mons = tableField[IIMONS_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();
                    table.encRate_sugoi = tableField[ENCRATESUGOI_FIELD].AsInt;
                    table.sugoi_mons = tableField[SUGOIMONS_FIELD].GetArrayElements().Select(ParseMonsLv).ToList();

                    encountTable.table.Add(table);
                }

                encountTable.urayama = new();
                var urayamaFields = mono[URAYAMA_FIELD].GetArrayElements();
                foreach (var urayamaField in urayamaFields)
                {
                    var urayama = new FieldEncountTable.Sheeturayama();

                    urayama.monsNo = urayamaField[LOWERCASE_MONSNO_FIELD].AsInt;

                    encountTable.urayama.Add(urayama);
                }

                encountTable.mistu = new();
                var mistuFields = mono[MISTU_FIELD].GetArrayElements();
                foreach (var mistuField in mistuFields)
                {
                    var mistu = new FieldEncountTable.Sheetmistu();

                    mistu.Rate = mistuField[RATE_FIELD].AsInt;
                    mistu.Normal = mistuField[NORMAL_FIELD].AsInt;
                    mistu.Rare = mistuField[RARE_FIELD].AsInt;
                    mistu.SuperRare = mistuField[SUPERRARE_FIELD].AsInt;

                    encountTable.mistu.Add(mistu);
                }

                encountTable.honeytree = new();
                var honeyTreeFields = mono[HONEYTREE_FIELD].GetArrayElements();
                foreach (var honeyTreeField in honeyTreeFields)
                {
                    var honeyTree = new FieldEncountTable.Sheethoneytree();

                    honeyTree.Normal = honeyTreeField[NORMAL_FIELD].AsInt;
                    honeyTree.Rare = honeyTreeField[RARE_FIELD].AsInt;

                    encountTable.honeytree.Add(honeyTree);
                }

                encountTable.safari = new();
                var safariFields = mono[SAFARI_FIELD].GetArrayElements();
                foreach (var safariField in safariFields)
                {
                    var safari = new FieldEncountTable.Sheetsafari();

                    safari.MonsNo = safariField[MONSNO_FIELD].AsInt;

                    encountTable.safari.Add(safari);
                }

                encountTable.mvpoke = new();
                var mvpokeFields = mono[MVPOKE_FIELD].GetArrayElements();
                foreach (var mvpokeField in mvpokeFields)
                {
                    var mvpoke = new FieldEncountTable.Sheetmvpoke();

                    mvpoke.zoneID = (ZoneID)mvpokeField[ZONEID_FIELD].AsInt;
                    mvpoke.nextCount = mvpokeField[NEXTCOUNT_FIELD].AsInt;
                    mvpoke.nextZoneID = mvpokeField[NEXTZONEID_FIELD].GetArrayElements().Select(f => (ZoneID)f.AsInt).ToList();

                    encountTable.mvpoke.Add(mvpoke);
                }

                encountTable.legendpoke = new();
                var legendPokeFields = mono[LEGENDPOKE_FIELD].GetArrayElements();
                foreach (var legendPokeField in legendPokeFields)
                {
                    var legendPoke = new FieldEncountTable.Sheetlegendpoke();

                    legendPoke.monsNo = legendPokeField[LOWERCASE_MONSNO_FIELD].AsInt;
                    legendPoke.formNo = legendPokeField[FORMNO_FIELD].AsInt;
                    legendPoke.isFixedEncSeq = legendPokeField[ISFIXEDENCSEQ_FIELD].AsBool;
                    legendPoke.encSeq = legendPokeField[ENCSEQ_FIELD].AsString;
                    legendPoke.isFixedBGM = legendPokeField[ISFIXEDBGM_FIELD].AsBool;
                    legendPoke.bgmEvent = legendPokeField[BGMEVENT_FIELD].AsString;
                    legendPoke.isFixedBtlBg = legendPokeField[ISFIXEDBTLBG_FIELD].AsBool;
                    legendPoke.btlBg = legendPokeField[BTLBG_FIELD].AsInt;
                    legendPoke.isFixedSetupEffect = legendPokeField[ISFIXEDSETUPEFFECT_FIELD].AsBool;
                    legendPoke.setupEffect = legendPokeField[SETUPEFFECT_FIELD].AsInt;
                    legendPoke.waza1 = legendPokeField[WAZA1_FIELD].AsInt;
                    legendPoke.waza2 = legendPokeField[WAZA2_FIELD].AsInt;
                    legendPoke.waza3 = legendPokeField[WAZA3_FIELD].AsInt;
                    legendPoke.waza4 = legendPokeField[WAZA4_FIELD].AsInt;

                    encountTable.legendpoke.Add(legendPoke);
                }

                encountTable.zui = new();
                var zuiFields = mono[ZUI_FIELD].GetArrayElements();
                foreach (var zuiField in zuiFields)
                {
                    var zui = new FieldEncountTable.Sheetzui();

                    zui.zoneID = (ZoneID)zuiField[ZONEID_FIELD].AsInt;
                    zui.form = zuiField[FORM_FIELD].GetArrayElements().Select(f => f.AsBool).ToList();

                    encountTable.zui.Add(zui);
                }

                data.Add(encountTable);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, FieldEncountTableCollection data)
        {
            var gameSettingsBundle = fileManager.GetGameSettingsBundle();
            var monos = new List<KeyValuePair<long, AssetTypeValueField>>()
            {
                gameSettingsBundle.GetMonoByName(FIELDENCOUNTTABLED_MONONAME),
                gameSettingsBundle.GetMonoByName(FIELDENCOUNTTABLEP_MONONAME),
            };

            foreach (var (pathID, mono) in monos)
            {
                var encountTable = data.First(t => t.pathID == pathID);

                mono[TABLE_FIELD].SetArrayElementsAndInit(encountTable.table, (tableField, table) =>
                {
                    tableField[ZONEID_FIELD].AsInt = (int)table.zoneID;
                    tableField[ENCRATEGR_FIELD].AsInt = table.encRate_gr;
                    tableField[GROUNDMONS_FIELD].SetArrayElementsAndInit(table.ground_mons, SaveMonsLv);
                    tableField[TAIRYO_FIELD].SetArrayElementsAndInit(table.tairyo, SaveMonsLv);
                    tableField[DAY_FIELD].SetArrayElementsAndInit(table.day, SaveMonsLv);
                    tableField[NIGHT_FIELD].SetArrayElementsAndInit(table.night, SaveMonsLv);
                    tableField[SWAYGRASS_FIELD].SetArrayElementsAndInit(table.swayGrass, SaveMonsLv);
                    tableField[FORMPROB_FIELD].SetArrayElementsAndInit(table.FormProb, (f, v) => f.AsInt = v);
                    tableField[NAZO_FIELD].SetArrayElementsAndInit(table.Nazo, (f, v) => f.AsInt = v);
                    tableField[ANNOONTABLE_FIELD].SetArrayElementsAndInit(table.AnnoonTable, (f, v) => f.AsInt = v);
                    tableField[GBARUBY_FIELD].SetArrayElementsAndInit(table.gbaRuby, SaveMonsLv);
                    tableField[GBASAPP_FIELD].SetArrayElementsAndInit(table.gbaSapp, SaveMonsLv);
                    tableField[GBAEME_FIELD].SetArrayElementsAndInit(table.gbaEme, SaveMonsLv);
                    tableField[GBAFIRE_FIELD].SetArrayElementsAndInit(table.gbaFire, SaveMonsLv);
                    tableField[GBALEAF_FIELD].SetArrayElementsAndInit(table.gbaLeaf, SaveMonsLv);
                    tableField[ENCRATEWAT_FIELD].AsInt = table.encRate_wat;
                    tableField[WATERMONS_FIELD].SetArrayElementsAndInit(table.water_mons, SaveMonsLv);
                    tableField[ENCRATETURIBORO_FIELD].AsInt = table.encRate_turi_boro;
                    tableField[BOROMONS_FIELD].SetArrayElementsAndInit(table.boro_mons, SaveMonsLv);
                    tableField[ENCRATETURIII_FIELD].AsInt = table.encRate_turi_ii;
                    tableField[IIMONS_FIELD].SetArrayElementsAndInit(table.ii_mons, SaveMonsLv);
                    tableField[ENCRATESUGOI_FIELD].AsInt = table.encRate_sugoi;
                    tableField[SUGOIMONS_FIELD].SetArrayElementsAndInit(table.sugoi_mons, SaveMonsLv);
                });

                mono[URAYAMA_FIELD].SetArrayElementsAndInit(encountTable.urayama, (urayamaField, urayama) =>
                {
                    urayamaField[LOWERCASE_MONSNO_FIELD].AsInt = urayama.monsNo;
                });

                mono[MISTU_FIELD].SetArrayElementsAndInit(encountTable.mistu, (mistuField, mistu) =>
                {
                    mistuField[RATE_FIELD].AsInt = mistu.Rate;
                    mistuField[NORMAL_FIELD].AsInt = mistu.Normal;
                    mistuField[RARE_FIELD].AsInt = mistu.Rare;
                    mistuField[SUPERRARE_FIELD].AsInt = mistu.SuperRare;
                });

                mono[HONEYTREE_FIELD].SetArrayElementsAndInit(encountTable.honeytree, (honeyTreeField, honeyTree) =>
                {
                    honeyTreeField[NORMAL_FIELD].AsInt = honeyTree.Normal;
                    honeyTreeField[RARE_FIELD].AsInt = honeyTree.Rare;
                });

                mono[SAFARI_FIELD].SetArrayElementsAndInit(encountTable.safari, (safariField, safari) =>
                {
                    safariField[MONSNO_FIELD].AsInt = safari.MonsNo;
                });

                mono[MVPOKE_FIELD].SetArrayElementsAndInit(encountTable.mvpoke, (mvpokeField, mvpoke) =>
                {
                    mvpokeField[ZONEID_FIELD].AsInt = (int)mvpoke.zoneID;
                    mvpokeField[NEXTCOUNT_FIELD].AsInt = mvpoke.nextCount;
                    mvpokeField[NEXTZONEID_FIELD].SetArrayElementsAndInit(mvpoke.nextZoneID, (f, v) => f.AsInt = (int)v);
                });

                mono[LEGENDPOKE_FIELD].SetArrayElementsAndInit(encountTable.legendpoke, (legendPokeField, legendPoke) =>
                {
                    legendPokeField[LOWERCASE_MONSNO_FIELD].AsInt = legendPoke.monsNo;
                    legendPokeField[FORMNO_FIELD].AsInt = legendPoke.formNo;
                    legendPokeField[ISFIXEDENCSEQ_FIELD].AsBool = legendPoke.isFixedEncSeq;
                    legendPokeField[ENCSEQ_FIELD].AsString = legendPoke.encSeq;
                    legendPokeField[ISFIXEDBGM_FIELD].AsBool = legendPoke.isFixedBGM;
                    legendPokeField[BGMEVENT_FIELD].AsString = legendPoke.bgmEvent;
                    legendPokeField[ISFIXEDBTLBG_FIELD].AsBool = legendPoke.isFixedBtlBg;
                    legendPokeField[BTLBG_FIELD].AsInt = legendPoke.btlBg;
                    legendPokeField[ISFIXEDSETUPEFFECT_FIELD].AsBool = legendPoke.isFixedSetupEffect;
                    legendPokeField[SETUPEFFECT_FIELD].AsInt = legendPoke.setupEffect;
                    legendPokeField[WAZA1_FIELD].AsInt = legendPoke.waza1;
                    legendPokeField[WAZA2_FIELD].AsInt = legendPoke.waza2;
                    legendPokeField[WAZA3_FIELD].AsInt = legendPoke.waza3;
                    legendPokeField[WAZA4_FIELD].AsInt = legendPoke.waza4;
                });

                mono[ZUI_FIELD].SetArrayElementsAndInit(encountTable.zui, (zuiField, zui) =>
                {
                    zuiField[ZONEID_FIELD].AsInt = (int)zui.zoneID;
                    zuiField[FORM_FIELD].SetArrayElementsAndInit(zui.form, (f, v) => f.AsBool = v);
                });

                gameSettingsBundle.SetMonoByPathID(pathID, mono);
            }
        }

        private FieldEncountTable.Sheettable.MonsLv ParseMonsLv(AssetTypeValueField field)
        {
            return new FieldEncountTable.Sheettable.MonsLv()
            {
                maxlv = field[MAXLV_FIELD].AsInt,
                minlv = field[MINLV_FIELD].AsInt,
                monsNo = field[LOWERCASE_MONSNO_FIELD].AsInt,
            };
        }

        private void SaveMonsLv(AssetTypeValueField field, FieldEncountTable.Sheettable.MonsLv data)
        {
            field[MAXLV_FIELD].AsInt = data.maxlv;
            field[MINLV_FIELD].AsInt = data.minlv;
            field[LOWERCASE_MONSNO_FIELD].AsInt = data.monsNo;
        }
    }
}
