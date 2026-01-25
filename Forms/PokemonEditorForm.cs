using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static ImpostersOrdeal.GameDataTypes;

namespace ImpostersOrdeal
{
    public partial class PokemonEditorForm : Form
    {
        private GameDataSet gameData;

        public List<string> pokemon;
        public List<string> moves;
        public List<string> typings;
        public List<string> items;
        public List<string> abilities;
        public Dictionary<int, string> tms;
        public PokemonDataTable.PokemonData p;

        public readonly string[] colors = new string[]
            {
                "Red", "Blue", "Yellow", "Green", "Black",
                "Brown", "Purple", "Gray", "White", "Pink"
            };

        public readonly string[] eggGroups = new string[]
            {
                "", "Monster", "Water 1", "Bug", "Flying", "Field", "Fairy", "Grass",
                "Human-Like", "Water 3", "Mineral", "Amorphous", "Water 2", "Ditto", "Dragon", "Undiscovered"
            };

        public PokemonEditorForm(GameDataSet gameData)
        {
            this.gameData = gameData;

            InitializeComponent();

            pokemon = gameData.GetAllLabels(Constants.POKEMONSPECIES_MESSAGEFILE_NAME);
            moves = gameData.GetAllLabels(Constants.MOVE_MESSAGEFILE_NAME);
            typings = gameData.GetAllLabels(Constants.TYPE_MESSAGEFILE_NAME);
            items = gameData.GetAllLabels(Constants.ITEM_MESSAGEFILE_NAME);
            abilities = gameData.GetAllLabels(Constants.ABILITY_MESSAGEFILE_NAME);
            tms = new();

            // TODO: TM validity stuff
            /*for (int tmID = 0; tmID < gameData.itemTable.WazaMachine.Count; tmID++)
                if (gameData.itemTable.WazaMachine[tmID].IsValid() && !tms.ContainsKey(gameData.itemTable.Item[gameData.itemTable.WazaMachine[tmID].itemNo].group_id - 1))
                    tms[gameData.itemTable.Item[gameData.itemTable.WazaMachine[tmID].itemNo].group_id - 1] = gameData.itemTable.WazaMachine[tmID].GetFullName();*/
            for (int tmID = 0; tmID < gameData.itemTable.WazaMachine.Count; tmID++)
                if (!tms.ContainsKey(gameData.itemTable.Item[gameData.itemTable.WazaMachine[tmID].itemNo].group_id - 1))
                    tms[gameData.itemTable.Item[gameData.itemTable.WazaMachine[tmID].itemNo].group_id - 1] = string.Format("TM{0}", gameData.itemTable.WazaMachine[tmID].machineNo);

            dexIDComboBox.DataSource = pokemon;
            dexIDComboBox.SelectedIndex = 0;
            formIDComboBox.DataSource = gameData.GetAllFormNames(0);
            formIDComboBox.SelectedIndex = 0;
            p = gameData.pokemonDataTable.Data[0];

            type1ComboBox.DataSource = typings.ToArray();
            type2ComboBox.DataSource = typings.ToArray();

            item1ComboBox.DataSource = items.ToArray();
            item2ComboBox.DataSource = items.ToArray();
            item3ComboBox.DataSource = items.ToArray();

            ability1ComboBox.DataSource = abilities.ToArray();
            ability2ComboBox.DataSource = abilities.ToArray();
            hiddenAbilityComboBox.DataSource = abilities.ToArray();

            colorComboBox.DataSource = colors;
            eggGroup1ComboBox.DataSource = eggGroups.ToArray();
            eggGroup2ComboBox.DataSource = eggGroups.ToArray();
            // TODO: Name stuff
            //growthComboBox.DataSource = gameData.growthRates.Select(g => g.GetName()).ToArray();
            growthComboBox.DataSource = gameData.growthRateTable.Rates.Select(g => string.Format("{0}", g.id)).ToArray();

            tmCompatibilityCheckedListBox.Items.Clear();
            tmCompatibilityCheckedListBox.Items.AddRange(tms.Values.ToArray());

            levelUpMoveColumn.DataSource = moves.ToArray();
            levelColumn.ValueType = typeof(ushort);
            eggMoveColumn.DataSource = moves.ToArray();

            RefreshPokemonDisplay();

            ActivateControls();
        }

        private void DexIDChanged(object sender, EventArgs e)
        {
            DeactivateControls();

            formIDComboBox.DataSource = gameData.GetAllFormNames(dexIDComboBox.SelectedIndex);
            formIDComboBox.SelectedIndex = 0;
            p = gameData.pokemonDataTable.Data[dexIDComboBox.SelectedIndex];
            RefreshPokemonDisplay();

            ActivateControls();
        }

        private void FormIDChanged(object sender, EventArgs e)
        {
            DeactivateControls();

            p = gameData.pokemonDataTable.Data[dexIDComboBox.SelectedIndex];
            RefreshPokemonDisplay();

            ActivateControls();
        }

        private void RefreshPokemonDisplay()
        {
            personalIDTextBox.Text = p.personal.id.ToString();

            numericUpDown1.Value = p.personal.basic_hp;
            numericUpDown2.Value = p.personal.basic_atk;
            numericUpDown3.Value = p.personal.basic_def;
            numericUpDown4.Value = p.personal.basic_spatk;
            numericUpDown5.Value = p.personal.basic_spdef;
            numericUpDown6.Value = p.personal.basic_agi;
            //bstTextBox.Text = p.GetBST().ToString();
            bstTextBox.Text = string.Format("{0}", numericUpDown1.Value + numericUpDown2.Value + numericUpDown3.Value + numericUpDown4.Value + numericUpDown5.Value + numericUpDown6.Value);

            // TODO: evyield
            //int[] evYield = p.GetEvYield();
            int[] evYield = new int[] {0, 0, 0, 0, 0, 0};
            numericUpDown7.Value = evYield[0];
            numericUpDown8.Value = evYield[1];
            numericUpDown9.Value = evYield[2];
            numericUpDown10.Value = evYield[4];
            numericUpDown11.Value = evYield[5];
            numericUpDown12.Value = evYield[3];

            type1ComboBox.SelectedIndex = p.personal.type1;
            type2ComboBox.SelectedIndex = p.personal.type2;

            item1ComboBox.SelectedIndex = p.personal.item1;
            item2ComboBox.SelectedIndex = p.personal.item2;
            item3ComboBox.SelectedIndex = p.personal.item3;

            ability1ComboBox.SelectedIndex = p.personal.tokusei1;
            ability2ComboBox.SelectedIndex = p.personal.tokusei2;
            hiddenAbilityComboBox.SelectedIndex = p.personal.tokusei3;

            colorComboBox.SelectedIndex = p.personal.color;
            numericUpDown13.Value = p.personal.gra_no;
            numericUpDown14.Value = p.personal.get_rate;
            numericUpDown15.Value = p.personal.rank;
            numericUpDown16.Value = p.personal.sex;
            numericUpDown17.Value = p.personal.egg_birth;
            numericUpDown18.Value = p.personal.initial_friendship;
            eggGroup1ComboBox.SelectedIndex = p.personal.egg_group1;
            eggGroup2ComboBox.SelectedIndex = p.personal.egg_group2;
            growthComboBox.SelectedIndex = p.personal.grow;
            numericUpDown19.Value = p.personal.give_exp;
            numericUpDown20.Value = p.personal.height;
            numericUpDown21.Value = p.personal.weight;

            // TODO: tmcompat
            //bool[] tmCompatibility = p.GetTMCompatibility();
            bool[] tmCompatibility = Enumerable.Range(0, 256).Select(i => true).ToArray();
            int[] tmKeys = tms.Keys.ToArray();
            for (int i = 0; i < tmCompatibilityCheckedListBox.Items.Count; i++)
                if (tmKeys[i] >= 0 && tmKeys[i] < tmCompatibility.Length)
                    tmCompatibilityCheckedListBox.SetItemChecked(i, tmCompatibility[tmKeys[i]]);
                else
                    tmCompatibilityCheckedListBox.SetItemChecked(i, false);

            levelUpMoveDataGridView.Rows.Clear();
            for (int i = 0; i < p.levelUpMoves.moves.Count; i++)
                levelUpMoveDataGridView.Rows.Add(new object[] { p.levelUpMoves.moves[i].level, moves[p.levelUpMoves.moves[i].move] });

            eggMoveDataGridView.Rows.Clear();
            for (int i = 0; i < p.eggMoves.wazaNo.Count; i++)
                eggMoveDataGridView.Rows.Add(new object[] { moves[p.eggMoves.wazaNo[i]] });
        }

        private void CommitEdit(object sender, EventArgs e)
        {
            p.personal.basic_hp = (byte)numericUpDown1.Value;
            p.personal.basic_atk = (byte)numericUpDown2.Value;
            p.personal.basic_def = (byte)numericUpDown3.Value;
            p.personal.basic_spatk = (byte)numericUpDown4.Value;
            p.personal.basic_spdef = (byte)numericUpDown5.Value;
            p.personal.basic_agi = (byte)numericUpDown6.Value;
            //bstTextBox.Text = p.GetBST().ToString();
            bstTextBox.Text = string.Format("{0}", numericUpDown1.Value + numericUpDown2.Value + numericUpDown3.Value + numericUpDown4.Value + numericUpDown5.Value + numericUpDown6.Value);

            // TODO: evyield
            /*p.SetEvYield(new int[]
            {
                (int)numericUpDown7.Value,
                (int)numericUpDown8.Value,
                (int)numericUpDown9.Value,
                (int)numericUpDown12.Value,
                (int)numericUpDown10.Value,
                (int)numericUpDown11.Value
            });*/

            p.personal.type1 = (byte)(type1ComboBox.SelectedIndex == -1 ? 0 : type1ComboBox.SelectedIndex);
            p.personal.type2 = (byte)(type2ComboBox.SelectedIndex == -1 ? 0 : type2ComboBox.SelectedIndex);

            p.personal.item1 = (ushort)(item1ComboBox.SelectedIndex == -1 ? 0 : item1ComboBox.SelectedIndex);
            p.personal.item2 = (ushort)(item2ComboBox.SelectedIndex == -1 ? 0 : item2ComboBox.SelectedIndex);
            p.personal.item3 = (ushort)(item3ComboBox.SelectedIndex == -1 ? 0 : item3ComboBox.SelectedIndex);

            p.personal.tokusei1 = (ushort)(ability1ComboBox.SelectedIndex == -1 ? 0 : ability1ComboBox.SelectedIndex);
            p.personal.tokusei2 = (ushort)(ability2ComboBox.SelectedIndex == -1 ? 0 : ability2ComboBox.SelectedIndex);
            p.personal.tokusei3 = (ushort)(hiddenAbilityComboBox.SelectedIndex == -1 ? 0 : hiddenAbilityComboBox.SelectedIndex);

            p.personal.color = (byte)(colorComboBox.SelectedIndex == -1 ? 0 : colorComboBox.SelectedIndex);
            p.personal.gra_no = (ushort)numericUpDown13.Value;
            p.personal.get_rate = (byte)numericUpDown14.Value;
            p.personal.rank = (byte)numericUpDown15.Value;
            p.personal.sex = (byte)numericUpDown16.Value;
            p.personal.egg_birth = (byte)numericUpDown17.Value;
            p.personal.initial_friendship = (byte)numericUpDown18.Value;
            p.personal.egg_group1 = (byte)(eggGroup1ComboBox.SelectedIndex == -1 ? 0 : eggGroup1ComboBox.SelectedIndex);
            p.personal.egg_group2 = (byte)(eggGroup2ComboBox.SelectedIndex == -1 ? 0 : eggGroup2ComboBox.SelectedIndex);
            p.personal.grow = (byte)(growthComboBox.SelectedIndex == -1 ? 0 : growthComboBox.SelectedIndex);
            p.personal.give_exp = (ushort)numericUpDown19.Value;
            p.personal.height = (ushort)numericUpDown20.Value;
            p.personal.weight = (ushort)numericUpDown21.Value;

            // TODO: tmcompat
            /*bool[] tmCompatibility = new bool[gameData.GetTMCompatibilitySetSize()];
            for (int i = 0; i < tmCompatibilityCheckedListBox.Items.Count; i++)
                tmCompatibility[tms.Keys.ToArray()[i]] = tmCompatibilityCheckedListBox.GetItemChecked(i);
            p.SetTMCompatibility(tmCompatibility);*/

            List<PokemonDataTable.PokemonData.SheetWazaOboe.LearnedMove> levelUpMoves = new();
            for (int i = 0; i < levelUpMoveDataGridView.Rows.Count; i++)
            {
                if (levelUpMoveDataGridView.Rows[i].Cells[0].Value == null ||
                    levelUpMoveDataGridView.Rows[i].Cells[1].Value == null ||
                    moves.IndexOf((string)levelUpMoveDataGridView.Rows[i].Cells[1].Value) == 0)
                    continue;
                var l = new PokemonDataTable.PokemonData.SheetWazaOboe.LearnedMove();
                l.level = (ushort)levelUpMoveDataGridView.Rows[i].Cells[0].Value;
                l.move = (ushort)moves.IndexOf((string)levelUpMoveDataGridView.Rows[i].Cells[1].Value);
                levelUpMoves.Add(l);
            }
            levelUpMoves.Sort((l1, l2) => l1.level.CompareTo(l2.level));
            p.levelUpMoves.moves = levelUpMoves;

            List<ushort> eggMoves = new();
            for (int i = 0; i < eggMoveDataGridView.Rows.Count; i++)
            {
                if (eggMoveDataGridView.Rows[i].Cells[0].Value == null ||
                    moves.IndexOf((string)eggMoveDataGridView.Rows[i].Cells[0].Value) == 0)
                    continue;
                eggMoves.Add((ushort)moves.IndexOf((string)eggMoveDataGridView.Rows[i].Cells[0].Value));
            }
            p.eggMoves.wazaNo = eggMoves;
        }

        private void ActivateControls()
        {
            dexIDComboBox.SelectedIndexChanged += DexIDChanged;
            formIDComboBox.SelectedIndexChanged += FormIDChanged;
            numericUpDown1.ValueChanged += CommitEdit;
            numericUpDown2.ValueChanged += CommitEdit;
            numericUpDown3.ValueChanged += CommitEdit;
            numericUpDown4.ValueChanged += CommitEdit;
            numericUpDown5.ValueChanged += CommitEdit;
            numericUpDown6.ValueChanged += CommitEdit;
            numericUpDown12.ValueChanged += CommitEdit;
            numericUpDown11.ValueChanged += CommitEdit;
            numericUpDown10.ValueChanged += CommitEdit;
            numericUpDown9.ValueChanged += CommitEdit;
            numericUpDown8.ValueChanged += CommitEdit;
            numericUpDown7.ValueChanged += CommitEdit;
            type1ComboBox.SelectedIndexChanged += CommitEdit;
            type2ComboBox.SelectedIndexChanged += CommitEdit;
            item1ComboBox.SelectedIndexChanged += CommitEdit;
            item2ComboBox.SelectedIndexChanged += CommitEdit;
            item3ComboBox.SelectedIndexChanged += CommitEdit;
            ability1ComboBox.SelectedIndexChanged += CommitEdit;
            ability2ComboBox.SelectedIndexChanged += CommitEdit;
            hiddenAbilityComboBox.SelectedIndexChanged += CommitEdit;
            colorComboBox.SelectedIndexChanged += CommitEdit;
            numericUpDown13.ValueChanged += CommitEdit;
            numericUpDown14.ValueChanged += CommitEdit;
            numericUpDown15.ValueChanged += CommitEdit;
            numericUpDown16.ValueChanged += CommitEdit;
            numericUpDown17.ValueChanged += CommitEdit;
            numericUpDown18.ValueChanged += CommitEdit;
            eggGroup1ComboBox.SelectedIndexChanged += CommitEdit;
            eggGroup2ComboBox.SelectedIndexChanged += CommitEdit;
            growthComboBox.SelectedIndexChanged += CommitEdit;
            numericUpDown19.ValueChanged += CommitEdit;
            numericUpDown20.ValueChanged += CommitEdit;
            numericUpDown21.ValueChanged += CommitEdit;
            tmCompatibilityCheckedListBox.Leave += CommitEdit;
            levelUpMoveDataGridView.CellEndEdit += CommitEdit;
            levelUpMoveDataGridView.UserDeletedRow += CommitEdit;
            eggMoveDataGridView.CellEndEdit += CommitEdit;
            eggMoveDataGridView.UserDeletedRow += CommitEdit;
        }

        private void DeactivateControls()
        {
            dexIDComboBox.SelectedIndexChanged -= DexIDChanged;
            formIDComboBox.SelectedIndexChanged -= FormIDChanged;
            numericUpDown1.ValueChanged -= CommitEdit;
            numericUpDown2.ValueChanged -= CommitEdit;
            numericUpDown3.ValueChanged -= CommitEdit;
            numericUpDown4.ValueChanged -= CommitEdit;
            numericUpDown5.ValueChanged -= CommitEdit;
            numericUpDown6.ValueChanged -= CommitEdit;
            numericUpDown12.ValueChanged -= CommitEdit;
            numericUpDown11.ValueChanged -= CommitEdit;
            numericUpDown10.ValueChanged -= CommitEdit;
            numericUpDown9.ValueChanged -= CommitEdit;
            numericUpDown8.ValueChanged -= CommitEdit;
            numericUpDown7.ValueChanged -= CommitEdit;
            type1ComboBox.SelectedIndexChanged -= CommitEdit;
            type2ComboBox.SelectedIndexChanged -= CommitEdit;
            item1ComboBox.SelectedIndexChanged -= CommitEdit;
            item2ComboBox.SelectedIndexChanged -= CommitEdit;
            item3ComboBox.SelectedIndexChanged -= CommitEdit;
            ability1ComboBox.SelectedIndexChanged -= CommitEdit;
            ability2ComboBox.SelectedIndexChanged -= CommitEdit;
            hiddenAbilityComboBox.SelectedIndexChanged -= CommitEdit;
            colorComboBox.SelectedIndexChanged -= CommitEdit;
            numericUpDown13.ValueChanged -= CommitEdit;
            numericUpDown14.ValueChanged -= CommitEdit;
            numericUpDown15.ValueChanged -= CommitEdit;
            numericUpDown16.ValueChanged -= CommitEdit;
            numericUpDown17.ValueChanged -= CommitEdit;
            numericUpDown18.ValueChanged -= CommitEdit;
            eggGroup1ComboBox.SelectedIndexChanged -= CommitEdit;
            eggGroup2ComboBox.SelectedIndexChanged -= CommitEdit;
            growthComboBox.SelectedIndexChanged -= CommitEdit;
            numericUpDown19.ValueChanged -= CommitEdit;
            numericUpDown20.ValueChanged -= CommitEdit;
            numericUpDown21.ValueChanged -= CommitEdit;
            tmCompatibilityCheckedListBox.Leave -= CommitEdit;
            levelUpMoveDataGridView.CellEndEdit -= CommitEdit;
            levelUpMoveDataGridView.UserDeletedRow -= CommitEdit;
            eggMoveDataGridView.CellEndEdit -= CommitEdit;
            eggMoveDataGridView.UserDeletedRow -= CommitEdit;
        }

        private void DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MainForm.ShowDataError();
        }

        private void LevelUpMoveDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.SetValues(new object[] { (ushort)0, moves[0] });
        }

        private void EggMoveDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.SetValues(new object[] { moves[0] });
        }

        private void OpenEvolutionEditor(object sender, EventArgs e)
        {
            EvolutionEditorForm eef = new(this, gameData);
            eef.Show();
        }
    }
}
