using ImpostersOrdeal.Utils;
using System.Collections.Generic;
using System;

namespace ImpostersOrdeal
{
    public class VanillaUgEncounterLevelParser : IParser<UgEncounterLevelTable>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (UgEncounterLevelTable)data);

        public IEnumerable<Type> GetRequiredDataSources() => [typeof(UGDataBundle)];

        // Monos
        private static readonly string UGENCOUNTLEVEL_MONONAME = "UgEncountLevel";

        // Arrays
        private static readonly string DATA_FIELD = "Data";

        // Fields
        private static readonly string MNAME_FIELD = "m_Name";
        private static readonly string MAXLV_FIELD = "MaxLv";
        private static readonly string MINLV_FIELD = "MinLv";

        public UgEncounterLevelTable ParseFromSources(FileManager fileManager)
        {
            var data = new UgEncounterLevelTable();

            var ugDataBundle = fileManager.GetUGDataBundle();
            var (pathId, monoBehaviour) = ugDataBundle.GetMonoByName(UGENCOUNTLEVEL_MONONAME);

            data.pathID = pathId;
            data.m_Name = monoBehaviour[MNAME_FIELD].AsString;

            data.Ranges = new();
            var rangeFields = monoBehaviour[DATA_FIELD].GetArrayElements();
            foreach (var rangeField in rangeFields)
            {
                var range = new UgEncounterLevelTable.SheetData();

                range.MinLv = rangeField[MINLV_FIELD].AsInt;
                range.MaxLv = rangeField[MAXLV_FIELD].AsInt;

                data.Ranges.Add(range);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, UgEncounterLevelTable data)
        {
            var ugDataBundle = fileManager.GetUGDataBundle();
            var (pathID, monoBehaviour) = ugDataBundle.GetMonoByName(UGENCOUNTLEVEL_MONONAME);

            monoBehaviour[DATA_FIELD].SetArrayElementsAndInit(data.Ranges, (rangeField, range) =>
            {
                rangeField[MINLV_FIELD].AsInt = range.MinLv;
                rangeField[MAXLV_FIELD].AsInt = range.MaxLv;
            });

            ugDataBundle.SetMonoByPathID(pathID, monoBehaviour);
        }
    }
}
