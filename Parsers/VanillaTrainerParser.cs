using AssetsTools.NET;
using ImpostersOrdeal.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ImpostersOrdeal
{
    public class VanillaTrainerParser : IParser<TrainerTable>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (TrainerTable)data);

        public IEnumerable<Type> GetRequiredDataSources() => [typeof(DprMasterdatasBundle)];

        // Monos
        private static readonly string TRAINERTABLE_MONONAME = "TrainerTable";

        // Arrays
        private static readonly string BTLEFFID_FIELD = "BtlEffId";
        private static readonly string FIELDENCOUNT_FIELD = "FieldEncount";
        private static readonly string SEALTEMPLATE_FIELD = "SealTemplate";
        private static readonly string SKIRTGRAPHICSCHARA_FIELD = "SkirtGraphicsChara";
        private static readonly string TRAINERDATA_FIELD = "TrainerData";
        private static readonly string TRAINERPOKE_FIELD = "TrainerPoke";
        private static readonly string TRAINERREMATCH_FIELD = "TrainerRematch";
        private static readonly string TRAINERTYPE_FIELD = "TrainerType";

        // Fields
        private static readonly string AGE_FIELD = "Age";
        private static readonly string AIBIT_FIELD = "AIBit";
        private static readonly string ARENAID_FIELD = "ArenaID";
        private static readonly string BALLID_FIELD = "BallId";
        private static readonly string BASETRAINERID_FIELD = "BaseTrainerID";
        private static readonly string CAPTURETHROWTIME_FIELD = "CaptureThrowTime";
        private static readonly string COLORID_FIELD = "ColorID";
        private static readonly string EFFECTID_FIELD = "EffectID";
        private static readonly string EYEBGM_FIELD = "EyeBgm";
        private static readonly string FIGHTTYPE_FIELD = "FightType";
        private static readonly string GIFTITEM_FIELD = "GiftItem";
        private static readonly string GOLD_FIELD = "Gold";
        private static readonly string GROUP_FIELD = "Group";
        private static readonly string HAND_FIELD = "Hand";
        private static readonly string HELPHAND_FIELD = "HelpHand";
        private static readonly string HELPHOLDBALLHAND_FIELD = "HelpHoldBallHand";
        private static readonly string HOLDBALLHAND_FIELD = "HoldBallHand";
        private static readonly string HPRECOVERFLAG_FIELD = "HpRecoverFlag";
        private static readonly string LABELTRTYPE_FIELD = "LabelTrType";
        private static readonly string LOSELOOPTIME_FIELD = "LoseLoopTime";
        private static readonly string MNAME_FIELD = "m_Name";
        private static readonly string MODELID_FIELD = "ModelID";
        private static readonly string MSGBATTLE_FIELD = "MsgBattle";
        private static readonly string MSGFIELDAFTER_FIELD = "MsgFieldAfter";
        private static readonly string MSGFIELDBEFORE_FIELD = "MsgFieldBefore";
        private static readonly string MSGFIELDPOKEONE_FIELD = "MsgFieldPokeOne";
        private static readonly string MSGFIELDREVENGE_FIELD = "MsgFieldRevenge";
        private static readonly string NAMELABEL_FIELD = "NameLabel";
        private static readonly string POKEBALL_GENERIC_FIELD = "P{0}Ball";
        private static readonly string POKEEFFORTAGI_GENERIC_FIELD = "P{0}EffortAgi";
        private static readonly string POKEEFFORTATK_GENERIC_FIELD = "P{0}EffortAtk";
        private static readonly string POKEEFFORTDEF_GENERIC_FIELD = "P{0}EffortDef";
        private static readonly string POKEEFFORTHP_GENERIC_FIELD = "P{0}EffortHp";
        private static readonly string POKEEFFORTSPATK_GENERIC_FIELD = "P{0}EffortSpAtk";
        private static readonly string POKEEFFORTSPDEF_GENERIC_FIELD = "P{0}EffortSpDef";
        private static readonly string POKEFORMNO_GENERIC_FIELD = "P{0}FormNo";
        private static readonly string POKEISRARE_GENERIC_FIELD = "P{0}IsRare";
        private static readonly string POKEITEM_GENERIC_FIELD = "P{0}Item";
        private static readonly string POKELEVEL_GENERIC_FIELD = "P{0}Level";
        private static readonly string POKEMONSNO_GENERIC_FIELD = "P{0}MonsNo";
        private static readonly string POKESEAL_GENERIC_FIELD = "P{0}Seal";
        private static readonly string POKESEIKAKU_GENERIC_FIELD = "P{0}Seikaku";
        private static readonly string POKESEX_GENERIC_FIELD = "P{0}Sex";
        private static readonly string POKETALENTAGI_GENERIC_FIELD = "P{0}TalentAgi";
        private static readonly string POKETALENTATK_GENERIC_FIELD = "P{0}TalentAtk";
        private static readonly string POKETALENTDEF_GENERIC_FIELD = "P{0}TalentDef";
        private static readonly string POKETALENTHP_GENERIC_FIELD = "P{0}TalentHp";
        private static readonly string POKETALENTSPATK_GENERIC_FIELD = "P{0}TalentSpAtk";
        private static readonly string POKETALENTSPDEF_GENERIC_FIELD = "P{0}TalentSpDef";
        private static readonly string POKETOKUSEI_GENERIC_FIELD = "P{0}Tokusei";
        private static readonly string POKEWAZA1_GENERIC_FIELD = "P{0}Waza1";
        private static readonly string POKEWAZA2_GENERIC_FIELD = "P{0}Waza2";
        private static readonly string POKEWAZA3_GENERIC_FIELD = "P{0}Waza3";
        private static readonly string POKEWAZA4_GENERIC_FIELD = "P{0}Waza4";
        private static readonly string POS_GENERIC_FIELD = "Pos{0}";
        private static readonly string REMATCH_GENERIC_FIELD = "Rematch_0{0}";
        private static readonly string SEALID_GENERIC_FIELD = "SealID{0}";
        private static readonly string SEQBATTLE_FIELD = "SeqBattle";
        private static readonly string SEX_FIELD = "Sex";
        private static readonly string SKIRTGRAPHICSID_FIELD = "SkirtGraphicsID";
        private static readonly string THROWTIME_FIELD = "ThrowTime";
        private static readonly string TRAINEREFFECT_FIELD = "TrainerEffect";
        private static readonly string TRAINERID_FIELD = "TrainerID";
        private static readonly string TYPEID_FIELD = "TypeID";
        private static readonly string USEITEM_GENERIC_FIELD = "UseItem{0}";

        public TrainerTable ParseFromSources(FileManager fileManager)
        {
            var data = new TrainerTable();

            var dprMasterdatasBundle = fileManager.GetDprMasterdatasBundle();
            var (pathID, monoBehaviour) = dprMasterdatasBundle.GetMonoByName(TRAINERTABLE_MONONAME);

            data.pathID = pathID;
            data.m_Name = monoBehaviour[MNAME_FIELD].AsString;

            data.TrainerType = new();
            var trainerTypeFields = monoBehaviour[TRAINERTYPE_FIELD].GetArrayElements();
            foreach (var trainerTypeField in trainerTypeFields)
            {
                var trainerType = new TrainerTable.SheetTrainerType();

                trainerType.TypeID = trainerTypeField[TRAINERID_FIELD].AsInt;
                trainerType.LabelTrType = trainerTypeField[LABELTRTYPE_FIELD].AsString;
                trainerType.Sex = trainerTypeField[SEX_FIELD].AsByte;
                trainerType.Group = trainerTypeField[GROUP_FIELD].AsByte;
                trainerType.BallId = trainerTypeField[BALLID_FIELD].AsByte;
                trainerType.FieldEncount = trainerTypeField[FIELDENCOUNT_FIELD].GetArrayElements().Select(f => f.AsString).ToList();
                trainerType.BtlEffId = trainerTypeField[BTLEFFID_FIELD].GetArrayElements().Select(f => f.AsInt).ToList();
                trainerType.EyeBgm = trainerTypeField[EYEBGM_FIELD].AsString;
                trainerType.ModelID = trainerTypeField[MODELID_FIELD].AsString;
                trainerType.Hand = trainerTypeField[HAND_FIELD].AsByte;
                trainerType.HoldBallHand = trainerTypeField[HOLDBALLHAND_FIELD].AsByte;
                trainerType.HelpHand = trainerTypeField[HELPHAND_FIELD].AsByte;
                trainerType.HelpHoldBallHand = trainerTypeField[HELPHOLDBALLHAND_FIELD].AsByte;
                trainerType.ThrowTime = trainerTypeField[THROWTIME_FIELD].AsFloat;
                trainerType.CaptureThrowTime = trainerTypeField[CAPTURETHROWTIME_FIELD].AsFloat;
                trainerType.LoseLoopTime = trainerTypeField[LOSELOOPTIME_FIELD].AsFloat;
                trainerType.TrainerEffect = trainerTypeField[TRAINEREFFECT_FIELD].AsString;
                trainerType.Age = trainerTypeField[AGE_FIELD].AsByte;

                data.TrainerType.Add(trainerType);
            }

            data.TrainerData = new();
            var trainerDataFields = monoBehaviour[TRAINERDATA_FIELD].GetArrayElements();
            var trainerPokeFields = monoBehaviour[TRAINERPOKE_FIELD].GetArrayElements();
            var trainerRematchFields = monoBehaviour[TRAINERREMATCH_FIELD].GetArrayElements();
            for (int i=0; i<trainerDataFields.Count; i++)
            {
                var trainerDataField = trainerDataFields[i];
                var trainerPokeField = trainerPokeFields[i];

                var trainerData = new TrainerTable.SheetTrainerData();

                trainerData.TypeID = trainerDataField[TYPEID_FIELD].AsInt;
                trainerData.ColorID = trainerDataField[COLORID_FIELD].AsByte;
                trainerData.FightType = trainerDataField[FIGHTTYPE_FIELD].AsByte;
                trainerData.ArenaID = trainerDataField[ARENAID_FIELD].AsInt;
                trainerData.EffectID = trainerDataField[EFFECTID_FIELD].AsInt;
                trainerData.Gold = trainerDataField[GOLD_FIELD].AsByte;

                trainerData.UseItem = new();
                for (int j=1; j<=4; j++)
                {
                    var useItemFieldName = string.Format(USEITEM_GENERIC_FIELD, IntHelper.ConvertToString(j));
                    var useItem = trainerDataField[useItemFieldName].AsUShort;
                    if (useItem == 0)
                        break;

                    trainerData.UseItem.Add(useItem);
                }

                trainerData.HpRecoverFlag = trainerDataField[HPRECOVERFLAG_FIELD].AsBool;
                trainerData.GiftItem = trainerDataField[GIFTITEM_FIELD].AsUShort;
                trainerData.NameLabel = trainerDataField[NAMELABEL_FIELD].AsString;
                trainerData.MsgFieldPokeOne = trainerDataField[MSGFIELDPOKEONE_FIELD].AsString;
                trainerData.MsgFieldBefore = trainerDataField[MSGFIELDBEFORE_FIELD].AsString;
                trainerData.MsgFieldRevenge = trainerDataField[MSGFIELDREVENGE_FIELD].AsString;
                trainerData.MsgFieldAfter = trainerDataField[MSGFIELDAFTER_FIELD].AsString;

                trainerData.MsgBattle = trainerDataField[MSGBATTLE_FIELD].GetArrayElements().Select(f => f.AsString).ToList();
                trainerData.SeqBattle = trainerDataField[SEQBATTLE_FIELD].GetArrayElements().Select(f => f.AsString).ToList();
                trainerData.AIBit = trainerDataField[AIBIT_FIELD].AsUInt;

                trainerData.Pokes = new();
                for (int j=1; j<=6; j++)
                {
                    var monsno = trainerPokeField[string.Format(POKEMONSNO_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsUShort;
                    if (monsno == 0)
                        break;

                    var trainerPoke = new TrainerTable.SheetTrainerData.TrainerPoke();

                    trainerPoke.MonsNo = trainerPokeField[string.Format(POKEMONSNO_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsUShort;
                    trainerPoke.FormNo = trainerPokeField[string.Format(POKEFORMNO_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsUShort;
                    trainerPoke.IsRare = trainerPokeField[string.Format(POKEISRARE_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsBool;
                    trainerPoke.Level = trainerPokeField[string.Format(POKELEVEL_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.Sex = trainerPokeField[string.Format(POKESEX_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.Seikaku = trainerPokeField[string.Format(POKESEIKAKU_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.Tokusei = trainerPokeField[string.Format(POKETOKUSEI_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsUShort;
                    trainerPoke.Waza1 = trainerPokeField[string.Format(POKEWAZA1_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsUShort;
                    trainerPoke.Waza2 = trainerPokeField[string.Format(POKEWAZA2_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsUShort;
                    trainerPoke.Waza3 = trainerPokeField[string.Format(POKEWAZA3_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsUShort;
                    trainerPoke.Waza4 = trainerPokeField[string.Format(POKEWAZA4_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsUShort;
                    trainerPoke.Item = trainerPokeField[string.Format(POKEITEM_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsUShort;
                    trainerPoke.Ball = trainerPokeField[string.Format(POKEBALL_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.Seal = trainerPokeField[string.Format(POKESEAL_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsInt;
                    trainerPoke.TalentHp = trainerPokeField[string.Format(POKETALENTHP_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.TalentAtk = trainerPokeField[string.Format(POKETALENTATK_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.TalentDef = trainerPokeField[string.Format(POKETALENTDEF_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.TalentSpAtk = trainerPokeField[string.Format(POKETALENTSPATK_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.TalentSpDef = trainerPokeField[string.Format(POKETALENTSPDEF_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.TalentAgi = trainerPokeField[string.Format(POKETALENTAGI_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.EffortHp = trainerPokeField[string.Format(POKEEFFORTHP_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.EffortAtk = trainerPokeField[string.Format(POKEEFFORTATK_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.EffortDef = trainerPokeField[string.Format(POKEEFFORTDEF_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.EffortSpAtk = trainerPokeField[string.Format(POKEEFFORTSPATK_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.EffortSpDef = trainerPokeField[string.Format(POKEEFFORTSPDEF_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;
                    trainerPoke.EffortAgi = trainerPokeField[string.Format(POKEEFFORTAGI_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsByte;

                    trainerData.Pokes.Add(trainerPoke);
                }

                trainerData.Rematches = new();
                var trainerRematchField = trainerRematchFields.Find(f => f[BASETRAINERID_FIELD].AsInt == i);
                if (trainerRematchField != null)
                {
                    for (int j=1; j<=5; j++)
                        trainerData.Rematches.Add(trainerRematchField[string.Format(REMATCH_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsInt);
                }

                data.TrainerData.Add(trainerData);
            }

            data.SealTemplates = new();
            var sealTemplateFields = monoBehaviour[SEALTEMPLATE_FIELD].GetArrayElements();
            foreach (var sealTemplateField in sealTemplateFields)
            {
                var sealTemplate = new TrainerTable.SealTemplate();

                sealTemplate.Seals = new();
                for (int i=1; i<=20; i++)
                {
                    var sealID = sealTemplateField[string.Format(SEALID_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsInt;
                    if (sealID == 0)
                        break;

                    var seal = new TrainerTable.SealTemplate.Seal();

                    seal.SealID = sealID;
                    seal.Pos = sealTemplateField[string.Format(POS_GENERIC_FIELD, IntHelper.ConvertToString(i))].GetAsVector3();

                    sealTemplate.Seals.Add(seal);
                }

                data.SealTemplates.Add(sealTemplate);
            }
            
            data.SkirtGraphicsChara = new();
            var skirtFields = monoBehaviour[SKIRTGRAPHICSCHARA_FIELD].GetArrayElements();
            foreach (var skirtField in skirtFields)
            {
                var skirt = new TrainerTable.SheetSkirtGraphicsChara();

                skirt.SkirtGraphicsID = skirtField[SKIRTGRAPHICSID_FIELD].AsString;

                data.SkirtGraphicsChara.Add(skirt);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, TrainerTable data)
        {
            var dprMasterdatasBundle = fileManager.GetDprMasterdatasBundle();
            var (pathID, monoBehaviour) = dprMasterdatasBundle.GetMonoByName(TRAINERTABLE_MONONAME);

            monoBehaviour[TRAINERTYPE_FIELD].SetArrayElementsAndInit(data.TrainerType, (trainerTypeField, trainerType) =>
            {
                trainerTypeField[TRAINERID_FIELD].AsInt = trainerType.TypeID;
                trainerTypeField[LABELTRTYPE_FIELD].AsString = trainerType.LabelTrType;
                trainerTypeField[SEX_FIELD].AsByte = trainerType.Sex;
                trainerTypeField[GROUP_FIELD].AsByte = trainerType.Group;
                trainerTypeField[BALLID_FIELD].AsByte = trainerType.BallId;
                trainerTypeField[FIELDENCOUNT_FIELD].SetArrayElementsAndInit(trainerType.FieldEncount, (f, v) => f.AsString = v);
                trainerTypeField[BTLEFFID_FIELD].SetArrayElementsAndInit(trainerType.BtlEffId, (f, v) => f.AsInt = v);
                trainerTypeField[EYEBGM_FIELD].AsString = trainerType.EyeBgm;
                trainerTypeField[MODELID_FIELD].AsString = trainerType.ModelID;
                trainerTypeField[HAND_FIELD].AsByte = trainerType.Hand;
                trainerTypeField[HOLDBALLHAND_FIELD].AsByte = trainerType.HoldBallHand;
                trainerTypeField[HELPHAND_FIELD].AsByte = trainerType.HelpHand;
                trainerTypeField[HELPHOLDBALLHAND_FIELD].AsByte = trainerType.HelpHoldBallHand;
                trainerTypeField[THROWTIME_FIELD].AsFloat = trainerType.ThrowTime;
                trainerTypeField[CAPTURETHROWTIME_FIELD].AsFloat = trainerType.CaptureThrowTime;
                trainerTypeField[LOSELOOPTIME_FIELD].AsFloat = trainerType.LoseLoopTime;
                trainerTypeField[TRAINEREFFECT_FIELD].AsString = trainerType.TrainerEffect;
                trainerTypeField[AGE_FIELD].AsByte = trainerType.Age;
            });

            monoBehaviour[TRAINERDATA_FIELD].SetArrayElementsAndInit(data.TrainerData, (trainerDataField, trainerData) =>
            {
                trainerDataField[TYPEID_FIELD].AsInt = trainerData.TypeID;
                trainerDataField[COLORID_FIELD].AsByte = trainerData.ColorID;
                trainerDataField[FIGHTTYPE_FIELD].AsByte = trainerData.FightType;
                trainerDataField[ARENAID_FIELD].AsInt = trainerData.ArenaID;
                trainerDataField[EFFECTID_FIELD].AsInt = trainerData.EffectID;
                trainerDataField[GOLD_FIELD].AsByte = trainerData.Gold;

                for (int i=1; i<=4; i++)
                {
                    var useItemFieldName = string.Format(USEITEM_GENERIC_FIELD, IntHelper.ConvertToString(i));
                    if (trainerData.UseItem.Count >= i)
                        trainerDataField[useItemFieldName].AsUShort = trainerData.UseItem[i-1];
                    else
                        trainerDataField[useItemFieldName].AsUShort = 0;
                }

                trainerDataField[HPRECOVERFLAG_FIELD].AsBool = trainerData.HpRecoverFlag;
                trainerDataField[GIFTITEM_FIELD].AsUShort = trainerData.GiftItem;
                trainerDataField[NAMELABEL_FIELD].AsString = trainerData.NameLabel;
                trainerDataField[MSGFIELDPOKEONE_FIELD].AsString = trainerData.MsgFieldPokeOne;
                trainerDataField[MSGFIELDBEFORE_FIELD].AsString = trainerData.MsgFieldBefore;
                trainerDataField[MSGFIELDREVENGE_FIELD].AsString = trainerData.MsgFieldRevenge;
                trainerDataField[MSGFIELDAFTER_FIELD].AsString = trainerData.MsgFieldAfter;

                trainerDataField[MSGBATTLE_FIELD].SetArrayElementsAndInit(trainerData.MsgBattle, (f, v) => f.AsString = v);
                trainerDataField[SEQBATTLE_FIELD].SetArrayElementsAndInit(trainerData.SeqBattle, (f, v) => f.AsString = v);
                trainerDataField[AIBIT_FIELD].AsUInt = trainerData.AIBit;
            });

            monoBehaviour[TRAINERPOKE_FIELD].SetArrayElementsAndInit(data.TrainerData, (trainerPokeField, trainerData) =>
            {
                for (int i=1; i<=6; i++)
                {
                    if (trainerData.Pokes.Count >= i)
                    {
                        var trainerPoke = trainerData.Pokes[i-1];

                        trainerPokeField[string.Format(POKEMONSNO_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = trainerPoke.MonsNo;
                        trainerPokeField[string.Format(POKEFORMNO_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = trainerPoke.FormNo;
                        trainerPokeField[string.Format(POKEISRARE_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsBool = trainerPoke.IsRare;
                        trainerPokeField[string.Format(POKELEVEL_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.Level;
                        trainerPokeField[string.Format(POKESEX_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.Sex;
                        trainerPokeField[string.Format(POKESEIKAKU_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.Seikaku;
                        trainerPokeField[string.Format(POKETOKUSEI_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = trainerPoke.Tokusei;
                        trainerPokeField[string.Format(POKEWAZA1_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = trainerPoke.Waza1;
                        trainerPokeField[string.Format(POKEWAZA2_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = trainerPoke.Waza2;
                        trainerPokeField[string.Format(POKEWAZA3_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = trainerPoke.Waza3;
                        trainerPokeField[string.Format(POKEWAZA4_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = trainerPoke.Waza4;
                        trainerPokeField[string.Format(POKEITEM_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = trainerPoke.Item;
                        trainerPokeField[string.Format(POKEBALL_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.Ball;
                        trainerPokeField[string.Format(POKESEAL_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsInt = trainerPoke.Seal;
                        trainerPokeField[string.Format(POKETALENTHP_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.TalentHp;
                        trainerPokeField[string.Format(POKETALENTATK_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.TalentAtk;
                        trainerPokeField[string.Format(POKETALENTDEF_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.TalentDef;
                        trainerPokeField[string.Format(POKETALENTSPATK_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.TalentSpAtk;
                        trainerPokeField[string.Format(POKETALENTSPDEF_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.TalentSpDef;
                        trainerPokeField[string.Format(POKETALENTAGI_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.TalentAgi;
                        trainerPokeField[string.Format(POKEEFFORTHP_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.EffortHp;
                        trainerPokeField[string.Format(POKEEFFORTATK_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.EffortAtk;
                        trainerPokeField[string.Format(POKEEFFORTDEF_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.EffortDef;
                        trainerPokeField[string.Format(POKEEFFORTSPATK_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.EffortSpAtk;
                        trainerPokeField[string.Format(POKEEFFORTSPDEF_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.EffortSpDef;
                        trainerPokeField[string.Format(POKEEFFORTAGI_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = trainerPoke.EffortAgi;
                    }
                    else
                    {
                        trainerPokeField[string.Format(POKEMONSNO_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = 0;
                        trainerPokeField[string.Format(POKEFORMNO_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = 0;
                        trainerPokeField[string.Format(POKEISRARE_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsBool = false;
                        trainerPokeField[string.Format(POKELEVEL_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                        trainerPokeField[string.Format(POKESEX_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 3;
                        trainerPokeField[string.Format(POKESEIKAKU_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                        trainerPokeField[string.Format(POKETOKUSEI_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = 0;
                        trainerPokeField[string.Format(POKEWAZA1_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = 0;
                        trainerPokeField[string.Format(POKEWAZA2_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = 0;
                        trainerPokeField[string.Format(POKEWAZA3_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = 0;
                        trainerPokeField[string.Format(POKEWAZA4_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = 0;
                        trainerPokeField[string.Format(POKEITEM_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsUShort = 0;
                        trainerPokeField[string.Format(POKEBALL_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 28;
                        trainerPokeField[string.Format(POKESEAL_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsInt = -1;
                        trainerPokeField[string.Format(POKETALENTHP_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                        trainerPokeField[string.Format(POKETALENTATK_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                        trainerPokeField[string.Format(POKETALENTDEF_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                        trainerPokeField[string.Format(POKETALENTSPATK_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                        trainerPokeField[string.Format(POKETALENTSPDEF_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                        trainerPokeField[string.Format(POKETALENTAGI_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                        trainerPokeField[string.Format(POKEEFFORTHP_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                        trainerPokeField[string.Format(POKEEFFORTATK_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                        trainerPokeField[string.Format(POKEEFFORTDEF_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                        trainerPokeField[string.Format(POKEEFFORTSPATK_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                        trainerPokeField[string.Format(POKEEFFORTSPDEF_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                        trainerPokeField[string.Format(POKEEFFORTAGI_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsByte = 0;
                    }
                }
            });

            List<AssetTypeValueField> newtrainerRematches = new();
            for (int i=0; i<data.TrainerData.Count; i++)
            {
                var trainerData = data.TrainerData[i];
                if (trainerData.Rematches.Count <= 0)
                    continue;

                AssetTypeValueField trainerRematchField = monoBehaviour[TRAINERREMATCH_FIELD].CreateArrayElement();

                trainerRematchField[BASETRAINERID_FIELD].AsInt = i;

                for (int j=1; j<=5; j++)
                {
                    if (trainerData.Rematches.Count >= j)
                        trainerRematchField[string.Format(REMATCH_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsInt = trainerData.Rematches[j-1];
                    else
                        trainerRematchField[string.Format(REMATCH_GENERIC_FIELD, IntHelper.ConvertToString(j))].AsInt = 707;
                }

                newtrainerRematches.Add(trainerRematchField);
            }
            monoBehaviour[TRAINERREMATCH_FIELD].SetArrayElements(newtrainerRematches);

            monoBehaviour[SEALTEMPLATE_FIELD].SetArrayElementsAndInit(data.SealTemplates, (sealTemplateField, sealTemplate) =>
            {
                for (int i=1; i<=20; i++)
                {
                    if (sealTemplate.Seals.Count >= i)
                    {
                        var seal = sealTemplate.Seals[i-1];

                        sealTemplateField[string.Format(SEALID_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsInt = seal.SealID;
                        sealTemplateField[string.Format(POS_GENERIC_FIELD, IntHelper.ConvertToString(i))].SetAsVector3(seal.Pos);
                    }
                    else
                    {
                        sealTemplateField[string.Format(SEALID_GENERIC_FIELD, IntHelper.ConvertToString(i))].AsInt = 0;
                        sealTemplateField[string.Format(POS_GENERIC_FIELD, IntHelper.ConvertToString(i))].SetAsVector3(Vector3.Zero);
                    }
                }
            });

            monoBehaviour[SKIRTGRAPHICSCHARA_FIELD].SetArrayElementsAndInit(data.SkirtGraphicsChara, (skirtField, skirt) =>
            {
                skirtField[SKIRTGRAPHICSID_FIELD].AsString = skirt.SkirtGraphicsID;
            });

            dprMasterdatasBundle.SetMonoByPathID(pathID, monoBehaviour);
        }
    }
}
