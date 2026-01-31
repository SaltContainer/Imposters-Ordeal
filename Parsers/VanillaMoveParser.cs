using ImpostersOrdeal.Utils;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ImpostersOrdeal
{
    public class VanillaMoveParser : IParser<MoveTable>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (MoveTable)data);

        public IEnumerable<Type> GetRequiredDataSources() => [typeof(PersonalMasterdatasBundle)];

        // Monos
        private static readonly string WAZATABLE_MONONAME = "WazaTable";

        // Arrays
        private static readonly string WAZA_FIELD = "Waza";
        private static readonly string YUBIWOHURU_FIELD = "Yubiwohuru";

        // Fields
        private static readonly string AISEQNO_FIELD = "aiSeqNo";
        private static readonly string BASEPP_FIELD = "basePP";
        private static readonly string CATEGORY_FIELD = "category";
        private static readonly string CONTESTWAZANO_FIELD = "contestWazaNo";
        private static readonly string CRITICALRANK_FIELD = "criticalRank";
        private static readonly string DAMAGERECOVERRATIO_FIELD = "damageRecoverRatio";
        private static readonly string DAMAGETYPE_FIELD = "damageType";
        private static readonly string FLAGS_FIELD = "flags";
        private static readonly string HITCOUNTMAX_FIELD = "hitCountMax";
        private static readonly string HITCOUNTMIN_FIELD = "hitCountMin";
        private static readonly string HITPER_FIELD = "hitPer";
        private static readonly string HPRECOVERRATIO_FIELD = "hpRecoverRatio";
        private static readonly string ISVALID_FIELD = "isValid";
        private static readonly string MNAME_FIELD = "m_Name";
        private static readonly string POWER_FIELD = "power";
        private static readonly string PRIORITY_FIELD = "priority";
        private static readonly string RANKEFFPER1_FIELD = "rankEffPer1";
        private static readonly string RANKEFFPER2_FIELD = "rankEffPer2";
        private static readonly string RANKEFFPER3_FIELD = "rankEffPer3";
        private static readonly string RANKEFFTYPE1_FIELD = "rankEffType1";
        private static readonly string RANKEFFTYPE2_FIELD = "rankEffType2";
        private static readonly string RANKEFFTYPE3_FIELD = "rankEffType3";
        private static readonly string RANKEFFVALUE1_FIELD = "rankEffValue1";
        private static readonly string RANKEFFVALUE2_FIELD = "rankEffValue2";
        private static readonly string RANKEFFVALUE3_FIELD = "rankEffValue3";
        private static readonly string SHRINKPER_FIELD = "shrinkPer";
        private static readonly string SICKCONT_FIELD = "sickCont";
        private static readonly string SICKID_FIELD = "sickID";
        private static readonly string SICKPER_FIELD = "sickPer";
        private static readonly string SICKTURNMAX_FIELD = "sickTurnMax";
        private static readonly string SICKTURNMIN_FIELD = "sickTurnMin";
        private static readonly string TARGET_FIELD = "target";
        private static readonly string TYPE_FIELD = "type";
        private static readonly string WAZANO_FIELD = "wazaNo";
        private static readonly string WAZANOS_FIELD = "wazaNos";

        public MoveTable ParseFromSources(FileManager fileManager)
        {
            var data = new MoveTable();

            var personalMasterdatasBundle = fileManager.GetPersonalMasterdatasBundle();
            var (pathId, mono) = personalMasterdatasBundle.GetMonoByName(WAZATABLE_MONONAME);

            data.pathID = pathId;
            data.m_Name = mono[MNAME_FIELD].AsString;

            data.Waza = new();
            var wazaFields = mono[WAZA_FIELD].GetArrayElements();
            foreach (var wazaField in wazaFields)
            {
                var waza = new MoveTable.SheetWaza();

                waza.wazaNo = wazaField[WAZANO_FIELD].AsInt;
                waza.isValid = wazaField[ISVALID_FIELD].AsBool;
                waza.type = wazaField[TYPE_FIELD].AsByte;
                waza.category = wazaField[CATEGORY_FIELD].AsByte;
                waza.damageType = wazaField[DAMAGETYPE_FIELD].AsByte;
                waza.power = wazaField[POWER_FIELD].AsByte;
                waza.hitPer = wazaField[HITPER_FIELD].AsByte;
                waza.basePP = wazaField[BASEPP_FIELD].AsByte;
                waza.priority = wazaField[PRIORITY_FIELD].AsSByte;
                waza.hitCountMax = wazaField[HITCOUNTMAX_FIELD].AsByte;
                waza.hitCountMin = wazaField[HITCOUNTMIN_FIELD].AsByte;
                waza.sickID = wazaField[SICKID_FIELD].AsUShort;
                waza.sickPer = wazaField[SICKPER_FIELD].AsByte;
                waza.sickCont = wazaField[SICKCONT_FIELD].AsByte;
                waza.sickTurnMin = wazaField[SICKTURNMIN_FIELD].AsByte;
                waza.sickTurnMax = wazaField[SICKTURNMAX_FIELD].AsByte;
                waza.criticalRank = wazaField[CRITICALRANK_FIELD].AsByte;
                waza.shrinkPer = wazaField[SHRINKPER_FIELD].AsByte;
                waza.aiSeqNo = wazaField[AISEQNO_FIELD].AsUShort;
                waza.damageRecoverRatio = wazaField[DAMAGERECOVERRATIO_FIELD].AsSByte;
                waza.hpRecoverRatio = wazaField[HPRECOVERRATIO_FIELD].AsSByte;
                waza.target = wazaField[TARGET_FIELD].AsByte;
                waza.rankEffType1 = wazaField[RANKEFFTYPE1_FIELD].AsByte;
                waza.rankEffType2 = wazaField[RANKEFFTYPE2_FIELD].AsByte;
                waza.rankEffType3 = wazaField[RANKEFFTYPE3_FIELD].AsByte;
                waza.rankEffValue1 = wazaField[RANKEFFVALUE1_FIELD].AsSByte;
                waza.rankEffValue2 = wazaField[RANKEFFVALUE2_FIELD].AsSByte;
                waza.rankEffValue3 = wazaField[RANKEFFVALUE3_FIELD].AsSByte;
                waza.rankEffPer1 = wazaField[RANKEFFPER1_FIELD].AsByte;
                waza.rankEffPer2 = wazaField[RANKEFFPER2_FIELD].AsByte;
                waza.rankEffPer3 = wazaField[RANKEFFPER3_FIELD].AsByte;
                waza.flags = wazaField[FLAGS_FIELD].AsUInt;
                waza.contestWazaNo = wazaField[CONTESTWAZANO_FIELD].AsUInt;

                data.Waza.Add(waza);
            }

            data.Yubiwohuru = new();
            var yubiwohuruFields = mono[YUBIWOHURU_FIELD].GetArrayElements();
            foreach (var yubiwohuruField in yubiwohuruFields)
            {
                var yubiwohuru = new MoveTable.SheetYubiwohuru();

                yubiwohuru.wazaNos = yubiwohuruField[WAZANOS_FIELD].GetArrayElements().Select(f => f.AsUShort).ToList();

                data.Yubiwohuru.Add(yubiwohuru);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, MoveTable data)
        {
            var personalMasterdatasBundle = fileManager.GetPersonalMasterdatasBundle();
            var (pathId, mono) = personalMasterdatasBundle.GetMonoByName(WAZATABLE_MONONAME);

            mono[WAZA_FIELD].SetArrayElementsAndInit(data.Waza, (wazaField, waza) =>
            {
                wazaField[WAZANO_FIELD].AsInt = waza.wazaNo;
                wazaField[ISVALID_FIELD].AsBool = waza.isValid;
                wazaField[TYPE_FIELD].AsByte = waza.type;
                wazaField[CATEGORY_FIELD].AsByte = waza.category;
                wazaField[DAMAGETYPE_FIELD].AsByte = waza.damageType;
                wazaField[POWER_FIELD].AsByte = waza.power;
                wazaField[HITPER_FIELD].AsByte = waza.hitPer;
                wazaField[BASEPP_FIELD].AsByte = waza.basePP;
                wazaField[PRIORITY_FIELD].AsSByte = waza.priority;
                wazaField[HITCOUNTMAX_FIELD].AsByte = waza.hitCountMax;
                wazaField[HITCOUNTMIN_FIELD].AsByte = waza.hitCountMin;
                wazaField[SICKID_FIELD].AsUShort = waza.sickID;
                wazaField[SICKPER_FIELD].AsByte = waza.sickPer;
                wazaField[SICKCONT_FIELD].AsByte = waza.sickCont;
                wazaField[SICKTURNMIN_FIELD].AsByte = waza.sickTurnMin;
                wazaField[SICKTURNMAX_FIELD].AsByte = waza.sickTurnMax;
                wazaField[CRITICALRANK_FIELD].AsByte = waza.criticalRank;
                wazaField[SHRINKPER_FIELD].AsByte = waza.shrinkPer;
                wazaField[AISEQNO_FIELD].AsUShort = waza.aiSeqNo;
                wazaField[DAMAGERECOVERRATIO_FIELD].AsSByte = waza.damageRecoverRatio;
                wazaField[HPRECOVERRATIO_FIELD].AsSByte = waza.hpRecoverRatio;
                wazaField[TARGET_FIELD].AsByte = waza.target;
                wazaField[RANKEFFTYPE1_FIELD].AsByte = waza.rankEffType1;
                wazaField[RANKEFFTYPE2_FIELD].AsByte = waza.rankEffType2;
                wazaField[RANKEFFTYPE3_FIELD].AsByte = waza.rankEffType3;
                wazaField[RANKEFFVALUE1_FIELD].AsSByte = waza.rankEffValue1;
                wazaField[RANKEFFVALUE2_FIELD].AsSByte = waza.rankEffValue2;
                wazaField[RANKEFFVALUE3_FIELD].AsSByte = waza.rankEffValue3;
                wazaField[RANKEFFPER1_FIELD].AsByte = waza.rankEffPer1;
                wazaField[RANKEFFPER2_FIELD].AsByte = waza.rankEffPer2;
                wazaField[RANKEFFPER3_FIELD].AsByte = waza.rankEffPer3;
                wazaField[FLAGS_FIELD].AsUInt = waza.flags;
                wazaField[CONTESTWAZANO_FIELD].AsUInt = waza.contestWazaNo;
            });

            mono[YUBIWOHURU_FIELD].SetArrayElementsAndInit(data.Yubiwohuru, (yubiwohuruField, yubiwohuru) =>
            {
                yubiwohuruField[WAZANOS_FIELD].SetArrayElementsAndInit(yubiwohuru.wazaNos, (f, v) => f.AsUShort = v);
            });

            personalMasterdatasBundle.SetMonoByPathID(pathId, mono);
        }
    }
}
