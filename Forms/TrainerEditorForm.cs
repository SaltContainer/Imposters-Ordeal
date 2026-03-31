using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ImpostersOrdeal
{
    public partial class TrainerEditorForm : Form
    {
        private GameDataSet gameData;

        public List<TrainerTable.SheetTrainerData> trainers;
        private Dictionary<int, string> trainerTypeLabels;
        public Dictionary<int, string> trainerTypeNames;
        private Dictionary<int, int> trainerTypeToCC;
        private Dictionary<string, string> labelToTrainerName;
        private Dictionary<TrainerTable.SheetTrainerData, int> trainerToID;
        public List<string> items;
        public TrainerTable.SheetTrainerData t;
        private TrainerPokemonEditorForm tpef;
        private TrainerTable.SheetTrainerData trainerClipboard;
        private List<TrainerTable.SheetTrainerData.TrainerPoke> tpClipboard;

        private readonly string[] sortNames = new string[]
        {
            "Sort by ID",
            "Sort by name",
            "Sort by level"
        };
        private readonly Comparison<TrainerTable.SheetTrainerData>[] sortComparisons;

        public string FullTrainerName => string.Format("{0} {1}", trainerTypeNames[t.TypeID], gameData.GetLabelByName(Constants.TRAINERNAME_MESSAGEFILE_NAME, t.NameLabel));

        public TrainerEditorForm(GameDataSet gameData)
        {
            this.gameData = gameData;

            trainerTypeLabels = new();
            trainerTypeNames = new();
            trainerTypeToCC = new();
            trainerTypeLabels.Add(-1, "None");
            trainerTypeNames.Add(-1, "None");
            trainerTypeToCC.Add(-1, 0);

            for (int i = 0; i < gameData.trainerTable.TrainerType.Count; i++)
            {
                var tt = gameData.trainerTable.TrainerType[i];

                // Ignore type IDs of -1, since they can't be used anyways
                if (tt.TypeID == -1)
                    continue;

                trainerTypeLabels.Add(tt.TypeID, tt.LabelTrType);
                trainerTypeNames.Add(tt.TypeID, gameData.GetLabelByName(Constants.TRAINERTYPE_MESSAGEFILE_NAME, tt.LabelTrType));
                trainerTypeToCC.Add(tt.TypeID, trainerTypeToCC.Count);
            }
            labelToTrainerName = gameData.GetAllLabelsDictionary(Constants.TRAINERNAME_MESSAGEFILE_NAME);
            items = gameData.GetAllLabels(Constants.ITEM_MESSAGEFILE_NAME);

            InitializeComponent();
            tpef = new(this, gameData);

            trainers = new();
            trainers.AddRange(gameData.trainerTable.TrainerData);
            trainerToID = new Dictionary<TrainerTable.SheetTrainerData, int>();
            for (int i=0; i<trainers.Count; i++)
                trainerToID[trainers[i]] = i;

            sortComparisons =
            [
                (t1, t2) => trainerToID[t1].CompareTo(trainerToID[t2]),
                (t1, t2) => labelToTrainerName[t1.NameLabel].CompareTo(labelToTrainerName[t2.NameLabel]),
                (t1, t2) => t1.AverageLevel.CompareTo(t2.AverageLevel)
            ];

            sortByComboBox.DataSource = sortNames;
            sortByComboBox.SelectedIndex = 0;
            trainers.Sort(sortComparisons[sortByComboBox.SelectedIndex]);

            PopulateListBox();
            t = trainers[0];

            trainerTypeComboBox.DataSource = trainerTypeLabels.Values.ToArray();
            trainerNameComboBox.DataSource = labelToTrainerName.Values.ToArray();

            item1ComboBox.DataSource = items.ToArray();
            item2ComboBox.DataSource = items.ToArray();
            item3ComboBox.DataSource = items.ToArray();
            item4ComboBox.DataSource = items.ToArray();

            RefreshTrainerDisplay();
            ActivateControls();
        }

        private void TrainerChanged(object sender, EventArgs e)
        {
            DeactivateControls();

            t = trainers[listBox.SelectedIndex];
            RefreshTrainerDisplay();

            ActivateControls();
        }

        private void SortChanged(object sender, EventArgs e)
        {
            DeactivateControls();

            trainers.Sort(sortComparisons[sortByComboBox.SelectedIndex]);
            PopulateListBox();
            listBox.SelectedIndex = trainers.IndexOf(t);

            ActivateControls();
        }

        private void RefreshTrainerDisplay()
        {
            RefreshTextBoxDisplay();

            trainerTypeComboBox.SelectedIndex = trainerTypeToCC[t.TypeID];
            trainerNameComboBox.SelectedItem = labelToTrainerName[t.NameLabel];
            arenaIDNumericUpDown.Value = t.ArenaID;
            effectIDNumericUpDown.Value = t.EffectID;

            doubleBattleCheckBox.Checked = t.FightType == 1;
            prizeMoneyNumericUpDown.Value = t.Gold;

            if (t.UseItem.Count >= 1) item1ComboBox.SelectedIndex = t.UseItem[0];
            else item1ComboBox.SelectedIndex = 0;

            if (t.UseItem.Count >= 2) item2ComboBox.SelectedIndex = t.UseItem[1];
            else item2ComboBox.SelectedIndex = 0;

            if (t.UseItem.Count >= 3) item3ComboBox.SelectedIndex = t.UseItem[2];
            else item3ComboBox.SelectedIndex = 0;

            if (t.UseItem.Count >= 4) item4ComboBox.SelectedIndex = t.UseItem[3];
            else item4ComboBox.SelectedIndex = 0;

            bool[] aiFlags = t.AIFlags;
            checkBox1.Checked = aiFlags[0];
            checkBox2.Checked = aiFlags[1];
            checkBox3.Checked = aiFlags[2];
            checkBox4.Checked = aiFlags[3];
            checkBox5.Checked = aiFlags[4];
            checkBox6.Checked = aiFlags[5];
            checkBox7.Checked = aiFlags[6];

            PopulatePartyDataGridView();
        }

        private void CommitEdit(object sender, EventArgs e)
        {
            t.TypeID = trainerTypeNames.Keys.ToArray()[trainerTypeComboBox.SelectedIndex];
            t.ArenaID = (int)arenaIDNumericUpDown.Value;
            t.EffectID = (int)effectIDNumericUpDown.Value;

            t.FightType = (byte)(doubleBattleCheckBox.Checked ? 1 : 0);
            t.Gold = (byte)prizeMoneyNumericUpDown.Value;
            t.UseItem[0] = (ushort)(item1ComboBox.SelectedIndex == -1 ? 0 : item1ComboBox.SelectedIndex);
            t.UseItem[1] = (ushort)(item2ComboBox.SelectedIndex == -1 ? 0 : item2ComboBox.SelectedIndex);
            t.UseItem[2] = (ushort)(item3ComboBox.SelectedIndex == -1 ? 0 : item3ComboBox.SelectedIndex);
            t.UseItem[3] = (ushort)(item4ComboBox.SelectedIndex == -1 ? 0 : item4ComboBox.SelectedIndex);

            bool[] aiFlags = new bool[32];
            aiFlags[0] = checkBox1.Checked;
            aiFlags[1] = checkBox2.Checked;
            aiFlags[2] = checkBox3.Checked;
            aiFlags[3] = checkBox4.Checked;
            aiFlags[4] = checkBox5.Checked;
            aiFlags[5] = checkBox6.Checked;
            aiFlags[6] = checkBox7.Checked;
            t.AIFlags = aiFlags;

            RefreshTextBoxDisplay();
        }

        private void CommitNameEdit(object sender, EventArgs e)
        {
            DeactivateControls();

            t.NameLabel = labelToTrainerName.Keys.ToArray()[trainerNameComboBox.SelectedIndex];
            PopulateListBox();
            RefreshTextBoxDisplay();

            ActivateControls();
        }

        private void ActivateControls()
        {
            sortByComboBox.SelectedIndexChanged += SortChanged;
            listBox.SelectedIndexChanged += TrainerChanged;

            trainerTypeComboBox.SelectedIndexChanged += CommitEdit;
            trainerNameComboBox.SelectedIndexChanged += CommitNameEdit;
            arenaIDNumericUpDown.ValueChanged += CommitEdit;
            effectIDNumericUpDown.ValueChanged += CommitEdit;

            doubleBattleCheckBox.CheckedChanged += CommitEdit;
            prizeMoneyNumericUpDown.ValueChanged += CommitEdit;
            item1ComboBox.SelectedIndexChanged += CommitEdit;
            item2ComboBox.SelectedIndexChanged += CommitEdit;
            item3ComboBox.SelectedIndexChanged += CommitEdit;
            item4ComboBox.SelectedIndexChanged += CommitEdit;

            checkBox1.CheckedChanged += CommitEdit;
            checkBox2.CheckedChanged += CommitEdit;
            checkBox3.CheckedChanged += CommitEdit;
            checkBox4.CheckedChanged += CommitEdit;
            checkBox5.CheckedChanged += CommitEdit;
            checkBox6.CheckedChanged += CommitEdit;
            checkBox7.CheckedChanged += CommitEdit;

            partyDataGridView.CellContentClick += ConfigureTP;
        }

        private void DeactivateControls()
        {
            sortByComboBox.SelectedIndexChanged -= SortChanged;
            listBox.SelectedIndexChanged -= TrainerChanged;

            trainerTypeComboBox.SelectedIndexChanged -= CommitEdit;
            trainerNameComboBox.SelectedIndexChanged -= CommitNameEdit;
            arenaIDNumericUpDown.ValueChanged -= CommitEdit;
            effectIDNumericUpDown.ValueChanged -= CommitEdit;

            doubleBattleCheckBox.CheckedChanged -= CommitEdit;
            prizeMoneyNumericUpDown.ValueChanged -= CommitEdit;
            item1ComboBox.SelectedIndexChanged -= CommitEdit;
            item2ComboBox.SelectedIndexChanged -= CommitEdit;
            item3ComboBox.SelectedIndexChanged -= CommitEdit;
            item4ComboBox.SelectedIndexChanged -= CommitEdit;

            checkBox1.CheckedChanged -= CommitEdit;
            checkBox2.CheckedChanged -= CommitEdit;
            checkBox3.CheckedChanged -= CommitEdit;
            checkBox4.CheckedChanged -= CommitEdit;
            checkBox5.CheckedChanged -= CommitEdit;
            checkBox6.CheckedChanged -= CommitEdit;
            checkBox7.CheckedChanged -= CommitEdit;

            partyDataGridView.CellContentClick -= ConfigureTP;
        }

        private void RefreshTextBoxDisplay()
        {
            trainerDisplayTextBox.Text = string.Format("{0} - {1} {2}", t.TypeID, trainerTypeNames[t.TypeID], labelToTrainerName[t.NameLabel]);
        }

        private void PopulatePartyDataGridView()
        {
            partyDataGridView.Rows.Clear();
            foreach (var tp in t.Pokes)
            {
                var speciesName = gameData.GetLabelByIndex(Constants.POKEMONSPECIES_MESSAGEFILE_NAME, tp.MonsNo);
                var formName = gameData.GetFormName(tp.MonsNo, tp.FormNo).Replace(speciesName, string.Empty).Trim();

                string name;
                if (string.IsNullOrEmpty(formName))
                    name = string.Format("Lv. {0} {1}", tp.Level, speciesName);
                else
                    name = string.Format("Lv. {0} {1} ({2})", tp.Level, speciesName, formName);

                partyDataGridView.Rows.Add(new object[] { name, "Configure" });
            }
        }

        private void ConfigureTP(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                if (e.RowIndex == t.Pokes.Count)
                {
                    // TODO: poke stuff
                    //t.Pokes.Add(t.Pokes.Count > 0 ? new(t.Pokes.Last()) : new());
                    t.Pokes.Add(new());
                }
                else
                {
                    tpef.SetTP(t, e.RowIndex);
                    tpef.ShowDialog();
                }

                PopulatePartyDataGridView();
            }
        }

        private void UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            t.Pokes.RemoveAt(e.Row.Index);
        }

        private void CopyTPButtonClick(object sender, EventArgs e)
        {
            // TODO: poke stuff
            tpClipboard = new();
            foreach (DataGridViewRow row in partyDataGridView.SelectedRows)
                if (row.Index >= 0 && row.Index < t.Pokes.Count)
                {
                    //tpClipboard.Add(new(t.trainerPokemon[row.Index]));
                    tpClipboard.Add(new());
                }
        }

        private void PasteTPButtonClick(object sender, EventArgs e)
        {
            if (tpClipboard == null)
                return;

            List<TrainerTable.SheetTrainerData.TrainerPoke> newParty = new();
            List<DataGridViewRow> selection = new();
            foreach (DataGridViewRow row in partyDataGridView.SelectedRows)
                selection.Add(row);

            int firstIndex = selection.Count > 0 ? selection.Select(r => r.Index).Min() : t.Pokes.Count;
            int lastIndex = selection.Count > 0 ? selection.Select(r => r.Index).Max() : t.Pokes.Count;

            // TODO: poke stuff
            for (int i = 0; i < firstIndex; i++)
            {
                //newParty.Add(new(t.Pokes[i]));
                newParty.Add(new());
            }
            
            foreach (var tp in tpClipboard)
            {
                //newParty.Add(new(tp));
                newParty.Add(new());
            }
            
            for (int i = lastIndex + 1; i < t.Pokes.Count; i++)
            {
                //newParty.Add(new(t.Pokes[i]));
                newParty.Add(new());
            }

            t.Pokes = newParty;

            PopulatePartyDataGridView();
        }

        private void ShowdownButtonClick(object sender, EventArgs e)
        {
            var tsef = new TrainerShowdownEditorForm(t, gameData, FullTrainerName);
            if (tsef.ShowDialog() == DialogResult.OK)
                t.Pokes = tsef.TrainerPokesResult;

            PopulatePartyDataGridView();
        }

        private void CopyTrainerButtonClick(object sender, EventArgs e)
        {
            // TODO: poke stuff
            //trainerClipboard = new(t);
            trainerClipboard = new();
        }

        private void PasteTrainerButtonClick(object sender, EventArgs e)
        {
            if (trainerClipboard != null)
            {
                DeactivateControls();
                // TODO: trainer stuff
                /*int id = trainers[listBox.SelectedIndex].TypeID;
                trainers[listBox.SelectedIndex].SetAll(trainerClipboard);
                trainers[listBox.SelectedIndex].trainerID = id;*/
                PopulateListBox();
                ActivateControls();

                TrainerChanged(null, null);
            }
        }

        private void PopulateListBox()
        {
            int index = listBox.SelectedIndex;
            if (index < 0)
                index = 0;

            listBox.DataSource = trainers.Select(o => string.Format("{0} - {1}", trainerToID[o], labelToTrainerName[o.NameLabel])).ToArray();
            listBox.SelectedIndex = index;
        }
    }
}