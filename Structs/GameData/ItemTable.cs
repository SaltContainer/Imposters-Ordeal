using ImpostersOrdeal.Utils;
using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class ItemTable : ScriptableObject
    {
        public List<SheetItem> Item = new List<SheetItem>();
        public List<SheetWazaMachine> WazaMachine = new List<SheetWazaMachine>();

        public class SheetItem
        {
            public short no;
            public byte type;
            public int iconid;
            public int price;
            public int bp_price;
            public byte eqp;
            public byte atc;
            public byte nage_atc;
            public byte sizen_atc;
            public byte sizen_type;
            public byte tuibamu_eff;
            public byte sort;
            public byte group;
            public byte group_id;
            public byte fld_pocket;
            public byte field_func;
            public byte battle_func;
            public byte wk_cmn;
            public byte wk_critical_up;
            public byte wk_atc_up;
            public byte wk_def_up;
            public byte wk_agi_up;
            public byte wk_hit_up;
            public byte wk_spa_up;
            public byte wk_spd_up;
            public byte wk_prm_pp_rcv;
            public sbyte wk_prm_hp_exp;
            public sbyte wk_prm_pow_exp;
            public sbyte wk_prm_def_exp;
            public sbyte wk_prm_agi_exp;
            public sbyte wk_prm_spa_exp;
            public sbyte wk_prm_spd_exp;
            public sbyte wk_friend1;
            public sbyte wk_friend2;
            public sbyte wk_friend3;
            public byte wk_prm_hp_rcv;
            public uint flags0;

            public bool[] Flags { get => flags0.GetBitArray(); set => flags0 = value.ConvertBitArrayToUint(); }

            public bool IsEnabled()
            {
                return !Flags[31];
            }
        }

        public class SheetWazaMachine
        {
            public int itemNo;
            public int machineNo;
            public int wazaNo;
        }

        public bool IsTMValid(int index)
        {
            if (index < 0 || index >= WazaMachine.Count)
                return false;

            return IsTMValid(WazaMachine[index]);
        }

        public bool IsTMValid(SheetWazaMachine tm)
        {
            return Item[tm.itemNo].IsEnabled() &&
                Item[tm.itemNo].field_func == 2 &&
                Item[tm.itemNo].group_id > 0;
        }
    }
}
