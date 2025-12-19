using AssetsTools.NET;
using ImpostersOrdeal.Utils;
using System;
using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class VanillaShopParser : IParser<ShopTable>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (ShopTable)data);

        // Monos
        private static readonly string SHOPTABLE_MONONAME = "ShopTable";

        // Arrays
        private static readonly string BOUTIQUESHOP_FIELD = "BoutiqueShop";
        private static readonly string BPSHOP_FIELD = "BPShop";
        private static readonly string FIXEDSHOP_FIELD = "FixedShop";
        private static readonly string FLOWERSHOP_FIELD = "FlowerShop";
        private static readonly string FS_FIELD = "FS";
        private static readonly string OTENKISHOP_FIELD = "OtenkiShop";
        private static readonly string PALPARKSHOP_FIELD = "PalParkShop";
        private static readonly string RIBBONSHOP_FIELD = "RibonShop";
        private static readonly string SEALSHOP_FIELD = "SealShop";
        private static readonly string VEILSTONE4FSHOP_FIELD = "TobariDepartment4FShop";

        // Fields
        private static readonly string BADGENUM_FIELD = "BadgeNum";
        private static readonly string DRESSGET_FIELD = "DressGet";
        private static readonly string DRESSNO_FIELD = "DressNo";
        private static readonly string ITEMNO_FIELD = "ItemNo";
        private static readonly string ITEMNO2_FIELD = "ItemNo2";
        private static readonly string MNAME_FIELD = "m_Name";
        private static readonly string NPCID_FIELD = "NPCID";
        private static readonly string OPENDRESS_FIELD = "OpenDress";
        private static readonly string PARKNAMEID_FIELD = "ParkNameID";
        private static readonly string PARKNAMENAZO_FIELD = "ParkNameNazo";
        private static readonly string PRICE_FIELD = "Price";
        private static readonly string LOWERCASE_PRICE_FIELD = "price";
        private static readonly string REQUESTITEM_FIELD = "RequestItem";
        private static readonly string SEALNO_FIELD = "SealNo";
        private static readonly string SHOPID_FIELD = "ShopID";
        private static readonly string UGITEMID_FIELD = "UgItemID";
        private static readonly string WEEK_FIELD = "Week";
        private static readonly string ZONEID_FIELD = "ZoneID";

        // Exceptions
        private static readonly string BPSHOP_PARSER_ERROR = "Oh my, this dump might be a bit outdated...\n" +
                                                             "Please input at least the 1.1.3 version of BDSP.\n" +
                                                             "I don't feel so good...";
        private static readonly string OUTDATED_DUMP_MSG = "Outdated Dump";

        public ShopTable ParseFromSources(FileManager fileManager)
        {
            var data = new ShopTable();

            var dprMasterdatasBundle = fileManager.GetDprMasterdatasBundle();
            var (pathID, monoBehaviour) = dprMasterdatasBundle.GetMonoByName(SHOPTABLE_MONONAME);

            data.pathID = pathID;
            data.m_Name = monoBehaviour[MNAME_FIELD].AsString;

            data.FS = new();
            var fsFields = monoBehaviour[FS_FIELD].GetArrayElements();
            foreach (var fsField in fsFields)
            {
                var fs = new ShopTable.SheetFS();

                fs.ItemNo = fsField[ITEMNO_FIELD].AsUShort;
                fs.BadgeNum = fsField[BADGENUM_FIELD].AsInt;
                fs.ZoneID = (ZoneID)fsField[ZONEID_FIELD].AsInt;

                data.FS.Add(fs);
            }

            data.FixedShop = new();
            var fixedShopFields = monoBehaviour[FIXEDSHOP_FIELD].GetArrayElements();
            foreach (var fixedShopField in fixedShopFields)
            {
                var fixedShop = new ShopTable.SheetFixedShop();

                fixedShop.ItemNo = fixedShopField[ITEMNO_FIELD].AsUShort;
                fixedShop.ShopID = fixedShopField[SHOPID_FIELD].AsInt;

                data.FixedShop.Add(fixedShop);
            }

            data.FlowerShop = new();
            var flowerShopFields = monoBehaviour[FLOWERSHOP_FIELD].GetArrayElements();
            foreach (var flowerShopField in flowerShopFields)
            {
                var flowerShop = new ShopTable.SheetFlowerShop();

                flowerShop.SealNo = flowerShopField[SEALNO_FIELD].AsInt;
                flowerShop.ItemNo = flowerShopField[ITEMNO_FIELD].AsUShort;
                flowerShop.Price = flowerShopField[PRICE_FIELD].AsInt;

                data.FlowerShop.Add(flowerShop);
            }

            data.RibonShop = new();
            var ribbonShopFields = monoBehaviour[RIBBONSHOP_FIELD].GetArrayElements();
            foreach (var ribbonShopField in ribbonShopFields)
            {
                var ribbonShop = new ShopTable.SheetRibonShop();

                ribbonShop.Price = ribbonShopField[PRICE_FIELD].AsInt;

                data.RibonShop.Add(ribbonShop);
            }

            data.SealShop = new();
            var sealShopFields = monoBehaviour[SEALSHOP_FIELD].GetArrayElements();
            foreach (var sealShopField in sealShopFields)
            {
                var sealShop = new ShopTable.SheetSealShop();

                sealShop.SealNo = sealShopField[SEALNO_FIELD].AsInt;
                sealShop.Price = sealShopField[PRICE_FIELD].AsInt;
                sealShop.Week = (DayOfWeek)sealShopField[WEEK_FIELD].AsInt;

                data.SealShop.Add(sealShop);
            }

            data.BPShop = new();
            var bpShopFields = monoBehaviour[BPSHOP_FIELD].GetArrayElements();
            foreach (var bpShopField in bpShopFields)
            {
                var bpShop = new ShopTable.SheetBPShop();

                bpShop.ItemNo = bpShopField[ITEMNO_FIELD].AsUShort;

                // If the NPCID field is missing, this is an outdated dump
                if (bpShopField[NPCID_FIELD].IsDummy)
                {
                    MainForm.ShowParserError(BPSHOP_PARSER_ERROR);
                    throw new Exception(OUTDATED_DUMP_MSG);
                }

                bpShop.NPCID = bpShopField[NPCID_FIELD].AsInt;

                data.BPShop.Add(bpShop);
            }

            data.OtenkiShop = new();
            var otenkiShopFields = monoBehaviour[OTENKISHOP_FIELD].GetArrayElements();
            foreach (var otenkiShopField in otenkiShopFields)
            {
                var otenkiShop = new ShopTable.SheetOtenkiShop();

                otenkiShop.ItemNo = otenkiShopField[ITEMNO_FIELD].AsUShort;
                otenkiShop.RequestItem = otenkiShopField[REQUESTITEM_FIELD].AsUShort;
                otenkiShop.Price = otenkiShopField[PRICE_FIELD].AsInt;

                data.OtenkiShop.Add(otenkiShop);
            }

            data.BoutiqueShop = new();
            var boutiqueShopFields = monoBehaviour[BOUTIQUESHOP_FIELD].GetArrayElements();
            foreach (var boutiqueShopField in boutiqueShopFields)
            {
                var boutiqueShop = new ShopTable.SheetBoutiqueShop();

                boutiqueShop.DressNo = boutiqueShopField[DRESSNO_FIELD].AsInt;
                boutiqueShop.OpenDress = boutiqueShopField[OPENDRESS_FIELD].AsInt;
                boutiqueShop.DressGet = boutiqueShopField[DRESSGET_FIELD].AsInt;
                boutiqueShop.Price = boutiqueShopField[PRICE_FIELD].AsInt;

                data.BoutiqueShop.Add(boutiqueShop);
            }

            data.PalParkShop = new();
            var palParkShopFields = monoBehaviour[PALPARKSHOP_FIELD].GetArrayElements();
            foreach (var palParkShopField in palParkShopFields)
            {
                var palParkShop = new ShopTable.SheetPalParkShop();

                palParkShop.ItemNo = palParkShopField[ITEMNO_FIELD].AsUShort;
                palParkShop.ItemNo2 = palParkShopField[ITEMNO2_FIELD].AsUShort;
                palParkShop.Price = palParkShopField[PRICE_FIELD].AsInt;
                palParkShop.ShopID = palParkShopField[SHOPID_FIELD].AsInt;
                palParkShop.ParkNameID = palParkShopField[PARKNAMEID_FIELD].AsInt;
                palParkShop.ParkNameNazo = palParkShopField[PARKNAMENAZO_FIELD].AsInt;

                data.PalParkShop.Add(palParkShop);
            }

            data.TobariDepartment4FShop = new();
            var veilstoneShopFields = monoBehaviour[VEILSTONE4FSHOP_FIELD].GetArrayElements();
            foreach (var veilstoneShopField in veilstoneShopFields)
            {
                var veilstoneShop = new ShopTable.SheetTobariDepartment4FShop();

                veilstoneShop.UgItemID = veilstoneShopField[UGITEMID_FIELD].AsInt;
                veilstoneShop.price = veilstoneShopField[LOWERCASE_PRICE_FIELD].AsInt;
                veilstoneShop.ShopID = veilstoneShopField[SHOPID_FIELD].AsInt;

                data.TobariDepartment4FShop.Add(veilstoneShop);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, ShopTable data)
        {
            var dprMasterdatasBundle = fileManager.GetDprMasterdatasBundle();
            var (pathID, monoBehaviour) = dprMasterdatasBundle.GetMonoByName(SHOPTABLE_MONONAME);

            List<AssetTypeValueField> newFS = new();
            foreach (var fs in data.FS)
            {
                AssetTypeValueField fsField = monoBehaviour[FS_FIELD].CreateArrayElement();

                fsField[ITEMNO_FIELD].AsUShort = fs.ItemNo;
                fsField[BADGENUM_FIELD].AsInt = fs.BadgeNum;
                fsField[ZONEID_FIELD].AsInt = (int)fs.ZoneID;

                newFS.Add(fsField);
            }
            monoBehaviour[FS_FIELD].SetArrayElements(newFS);

            List<AssetTypeValueField> newFixedShops = new();
            foreach (var fixedShop in data.FixedShop)
            {
                AssetTypeValueField fixedShopField = monoBehaviour[FIXEDSHOP_FIELD].CreateArrayElement();

                fixedShopField[ITEMNO_FIELD].AsUShort = fixedShop.ItemNo;
                fixedShopField[SHOPID_FIELD].AsInt = fixedShop.ShopID;

                newFixedShops.Add(fixedShopField);
            }
            monoBehaviour[FIXEDSHOP_FIELD].SetArrayElements(newFixedShops);

            List<AssetTypeValueField> newFlowerShops = new();
            foreach (var flowerShop in data.FlowerShop)
            {
                AssetTypeValueField flowerShopField = monoBehaviour[FLOWERSHOP_FIELD].CreateArrayElement();

                flowerShopField[SEALNO_FIELD].AsInt = flowerShop.SealNo;
                flowerShopField[ITEMNO_FIELD].AsUShort = flowerShop.ItemNo;
                flowerShopField[PRICE_FIELD].AsInt = flowerShop.Price;

                newFlowerShops.Add(flowerShopField);
            }
            monoBehaviour[FLOWERSHOP_FIELD].SetArrayElements(newFlowerShops);

            List<AssetTypeValueField> newRibbonShops = new();
            foreach (var ribbonShop in data.RibonShop)
            {
                AssetTypeValueField ribbonShopField = monoBehaviour[RIBBONSHOP_FIELD].CreateArrayElement();

                ribbonShopField[PRICE_FIELD].AsInt = ribbonShop.Price;

                newRibbonShops.Add(ribbonShopField);
            }
            monoBehaviour[RIBBONSHOP_FIELD].SetArrayElements(newRibbonShops);

            List<AssetTypeValueField> newSealShops = new();
            foreach (var sealShop in data.SealShop)
            {
                AssetTypeValueField sealShopField = monoBehaviour[SEALSHOP_FIELD].CreateArrayElement();

                sealShopField[SEALNO_FIELD].AsInt = sealShop.SealNo;
                sealShopField[PRICE_FIELD].AsInt = sealShop.Price;
                sealShopField[WEEK_FIELD].AsInt = (int)sealShop.Week;

                newSealShops.Add(sealShopField);
            }
            monoBehaviour[SEALSHOP_FIELD].SetArrayElements(newSealShops);

            List<AssetTypeValueField> newBpShops = new();
            foreach (var bpShop in data.BPShop)
            {
                AssetTypeValueField bpShopField = monoBehaviour[BPSHOP_FIELD].CreateArrayElement();

                bpShopField[ITEMNO_FIELD].AsUShort = bpShop.ItemNo;
                bpShopField[NPCID_FIELD].AsInt = bpShop.NPCID;

                newBpShops.Add(bpShopField);
            }
            monoBehaviour[BPSHOP_FIELD].SetArrayElements(newBpShops);

            List<AssetTypeValueField> newOtenkiShops = new();
            foreach (var otenkiShop in data.OtenkiShop)
            {
                AssetTypeValueField otenkiShopField = monoBehaviour[OTENKISHOP_FIELD].CreateArrayElement();

                otenkiShopField[ITEMNO_FIELD].AsUShort = otenkiShop.ItemNo;
                otenkiShopField[REQUESTITEM_FIELD].AsUShort = otenkiShop.RequestItem;
                otenkiShopField[PRICE_FIELD].AsInt = otenkiShop.Price;

                newOtenkiShops.Add(otenkiShopField);
            }
            monoBehaviour[OTENKISHOP_FIELD].SetArrayElements(newOtenkiShops);

            List<AssetTypeValueField> newBoutiqueShops = new();
            foreach (var boutiqueShop in data.BoutiqueShop)
            {
                AssetTypeValueField boutiqueShopField = monoBehaviour[BOUTIQUESHOP_FIELD].CreateArrayElement();

                boutiqueShopField[DRESSNO_FIELD].AsInt = boutiqueShop.DressNo;
                boutiqueShopField[OPENDRESS_FIELD].AsInt = boutiqueShop.OpenDress;
                boutiqueShopField[DRESSGET_FIELD].AsInt = boutiqueShop.DressGet;
                boutiqueShopField[PRICE_FIELD].AsInt = boutiqueShop.Price;

                newBoutiqueShops.Add(boutiqueShopField);
            }
            monoBehaviour[BOUTIQUESHOP_FIELD].SetArrayElements(newBoutiqueShops);

            List<AssetTypeValueField> newPalParkShops = new();
            foreach (var palparkShop in data.PalParkShop)
            {
                AssetTypeValueField palParkShopField = monoBehaviour[PALPARKSHOP_FIELD].CreateArrayElement();

                palParkShopField[ITEMNO_FIELD].AsUShort = palparkShop.ItemNo;
                palParkShopField[ITEMNO2_FIELD].AsUShort = palparkShop.ItemNo2;
                palParkShopField[PRICE_FIELD].AsInt = palparkShop.Price;
                palParkShopField[SHOPID_FIELD].AsInt = palparkShop.ShopID;
                palParkShopField[PARKNAMEID_FIELD].AsInt = palparkShop.ParkNameID;
                palParkShopField[PARKNAMENAZO_FIELD].AsInt = palparkShop.ParkNameNazo;

                newPalParkShops.Add(palParkShopField);
            }
            monoBehaviour[PALPARKSHOP_FIELD].SetArrayElements(newPalParkShops);

            List<AssetTypeValueField> newVeilstoneShops = new();
            foreach (var veilstoneShop in data.TobariDepartment4FShop)
            {
                AssetTypeValueField veilstoneShopField = monoBehaviour[VEILSTONE4FSHOP_FIELD].CreateArrayElement();

                veilstoneShopField[UGITEMID_FIELD].AsInt = veilstoneShop.UgItemID;
                veilstoneShopField[LOWERCASE_PRICE_FIELD].AsInt = veilstoneShop.price;
                veilstoneShopField[SHOPID_FIELD].AsInt = veilstoneShop.ShopID;

                newVeilstoneShops.Add(veilstoneShopField);
            }
            monoBehaviour[VEILSTONE4FSHOP_FIELD].SetArrayElements(newVeilstoneShops);

            dprMasterdatasBundle.SetMonoByPathID(pathID, monoBehaviour);
        }
    }
}
