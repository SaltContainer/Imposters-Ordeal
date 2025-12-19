using System;
using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class ShopTable : ScriptableObject
    {
        public List<SheetFS> FS = new List<SheetFS>();
        public List<SheetFixedShop> FixedShop = new List<SheetFixedShop>();
        public List<SheetFlowerShop> FlowerShop = new List<SheetFlowerShop>();
        public List<SheetRibonShop> RibonShop = new List<SheetRibonShop>();
        public List<SheetSealShop> SealShop = new List<SheetSealShop>();
        public List<SheetBPShop> BPShop = new List<SheetBPShop>();
        public List<SheetOtenkiShop> OtenkiShop = new List<SheetOtenkiShop>();
        public List<SheetBoutiqueShop> BoutiqueShop = new List<SheetBoutiqueShop>();
        public List<SheetPalParkShop> PalParkShop = new List<SheetPalParkShop>();
        public List<SheetTobariDepartment4FShop> TobariDepartment4FShop = new List<SheetTobariDepartment4FShop>();

        public class SheetFS
        {
            public ushort ItemNo;
            public int BadgeNum;
            public ZoneID ZoneID;
        }

        public class SheetFixedShop
        {
            public ushort ItemNo;
            public int ShopID;
        }

        public class SheetFlowerShop
        {
            public int SealNo;
            public ushort ItemNo;
            public int Price;
        }

        public class SheetRibonShop
        {
            public int Price;
        }

        public class SheetSealShop
        {
            public int SealNo;
            public int Price;
            public DayOfWeek Week;
        }

        public class SheetBPShop
        {
            public ushort ItemNo;
            public int NPCID;
        }

        public class SheetOtenkiShop
        {
            public ushort ItemNo;
            public ushort RequestItem;
            public int Price;
        }

        public class SheetBoutiqueShop
        {
            public int DressNo;
            public int OpenDress; // EvWork.SYSFLAG_INDEX
            public int DressGet; // EvWork.FLAG_INDEX
            public int Price;
        }

        public class SheetPalParkShop
        {
            public ushort ItemNo;
            public ushort ItemNo2;
            public int Price;
            public int ShopID;
            public int ParkNameID;
            public int ParkNameNazo;
        }

        public class SheetTobariDepartment4FShop
        {
            public int UgItemID;
            public int price;
            public int ShopID;
        }
    }
}
