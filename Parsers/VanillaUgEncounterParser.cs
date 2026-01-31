using ImpostersOrdeal.Utils;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ImpostersOrdeal
{
    public class VanillaUgEncounterParser : IParser<UgEncounterTableCollection>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (UgEncounterTableCollection)data);

        public IEnumerable<Type> GetRequiredDataSources() => [typeof(UGDataBundle)];

        // Monos
        private static readonly string UGENCOUNT_MONOSCRIPTNAME = "UgEncount";

        // Arrays
        private static readonly string TABLE_FIELD = "table";

        // Fields
        private static readonly string MNAME_FIELD = "m_Name";
        private static readonly string MONSNO_FIELD = "monsno";
        private static readonly string VERSION_FIELD = "version";
        private static readonly string ZUKANFLAG_FIELD = "zukanflag";

        public UgEncounterTableCollection ParseFromSources(FileManager fileManager)
        {
            var data = new UgEncounterTableCollection();

            var ugDataBundle = fileManager.GetUGDataBundle();
            var monoBehaviours = ugDataBundle.GetMonosByScriptName(UGENCOUNT_MONOSCRIPTNAME);

            foreach (var (pathId, monoBehaviour) in monoBehaviours)
            {
                UgEncounterTable table = new();

                table.pathID = pathId;
                table.m_Name = monoBehaviour[MNAME_FIELD].AsString;

                table.mons = new();
                var monFields = monoBehaviour[TABLE_FIELD].GetArrayElements();
                foreach (var monField in monFields)
                {
                    var mon = new UgEncounterTable.Sheettable();

                    mon.monsno = monField[MONSNO_FIELD].AsInt;
                    mon.version = monField[VERSION_FIELD].AsInt;
                    mon.zukanflag = monField[ZUKANFLAG_FIELD].AsInt;

                    table.mons.Add(mon);
                }

                data.Add(table);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, UgEncounterTableCollection data)
        {
            var ugDataBundle = fileManager.GetUGDataBundle();
            var monoBehaviours = ugDataBundle.GetMonosByScriptName(UGENCOUNT_MONOSCRIPTNAME);

            foreach (var (pathID, mono) in monoBehaviours)
            {
                var table = data.First(t => t.pathID == pathID);

                mono[TABLE_FIELD].SetArrayElementsAndInit(table.mons, (monField, mon) =>
                {
                    monField[MONSNO_FIELD].AsInt = mon.monsno;
                    monField[VERSION_FIELD].AsInt = mon.version;
                    monField[ZUKANFLAG_FIELD].AsInt = mon.zukanflag;
                });

                ugDataBundle.SetMonoByPathID(pathID, mono);
            }
        }
    }
}
