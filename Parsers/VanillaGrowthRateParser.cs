using ImpostersOrdeal.Utils;
using System.Linq;

namespace ImpostersOrdeal
{
    public class VanillaGrowthRateParser : IParser<GrowTable>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (GrowTable)data);

        // Monos
        private static readonly string GROWTABLE_MONONAME = "GrowTable";

        // Arrays
        private static readonly string DATA_FIELD = "Data";
        private static readonly string EXPS_FIELD = "exps";

        // Fields
        private static readonly string MNAME_FIELD = "m_Name";

        public GrowTable ParseFromSources(FileManager fileManager)
        {
            var data = new GrowTable();

            var personalMasterdatasBundle = fileManager.GetPersonalMasterdatasBundle();
            var (pathID, monoBehaviour) = personalMasterdatasBundle.GetMonoByName(GROWTABLE_MONONAME);

            data.pathID = pathID;
            data.m_Name = monoBehaviour[MNAME_FIELD].AsString;

            data.Rates = new();
            var ratesFields = monoBehaviour[DATA_FIELD].GetArrayElements();
            for (int i = 0; i < ratesFields.Count; i++)
            {
                var ratesField = ratesFields[i];
                var rate = new GrowTable.SheetData();

                rate.id = i;
                rate.exps = ratesField[EXPS_FIELD].GetArrayElements().Select(f => f.AsUInt).ToList();

                data.Rates.Add(rate);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, GrowTable data)
        {
            var personalMasterdatasBundle = fileManager.GetPersonalMasterdatasBundle();
            var (pathID, monoBehaviour) = personalMasterdatasBundle.GetMonoByName(GROWTABLE_MONONAME);

            monoBehaviour[DATA_FIELD].SetArrayElementsAndInit(data.Rates, (ratesField, rate) =>
            {
                ratesField[EXPS_FIELD].SetArrayElementsAndInit(rate.exps, (f, v) => f.AsUInt = v);
            });

            personalMasterdatasBundle.SetMonoByPathID(pathID, monoBehaviour);
        }
    }
}
