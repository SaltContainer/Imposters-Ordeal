using AssetsTools.NET;
using System.Collections.Generic;
using ImpostersOrdeal.Utils;
using System.Linq;

namespace ImpostersOrdeal
{
    public class VanillaPickupParser : IParser<PickupTable>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (PickupTable)data);

        // Monos
        private static readonly string PICKUPTABLE_MONONAME = "MonohiroiTable";

        // Arrays
        private static readonly string MONOHIROI_FIELD = "MonoHiroi";

        // Fields
        private static readonly string ID_FIELD = "ID";
        private static readonly string MNAME_FIELD = "m_Name";
        private static readonly string RATIOS_FIELD = "Ratios";

        public PickupTable ParseFromSources(FileManager fileManager)
        {
            var data = new PickupTable();

            var dprMasterdatasBundle = fileManager.GetDprMasterdatasBundle();
            var (pathID, monoBehaviour) = dprMasterdatasBundle.GetMonoByName(PICKUPTABLE_MONONAME);

            data.pathID = pathID;
            data.m_Name = monoBehaviour[MNAME_FIELD].AsString;

            data.PickupItems = new();
            var pickupItemFields = monoBehaviour[MONOHIROI_FIELD].GetArrayElements();
            foreach (var pickupItemField in pickupItemFields)
            {
                var pickupItem = new PickupTable.PickupItem();

                pickupItem.ID = pickupItemField[ID_FIELD].AsUShort;
                pickupItem.Ratios = pickupItemField[RATIOS_FIELD].GetArrayElements().Select(r => r.AsByte).ToList();

                data.PickupItems.Add(pickupItem);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, PickupTable data)
        {
            var dprMasterdatasBundle = fileManager.GetDprMasterdatasBundle();
            var (pathID, monoBehaviour) = dprMasterdatasBundle.GetMonoByName(PICKUPTABLE_MONONAME);

            List<AssetTypeValueField> newPickupItems = new();
            foreach (var pickupItem in data.PickupItems)
            {
                AssetTypeValueField pickupItemField = monoBehaviour[MONOHIROI_FIELD].CreateArrayElement();

                pickupItemField[ID_FIELD].AsUShort = pickupItem.ID;
                pickupItemField[RATIOS_FIELD].SetByteArrayElements(pickupItem.Ratios);

                newPickupItems.Add(pickupItemField);
            }
            monoBehaviour[MONOHIROI_FIELD].SetArrayElements(newPickupItems);

            dprMasterdatasBundle.SetMonoByPathID(pathID, monoBehaviour);
        }
    }
}
