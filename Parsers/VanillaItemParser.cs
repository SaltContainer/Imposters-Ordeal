using ImpostersOrdeal.Utils;
using System.Collections.Generic;
using System;

namespace ImpostersOrdeal
{
    public class VanillaItemParser : IParser<ItemTable>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (ItemTable)data);

        public IEnumerable<Type> GetRequiredDataSources() => [typeof(PersonalMasterdatasBundle)];

        // Monos
        private static readonly string ITEMTABLE_MONONAME = "ItemTable";

        // Arrays
        private static readonly string ITEM_FIELD = "Item";
        private static readonly string WAZAMACHINE_FIELD = "WazaMachine";

        // Fields
        private static readonly string ATC_FIELD = "atc";
        private static readonly string BATTLEFUNC_FIELD = "battle_func";
        private static readonly string BPPRICE_FIELD = "bp_price";
        private static readonly string EQP_FIELD = "eqp";
        private static readonly string FIELDFUNC_FIELD = "field_func";
        private static readonly string FLAGS0_FIELD = "flags0";
        private static readonly string FLDPOCKET_FIELD = "fld_pocket";
        private static readonly string GROUP_FIELD = "group";
        private static readonly string GROUPID_FIELD = "group_id";
        private static readonly string ICONID_FIELD = "iconid";
        private static readonly string ITEMNO_FIELD = "itemNo";
        private static readonly string MNAME_FIELD = "m_Name";
        private static readonly string MACHINENO_FIELD = "machineNo";
        private static readonly string NAGEATC_FIELD = "nage_atc";
        private static readonly string NO_FIELD = "no";
        private static readonly string PRICE_FIELD = "price";
        private static readonly string SIZENATC_FIELD = "sizen_atc";
        private static readonly string SIZENTYPE_FIELD = "sizen_type";
        private static readonly string SORT_FIELD = "sort";
        private static readonly string TUIBAMUEFF_FIELD = "tuibamu_eff";
        private static readonly string TYPE_FIELD = "type";
        private static readonly string WAZANO_FIELD = "wazaNo";
        private static readonly string WKAGIUP_FIELD = "wk_agi_up";
        private static readonly string WKATCUP_FIELD = "wk_atc_up";
        private static readonly string WKCMN_FIELD = "wk_cmn";
        private static readonly string WKCRITICALUP_FIELD = "wk_critical_up";
        private static readonly string WKDEFUP_FIELD = "wk_def_up";
        private static readonly string WKFRIEND1_FIELD = "wk_friend1";
        private static readonly string WKFRIEND2_FIELD = "wk_friend2";
        private static readonly string WKFRIEND3_FIELD = "wk_friend3";
        private static readonly string WKHITUP_FIELD = "wk_hit_up";
        private static readonly string WKPRMAGIEXP_FIELD = "wk_prm_agi_exp";
        private static readonly string WKPRMDEFEXP_FIELD = "wk_prm_def_exp";
        private static readonly string WKPRMHPEXP_FIELD = "wk_prm_hp_exp";
        private static readonly string WKPRMHPRCV_FIELD = "wk_prm_hp_rcv";
        private static readonly string WKPRMPOWEXP_FIELD = "wk_prm_pow_exp";
        private static readonly string WKPRMPPRCV_FIELD = "wk_prm_pp_rcv";
        private static readonly string WKPRMSPAEXP_FIELD = "wk_prm_spa_exp";
        private static readonly string WKPRMSPDEXP_FIELD = "wk_prm_spd_exp";
        private static readonly string WKSPAUP_FIELD = "wk_spa_up";
        private static readonly string WKSPDUP_FIELD = "wk_spd_up";

        public ItemTable ParseFromSources(FileManager fileManager)
        {
            var data = new ItemTable();

            var personalMasterdatasBundle = fileManager.GetPersonalMasterdatasBundle();
            var (pathId, mono) = personalMasterdatasBundle.GetMonoByName(ITEMTABLE_MONONAME);

            data.pathID = pathId;
            data.m_Name = mono[MNAME_FIELD].AsString;

            data.Item = new();
            var itemFields = mono[ITEM_FIELD].GetArrayElements();
            foreach (var itemField in itemFields)
            {
                var item = new ItemTable.SheetItem();

                item.no = itemField[NO_FIELD].AsShort;
                item.type = itemField[TYPE_FIELD].AsByte;
                item.iconid = itemField[ICONID_FIELD].AsInt;
                item.price = itemField[PRICE_FIELD].AsInt;
                item.bp_price = itemField[BPPRICE_FIELD].AsInt;
                item.eqp = itemField[EQP_FIELD].AsByte;
                item.atc = itemField[ATC_FIELD].AsByte;
                item.nage_atc = itemField[NAGEATC_FIELD].AsByte;
                item.sizen_atc = itemField[SIZENATC_FIELD].AsByte;
                item.sizen_type = itemField[SIZENTYPE_FIELD].AsByte;
                item.tuibamu_eff = itemField[TUIBAMUEFF_FIELD].AsByte;
                item.sort = itemField[SORT_FIELD].AsByte;
                item.group = itemField[GROUP_FIELD].AsByte;
                item.group_id = itemField[GROUPID_FIELD].AsByte;
                item.fld_pocket = itemField[FLDPOCKET_FIELD].AsByte;
                item.field_func = itemField[FIELDFUNC_FIELD].AsByte;
                item.battle_func = itemField[BATTLEFUNC_FIELD].AsByte;
                item.wk_cmn = itemField[WKCMN_FIELD].AsByte;
                item.wk_critical_up = itemField[WKCRITICALUP_FIELD].AsByte;
                item.wk_atc_up = itemField[WKATCUP_FIELD].AsByte;
                item.wk_def_up = itemField[WKDEFUP_FIELD].AsByte;
                item.wk_agi_up = itemField[WKAGIUP_FIELD].AsByte;
                item.wk_hit_up = itemField[WKHITUP_FIELD].AsByte;
                item.wk_spa_up = itemField[WKSPAUP_FIELD].AsByte;
                item.wk_spd_up = itemField[WKSPDUP_FIELD].AsByte;
                item.wk_prm_pp_rcv = itemField[WKPRMPPRCV_FIELD].AsByte;
                item.wk_prm_hp_exp = itemField[WKPRMHPEXP_FIELD].AsSByte;
                item.wk_prm_pow_exp = itemField[WKPRMPOWEXP_FIELD].AsSByte;
                item.wk_prm_def_exp = itemField[WKPRMDEFEXP_FIELD].AsSByte;
                item.wk_prm_agi_exp = itemField[WKPRMAGIEXP_FIELD].AsSByte;
                item.wk_prm_spa_exp = itemField[WKPRMSPAEXP_FIELD].AsSByte;
                item.wk_prm_spd_exp = itemField[WKPRMSPDEXP_FIELD].AsSByte;
                item.wk_friend1 = itemField[WKFRIEND1_FIELD].AsSByte;
                item.wk_friend2 = itemField[WKFRIEND2_FIELD].AsSByte;
                item.wk_friend3 = itemField[WKFRIEND3_FIELD].AsSByte;
                item.wk_prm_hp_rcv = itemField[WKPRMHPRCV_FIELD].AsByte;
                item.flags0 = itemField[FLAGS0_FIELD].AsUInt;

                data.Item.Add(item);
            }

            data.WazaMachine = new();
            var wazaMachineFields = mono[WAZAMACHINE_FIELD].GetArrayElements();
            foreach (var wazaMachineField in wazaMachineFields)
            {
                var wazaMachine = new ItemTable.SheetWazaMachine();

                wazaMachine.itemNo = wazaMachineField[ITEMNO_FIELD].AsInt;
                wazaMachine.machineNo = wazaMachineField[MACHINENO_FIELD].AsInt;
                wazaMachine.wazaNo = wazaMachineField[WAZANO_FIELD].AsInt;

                data.WazaMachine.Add(wazaMachine);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, ItemTable data)
        {
            var personalMasterdatasBundle = fileManager.GetPersonalMasterdatasBundle();
            var (pathId, mono) = personalMasterdatasBundle.GetMonoByName(ITEMTABLE_MONONAME);

            mono[ITEM_FIELD].SetArrayElementsAndInit(data.Item, (itemField, item) =>
            {
                itemField[NO_FIELD].AsShort = item.no;
                itemField[TYPE_FIELD].AsByte = item.type;
                itemField[ICONID_FIELD].AsInt = item.iconid;
                itemField[PRICE_FIELD].AsInt = item.price;
                itemField[BPPRICE_FIELD].AsInt = item.bp_price;
                itemField[EQP_FIELD].AsByte = item.eqp;
                itemField[ATC_FIELD].AsByte = item.atc;
                itemField[NAGEATC_FIELD].AsByte = item.nage_atc;
                itemField[SIZENATC_FIELD].AsByte = item.sizen_atc;
                itemField[SIZENTYPE_FIELD].AsByte = item.sizen_type;
                itemField[TUIBAMUEFF_FIELD].AsByte = item.tuibamu_eff;
                itemField[SORT_FIELD].AsByte = item.sort;
                itemField[GROUP_FIELD].AsByte = item.group;
                itemField[GROUPID_FIELD].AsByte = item.group_id;
                itemField[FLDPOCKET_FIELD].AsByte = item.fld_pocket;
                itemField[FIELDFUNC_FIELD].AsByte = item.field_func;
                itemField[BATTLEFUNC_FIELD].AsByte = item.battle_func;
                itemField[WKCMN_FIELD].AsByte = item.wk_cmn;
                itemField[WKCRITICALUP_FIELD].AsByte = item.wk_critical_up;
                itemField[WKATCUP_FIELD].AsByte = item.wk_atc_up;
                itemField[WKDEFUP_FIELD].AsByte = item.wk_def_up;
                itemField[WKAGIUP_FIELD].AsByte = item.wk_agi_up;
                itemField[WKHITUP_FIELD].AsByte = item.wk_hit_up;
                itemField[WKSPAUP_FIELD].AsByte = item.wk_spa_up;
                itemField[WKSPDUP_FIELD].AsByte = item.wk_spd_up;
                itemField[WKPRMPPRCV_FIELD].AsByte = item.wk_prm_pp_rcv;
                itemField[WKPRMHPEXP_FIELD].AsSByte = item.wk_prm_hp_exp;
                itemField[WKPRMPOWEXP_FIELD].AsSByte = item.wk_prm_pow_exp;
                itemField[WKPRMDEFEXP_FIELD].AsSByte = item.wk_prm_def_exp;
                itemField[WKPRMAGIEXP_FIELD].AsSByte = item.wk_prm_agi_exp;
                itemField[WKPRMSPAEXP_FIELD].AsSByte = item.wk_prm_spa_exp;
                itemField[WKPRMSPDEXP_FIELD].AsSByte = item.wk_prm_spd_exp;
                itemField[WKFRIEND1_FIELD].AsSByte = item.wk_friend1;
                itemField[WKFRIEND2_FIELD].AsSByte = item.wk_friend2;
                itemField[WKFRIEND3_FIELD].AsSByte = item.wk_friend3;
                itemField[WKPRMHPRCV_FIELD].AsByte = item.wk_prm_hp_rcv;
                itemField[FLAGS0_FIELD].AsUInt = item.flags0;
            });

            mono[WAZAMACHINE_FIELD].SetArrayElementsAndInit(data.WazaMachine, (wazaMachineField, wazaMachine) =>
            {
                wazaMachineField[ITEMNO_FIELD].AsInt = wazaMachine.itemNo;
                wazaMachineField[MACHINENO_FIELD].AsInt = wazaMachine.machineNo;
                wazaMachineField[WAZANO_FIELD].AsInt = wazaMachine.wazaNo;
            });

            personalMasterdatasBundle.SetMonoByPathID(pathId, mono);
        }
    }
}
