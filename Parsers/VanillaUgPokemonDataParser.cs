using ImpostersOrdeal.Utils;
using System.Linq;

namespace ImpostersOrdeal
{
    public class VanillaUgPokemonDataParser : IParser<UgPokemonDataTable>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (UgPokemonDataTable)data);

        // Monos
        private static readonly string UGPOKEMONDATA_MONONAME = "UgPokemonData";

        // Arrays
        private static readonly string FLAGRATE_FIELD = "flagrate";
        private static readonly string MOVERATE_FIELD = "move_rate";
        private static readonly string REACTION_FIELD = "reaction";
        private static readonly string REACTIONCODE_FIELD = "reactioncode";
        private static readonly string SUBMOVERATE_FIELD = "submove_rate";
        private static readonly string TABLE_FIELD = "table";

        // Fields
        private static readonly string MNAME_FIELD = "m_Name";
        private static readonly string MONSNO_FIELD = "monsno";
        private static readonly string MOVETYPE_FIELD = "movetype";
        private static readonly string RATEUP_FIELD = "rateup";
        private static readonly string SIZE_FIELD = "size";
        private static readonly string TYPE1ID_FIELD = "type1ID";
        private static readonly string TYPE2ID_FIELD = "type2ID";

        public UgPokemonDataTable ParseFromSources(FileManager fileManager)
        {
            var data = new UgPokemonDataTable();

            var ugDataBundle = fileManager.GetUGDataBundle();
            var (pathId, monoBehaviour) = ugDataBundle.GetMonoByName(UGPOKEMONDATA_MONONAME);

            data.pathID = pathId;
            data.m_Name = monoBehaviour[MNAME_FIELD].AsString;

            data.table = new();
            var monFields = monoBehaviour[TABLE_FIELD].GetArrayElements();
            foreach (var monField in monFields)
            {
                var mon = new UgPokemonDataTable.Sheettable();

                mon.monsno = monField[MONSNO_FIELD].AsInt;
                mon.type1ID = monField[TYPE1ID_FIELD].AsInt;
                mon.type2ID = monField[TYPE2ID_FIELD].AsInt;
                mon.size = monField[SIZE_FIELD].AsInt;
                mon.movetype = monField[MOVETYPE_FIELD].AsInt;
                mon.reactioncode = monField[REACTIONCODE_FIELD].GetArrayElements().Select(f => f.AsInt).ToList();
                mon.move_rate = monField[MOVERATE_FIELD].GetArrayElements().Select(f => f.AsInt).ToList();
                mon.submove_rate = monField[SUBMOVERATE_FIELD].GetArrayElements().Select(f => f.AsInt).ToList();
                mon.reaction = monField[REACTION_FIELD].GetArrayElements().Select(f => f.AsInt).ToList();
                mon.flagrate = monField[FLAGRATE_FIELD].GetArrayElements().Select(f => f.AsInt).ToList();
                mon.rateup = monField[RATEUP_FIELD].AsInt;

                data.table.Add(mon);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, UgPokemonDataTable data)
        {
            var ugDataBundle = fileManager.GetUGDataBundle();
            var (pathID, monoBehaviour) = ugDataBundle.GetMonoByName(UGPOKEMONDATA_MONONAME);

            monoBehaviour[TABLE_FIELD].SetArrayElementsAndInit(data.table, (monField, mon) =>
            {
                monField[MONSNO_FIELD].AsInt = mon.monsno;
                monField[TYPE1ID_FIELD].AsInt = mon.type1ID;
                monField[TYPE2ID_FIELD].AsInt = mon.type2ID;
                monField[SIZE_FIELD].AsInt = mon.size;
                monField[MOVETYPE_FIELD].AsInt = mon.movetype;
                monField[REACTIONCODE_FIELD].SetArrayElementsAndInit(mon.reactioncode, (f, v) => f.AsInt = v);
                monField[MOVERATE_FIELD].SetArrayElementsAndInit(mon.move_rate, (f, v) => f.AsInt = v);
                monField[SUBMOVERATE_FIELD].SetArrayElementsAndInit(mon.submove_rate, (f, v) => f.AsInt = v);
                monField[REACTION_FIELD].SetArrayElementsAndInit(mon.reaction, (f, v) => f.AsInt = v);
                monField[FLAGRATE_FIELD].SetArrayElementsAndInit(mon.flagrate, (f, v) => f.AsInt = v);
                monField[RATEUP_FIELD].AsInt = mon.rateup;
            });

            ugDataBundle.SetMonoByPathID(pathID, monoBehaviour);
        }
    }
}
