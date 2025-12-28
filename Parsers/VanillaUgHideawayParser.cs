using ImpostersOrdeal.Utils;
using System.Linq;

namespace ImpostersOrdeal
{
    public class VanillaUgHideawayParser : IParser<UgHideawayTable>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (UgHideawayTable)data);

        // Monos
        private static readonly string UGRANDMARK_MONONAME = "UgRandMark";
        private static readonly string UGSPECIALPOKEMON_MONONAME = "UgSpecialPokemon";

        // Arrays
        private static readonly string SHEET1_FIELD = "Sheet1";
        private static readonly string TABLE_FIELD = "table";
        private static readonly string TYPERATE_FIELD = "typerate";

        // Fields
        private static readonly string DSPECIALRATE_FIELD = "Dspecialrate";
        private static readonly string FILENAME_FIELD = "FileName";
        private static readonly string ID_FIELD = "id";
        private static readonly string LLMAX_FIELD = "llmax";
        private static readonly string LMAX_FIELD = "lmax";
        private static readonly string MNAME_FIELD = "m_Name";
        private static readonly string MAX_FIELD = "max";
        private static readonly string MIN_FIELD = "min";
        private static readonly string MMAX_FIELD = "mmax";
        private static readonly string MONSNO_FIELD = "monsno";
        private static readonly string PSPECIALRATE_FIELD = "Pspecialrate";
        private static readonly string SIZE_FIELD = "size";
        private static readonly string SMAX_FIELD = "smax";
        private static readonly string VERSION_FIELD = "version";
        private static readonly string WATERMAX_FIELD = "watermax";

        public UgHideawayTable ParseFromSources(FileManager fileManager)
        {
            var data = new UgHideawayTable();

            var ugDataBundle = fileManager.GetUGDataBundle();
            var (ugLandmarkPathID, ugLandmarkMono) = ugDataBundle.GetMonoByName(UGRANDMARK_MONONAME);
            var (ugSpecialPokemonPathID, ugSpecialPokemonMono) = ugDataBundle.GetMonoByName(UGSPECIALPOKEMON_MONONAME);

            data.hideawayData = new();
            data.hideawayData.pathID = ugLandmarkPathID;
            data.hideawayData.m_Name = ugLandmarkMono[MNAME_FIELD].AsString;

            data.hideawayData.hideaways = new();
            var hideawayFields = ugLandmarkMono[TABLE_FIELD].GetArrayElements();
            foreach (var hideawayField in hideawayFields)
            {
                var hideaway = new UgHideawayTable.UgHideawayData.Sheettable();

                hideaway.id = hideawayField[ID_FIELD].AsInt;
                hideaway.FileName = hideawayField[FILENAME_FIELD].AsString;
                hideaway.size = hideawayField[SIZE_FIELD].AsInt;
                hideaway.min = hideawayField[MIN_FIELD].AsInt;
                hideaway.max = hideawayField[MAX_FIELD].AsInt;
                hideaway.smax = hideawayField[SMAX_FIELD].AsInt;
                hideaway.mmax = hideawayField[MMAX_FIELD].AsInt;
                hideaway.lmax = hideawayField[LMAX_FIELD].AsInt;
                hideaway.llmax = hideawayField[LLMAX_FIELD].AsInt;
                hideaway.watermax = hideawayField[WATERMAX_FIELD].AsInt;
                hideaway.typerate = hideawayField[TYPERATE_FIELD].GetArrayElements().Select(f => f.AsInt).ToList();

                data.hideawayData.hideaways.Add(hideaway);
            }

            data.rareEncounterTable = new();
            data.hideawayData.pathID = ugSpecialPokemonPathID;
            data.hideawayData.m_Name = ugSpecialPokemonMono[MNAME_FIELD].AsString;

            data.rareEncounterTable.rareEncounters = new();
            var ugSpecialPokemonFields = ugSpecialPokemonMono[SHEET1_FIELD].GetArrayElements();
            foreach (var ugSpecialPokemonField in ugSpecialPokemonFields)
            {
                var rareEncounter = new UgHideawayTable.UgRareEncounterTable.UgRareEncounter();

                rareEncounter.id = ugSpecialPokemonField[ID_FIELD].AsInt;
                rareEncounter.monsno = ugSpecialPokemonField[MONSNO_FIELD].AsInt;
                rareEncounter.version = ugSpecialPokemonField[VERSION_FIELD].AsInt;
                rareEncounter.Dspecialrate = ugSpecialPokemonField[DSPECIALRATE_FIELD].AsInt;
                rareEncounter.Pspecialrate = ugSpecialPokemonField[PSPECIALRATE_FIELD].AsInt;

                data.rareEncounterTable.rareEncounters.Add(rareEncounter);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, UgHideawayTable data)
        {
            var ugDataBundle = fileManager.GetUGDataBundle();
            var (ugLandmarkPathID, ugLandmarkMono) = ugDataBundle.GetMonoByName(UGRANDMARK_MONONAME);
            var (ugSpecialPokemonPathID, ugSpecialPokemonMono) = ugDataBundle.GetMonoByName(UGSPECIALPOKEMON_MONONAME);

            ugLandmarkMono[TABLE_FIELD].SetArrayElementsAndInit(data.hideawayData.hideaways, (hideawayField, hideaway) =>
            {
                hideawayField[ID_FIELD].AsInt = hideaway.id;
                hideawayField[FILENAME_FIELD].AsString = hideaway.FileName;
                hideawayField[SIZE_FIELD].AsInt = hideaway.size;
                hideawayField[MIN_FIELD].AsInt = hideaway.min;
                hideawayField[MAX_FIELD].AsInt = hideaway.max;
                hideawayField[SMAX_FIELD].AsInt = hideaway.smax;
                hideawayField[MMAX_FIELD].AsInt = hideaway.mmax;
                hideawayField[LMAX_FIELD].AsInt = hideaway.lmax;
                hideawayField[LLMAX_FIELD].AsInt = hideaway.llmax;
                hideawayField[WATERMAX_FIELD].AsInt = hideaway.watermax;
                hideawayField[TYPERATE_FIELD].SetArrayElementsAndInit(hideaway.typerate, (f, v) => f.AsInt = v);
            });

            ugSpecialPokemonMono[TABLE_FIELD].SetArrayElementsAndInit(data.rareEncounterTable.rareEncounters, (rareEncounterField, rareEncounter) =>
            {
                rareEncounterField[ID_FIELD].AsInt = rareEncounter.id;
                rareEncounterField[MONSNO_FIELD].AsInt = rareEncounter.monsno;
                rareEncounterField[VERSION_FIELD].AsInt = rareEncounter.version;
                rareEncounterField[DSPECIALRATE_FIELD].AsInt = rareEncounter.Dspecialrate;
                rareEncounterField[PSPECIALRATE_FIELD].AsInt = rareEncounter.Pspecialrate;
            });

            ugDataBundle.SetMonoByPathID(ugLandmarkPathID, ugLandmarkMono);
            ugDataBundle.SetMonoByPathID(ugSpecialPokemonPathID, ugSpecialPokemonMono);
        }
    }
}
