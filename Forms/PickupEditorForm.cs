using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ImpostersOrdeal
{
    public partial class PickupEditorForm : Form
    {
        GameDataSet gameData;

        List<PickupTable.PickupItem> pickupItems;
        List<string> items;

        public PickupEditorForm(GameDataSet gameData)
        {
            this.gameData = gameData;

            pickupItems = gameData.pickupTable.PickupItems;
            items = gameData.GetAllLabels(Constants.ITEM_MESSAGEFILE_NAME);
            InitializeComponent();

            ItemColumn.DataSource = items.ToArray();
            Lv10Column.ValueType = typeof(int);
            Lv20Column.ValueType = typeof(int);
            Lv30Column.ValueType = typeof(int);
            Lv40Column.ValueType = typeof(int);
            Lv50Column.ValueType = typeof(int);
            Lv60Column.ValueType = typeof(int);
            Lv70Column.ValueType = typeof(int);
            Lv80Column.ValueType = typeof(int);
            Lv90Column.ValueType = typeof(int);
            Lv100Column.ValueType = typeof(int);

            foreach (var p in pickupItems)
                dataGridView.Rows.Add(items[p.ID], (int)p.Ratios[0],
                    (int)p.Ratios[1], (int)p.Ratios[2], (int)p.Ratios[3],
                    (int)p.Ratios[4], (int)p.Ratios[5], (int)p.Ratios[6],
                    (int)p.Ratios[7], (int)p.Ratios[8], (int)p.Ratios[9]);

            ActivateControls();
        }

        private void CommitEdit(object sender, EventArgs e)
        {
            pickupItems = new();
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                var p = new PickupTable.PickupItem();
                p.Ratios = new();
                p.ID = (ushort)items.IndexOf((string)row.Cells[0].Value);
                for (int i = 0; i < 10; i++)
                    p.Ratios.Add((byte)(int)row.Cells[1 + i].Value);
                pickupItems.Add(p);
            }
            gameData.pickupTable.PickupItems = pickupItems;
        }

        private void ActivateControls()
        {
            dataGridView.CellEndEdit += CommitEdit;
        }

        private void DeactivateControls()
        {
            dataGridView.CellEndEdit -= CommitEdit;
        }

        private void DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MainForm.ShowDataError();
        }
    }
}
