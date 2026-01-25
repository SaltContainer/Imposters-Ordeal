using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ImpostersOrdeal
{
    public partial class TMEditorForm : Form
    {
        GameDataSet gameData;

        List<ItemTable.SheetWazaMachine> tms;
        List<ItemTable.SheetItem> items;
        ItemTable.SheetWazaMachine t;
        ItemTable.SheetItem i;

        public TMEditorForm(GameDataSet gameData)
        {
            this.gameData = gameData;

            tms = gameData.itemTable.WazaMachine;
            items = gameData.itemTable.Item;
            InitializeComponent();

            PopulateListBox();
            listBox.SelectedIndex = 0;
            t = tms[0];
            i = items[t.itemNo];

            itemComboBox.DataSource = items.Select(i => string.Format("{0}", gameData.GetLabelByIndex(Constants.ITEM_MESSAGEFILE_NAME, i.no))).ToArray();
            moveComboBox.DataSource = gameData.moveTable.Waza.Select(m => string.Format("{0}", gameData.GetLabelByIndex(Constants.MOVE_MESSAGEFILE_NAME, m.wazaNo))).ToArray();

            RefreshTMDisplay();
            ActivateControls();
        }

        private void TMChanged(object sender, EventArgs e)
        {
            DeactivateControls();

            t = tms[listBox.SelectedIndex];
            i = items[t.itemNo];
            RefreshTMDisplay();

            ActivateControls();
        }

        private void RefreshTMDisplay()
        {
            itemComboBox.SelectedIndex = t.itemNo;
            moveComboBox.SelectedIndex = t.wazaNo;
            compatibilityNumericUpDown.Value = i.group_id;
        }

        private void CommitEdit(object sender, EventArgs e)
        {
            DeactivateControls();

            if (t.itemNo != itemComboBox.SelectedIndex)
            {
                t.itemNo = itemComboBox.SelectedIndex == -1 ? 0 : itemComboBox.SelectedIndex;
                PopulateListBox();
                i = items[t.itemNo];
                RefreshTMDisplay();
            }

            if (t.wazaNo != moveComboBox.SelectedIndex)
            {
                t.wazaNo = moveComboBox.SelectedIndex == -1 ? 0 : moveComboBox.SelectedIndex;
                PopulateListBox();
            }

            i.group_id = (byte)compatibilityNumericUpDown.Value;

            ActivateControls();
        }

        private void ActivateControls()
        {
            listBox.SelectedIndexChanged += TMChanged;
            itemComboBox.SelectedIndexChanged += CommitEdit;
            moveComboBox.SelectedIndexChanged += CommitEdit;
            compatibilityNumericUpDown.ValueChanged += CommitEdit;
        }

        private void DeactivateControls()
        {
            listBox.SelectedIndexChanged -= TMChanged;
            itemComboBox.SelectedIndexChanged -= CommitEdit;
            moveComboBox.SelectedIndexChanged -= CommitEdit;
            compatibilityNumericUpDown.ValueChanged -= CommitEdit;
        }

        private void PopulateListBox()
        {
            int i = listBox.SelectedIndex;
            listBox.DataSource = tms.Select(t => string.Format("{0}", gameData.GetLabelByIndex(Constants.ITEM_MESSAGEFILE_NAME, t.itemNo))).ToArray();
            listBox.SelectedIndex = i;
        }
    }
}
