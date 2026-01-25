using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ImpostersOrdeal
{
    public partial class MiscEncounterEditorForm : Form
    {
        GameDataSet gameData;

        FieldEncountTableCollection encounterTableFiles;
        List<string> dexEntries;
        FieldEncountTable f;

        private readonly string[] gameVersions = new string[]
        {
            "Diamond", "Pearl"
        };

        public MiscEncounterEditorForm(GameDataSet gameData)
        {
            this.gameData = gameData;

            encounterTableFiles = gameData.encounterTableFiles;

            dexEntries = gameData.GetAllLabels(Constants.POKEMONSPECIES_MESSAGEFILE_NAME);

            InitializeComponent();

            gameVersionComboBox.DataSource = gameVersions.ToList();
            gameVersionComboBox.SelectedIndex = 0;
            f = encounterTableFiles[0];

            trophyGardenColumn.DataSource = dexEntries.ToArray();
            rateColumn.ValueType = typeof(int);
            normalColumn.DataSource = dexEntries.ToArray();
            rareColumn.DataSource = dexEntries.ToArray();
            superRareColumn.DataSource = dexEntries.ToArray();
            safariColumn.DataSource = dexEntries.ToArray();

            RefreshFileDisplay();
            ActivateControls();
        }

        private void FileChanged(object sender, EventArgs e)
        {
            DeactivateControls();

            f = encounterTableFiles[gameVersionComboBox.SelectedIndex];
            RefreshFileDisplay();

            ActivateControls();
        }

        private void RefreshFileDisplay()
        {
            trophyGardenDataGridView.Rows.Clear();
            for (int i = 0; i < f.urayama.Count; i++)
                trophyGardenDataGridView.Rows.Add(new object[] { dexEntries[f.urayama[i].monsNo] });

            // TODO: Rework the honey tree stuff to actually use honey tree encounters
            honeyTreeDataGridView.Rows.Clear();
            for (int i = 0; i < f.mistu.Count; i++)
                honeyTreeDataGridView.Rows.Add(new object[] { f.mistu[i].Rate, dexEntries[f.mistu[i].Normal], dexEntries[f.mistu[i].Rare], dexEntries[f.mistu[i].SuperRare] });

            safariDataGridView.Rows.Clear();
            for (int i = 0; i < f.safari.Count; i++)
                safariDataGridView.Rows.Add(new object[] { dexEntries[f.safari[i].MonsNo] });
        }

        private void CommitEdit(object sender, EventArgs e)
        {
            for (int i = 0; i < f.urayama.Count; i++)
                f.urayama[i].monsNo = dexEntries.ToList().IndexOf((string)trophyGardenDataGridView.Rows[i].Cells[0].Value);

            for (int i = 0; i < f.mistu.Count; i++)
            {
                f.mistu[i].Rate = (int)honeyTreeDataGridView.Rows[i].Cells[0].Value;
                f.mistu[i].Normal = dexEntries.ToList().IndexOf((string)honeyTreeDataGridView.Rows[i].Cells[1].Value);
                f.mistu[i].Rare = dexEntries.ToList().IndexOf((string)honeyTreeDataGridView.Rows[i].Cells[2].Value);
                f.mistu[i].SuperRare = dexEntries.ToList().IndexOf((string)honeyTreeDataGridView.Rows[i].Cells[3].Value);
            }

            for (int i = 0; i < f.safari.Count; i++)
                f.safari[i].MonsNo = dexEntries.ToList().IndexOf((string)safariDataGridView.Rows[i].Cells[0].Value);
        }

        private void ActivateControls()
        {
            gameVersionComboBox.SelectedIndexChanged += FileChanged;
            trophyGardenDataGridView.CellEndEdit += CommitEdit;
            honeyTreeDataGridView.CellEndEdit += CommitEdit;
            safariDataGridView.CellEndEdit += CommitEdit;
        }

        private void DeactivateControls()
        {
            gameVersionComboBox.SelectedIndexChanged -= FileChanged;
            trophyGardenDataGridView.CellEndEdit -= CommitEdit;
            honeyTreeDataGridView.CellEndEdit -= CommitEdit;
            safariDataGridView.CellEndEdit -= CommitEdit;
        }

        private void DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MainForm.ShowDataError();
        }
    }
}
