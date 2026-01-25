using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ImpostersOrdeal
{
    public partial class ShopEditorForm : Form
    {
        GameDataSet gameData;

        List<ShopTable.SheetFS> martItems;
        List<ShopTable.SheetFixedShop> fixedShopItems;
        List<ShopTable.SheetBPShop> bpShopItems;
        List<string> items;
        List<string> zones;

        public ShopEditorForm(GameDataSet gameData)
        {
            this.gameData = gameData;

            martItems = gameData.shopTable.FS;
            fixedShopItems = gameData.shopTable.FixedShop;
            bpShopItems = gameData.shopTable.BPShop;

            items = gameData.GetAllLabels(Constants.ITEM_MESSAGEFILE_NAME);
            zones = Zones.zoneNames.ToList();
            zones[zones.Count - 1] = "All";

            InitializeComponent();

            commonItemColumn.DataSource = items.ToArray();
            badgeCountColumn.ValueType = typeof(int);
            zoneIDColumn.DataSource = zones.ToArray();
            fixedItemColumn.DataSource = items.ToArray();
            shopColumn.ValueType = typeof(int);
            bpItemColumn.DataSource = items.ToArray();
            npcColumn.ValueType = typeof(int);

            foreach (var m in martItems)
                martDataGridView.Rows.Add(new object[] { items[m.ItemNo], m.BadgeNum, m.ZoneID == ZoneID.UNKNOWN ? zones.Last() : zones[(int)m.ZoneID] });

            foreach (var f in fixedShopItems)
                fixedShopDataGridView.Rows.Add(new object[] { items[f.ItemNo], f.ShopID });

            foreach (var b in bpShopItems)
                bpShopDataGridView.Rows.Add(new object[] { items[b.ItemNo], b.NPCID });

            ActivateControls();
        }

        private void CommitEdit(object sender, EventArgs e)
        {
            martItems = new();
            foreach (DataGridViewRow row in martDataGridView.Rows)
            {
                if (row.Cells[0].Value == null ||
                    row.Cells[1].Value == null ||
                    row.Cells[2].Value == null ||
                    items.IndexOf((string)row.Cells[0].Value) == 0)
                    continue;
                var m = new ShopTable.SheetFS();
                m.ItemNo = (ushort)items.IndexOf((string)row.Cells[0].Value);
                m.BadgeNum = (int)row.Cells[1].Value;
                m.ZoneID = (ZoneID)zones.IndexOf((string)row.Cells[2].Value);
                if ((int)m.ZoneID == zones.Count - 1)
                    m.ZoneID = ZoneID.UNKNOWN;
                martItems.Add(m);
            }
            gameData.shopTable.FS = martItems;

            fixedShopItems = new();
            foreach (DataGridViewRow row in fixedShopDataGridView.Rows)
            {
                if (row.Cells[0].Value == null ||
                    row.Cells[1].Value == null ||
                    items.IndexOf((string)row.Cells[0].Value) == 0)
                    continue;
                var f = new ShopTable.SheetFixedShop();
                f.ItemNo = (ushort)items.IndexOf((string)row.Cells[0].Value);
                f.ShopID = (int)row.Cells[1].Value;
                fixedShopItems.Add(f);
            }
            gameData.shopTable.FixedShop = fixedShopItems;

            bpShopItems = new();
            foreach (DataGridViewRow row in bpShopDataGridView.Rows)
            {
                if (row.Cells[0].Value == null ||
                    row.Cells[1].Value == null ||
                    items.IndexOf((string)row.Cells[0].Value) == 0)
                    continue;
                var b = new ShopTable.SheetBPShop();
                b.ItemNo = (ushort)items.IndexOf((string)row.Cells[0].Value);
                b.NPCID = (int)row.Cells[1].Value;
                bpShopItems.Add(b);
            }
            gameData.shopTable.BPShop = bpShopItems;
        }

        private void ActivateControls()
        {
            martDataGridView.CellEndEdit += CommitEdit;
            martDataGridView.UserDeletedRow += CommitEdit;
            fixedShopDataGridView.CellEndEdit += CommitEdit;
            fixedShopDataGridView.UserDeletedRow += CommitEdit;
            bpShopDataGridView.CellEndEdit += CommitEdit;
            bpShopDataGridView.UserDeletedRow += CommitEdit;
        }

        private void DeactivateControls()
        {
            martDataGridView.CellEndEdit -= CommitEdit;
            martDataGridView.UserDeletedRow -= CommitEdit;
            fixedShopDataGridView.CellEndEdit -= CommitEdit;
            fixedShopDataGridView.UserDeletedRow -= CommitEdit;
            bpShopDataGridView.CellEndEdit -= CommitEdit;
            bpShopDataGridView.UserDeletedRow -= CommitEdit;
        }

        private void MartDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.SetValues(new object[] { items[0], 0, zones.Last() });
        }

        private void FixedShopDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.SetValues(new object[] { items[0], 0 });
        }

        private void BpShopDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.SetValues(new object[] { items[0], 0 });
        }

        private void DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MainForm.ShowDataError();
        }
    }
}
