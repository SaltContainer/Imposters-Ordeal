using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ImpostersOrdeal
{
    public partial class ItemEditorForm : Form
    {
        GameDataSet gameData;

        List<ItemTable.SheetItem> items;
        Dictionary<int, string> itemTypes = new();
        Dictionary<int, string> typings0 = new();
        Dictionary<int, string> itemGroups = new();
        Dictionary<int, string> fieldFunctions = new();
        ItemTable.SheetItem i;

        private string[] pluckEffects = new string[]
        {
            "None",
            "Cheri Berry",
            "Chesto Berry",
            "Pecha Berry",
            "Rawst Berry",
            "Aspear Berry",
            "Leppa Berry",
            "Oran Berry",
            "Persim Berry",
            "Lum Berry",
            "Sitrus Berry",
            "Figy Berry",
            "Wiki Berry",
            "Mago Berry",
            "Aguav Berry",
            "Iapapa Berry",
            "Liechi Berry",
            "Ganlon Berry",
            "Salac Berry",
            "Petaya Berry",
            "Apicot Berry",
            "Lansat Berry",
            "Starf Berry",
            "Micle Berry"
        };

        private string[] bagPockets = new string[]
        {
            "Medicine",
            "Poké Balls",
            "Battle Items",
            "Berries",
            "Other Items",
            "TMs",
            "Treasures",
            "Ingredients",
            "Key Items",
            "Unknown"
        };

        private string[] battleFunctions = new string[]
        {
            "None",
            "Poké Ball",
            "Alter Pokémon",
            "Escape"
        };

        public ItemEditorForm(GameDataSet gameData)
        {
            this.gameData = gameData;

            itemTypes[0] = "Bag Pocket";
            itemTypes[1] = "Medicine";
            itemTypes[2] = "Hold Item";
            itemTypes[3] = "Misc Item";
            itemTypes[4] = "Battle Item";
            itemTypes[5] = "Poké Ball";
            itemTypes[6] = "Mail";
            itemTypes[7] = "TM";
            itemTypes[8] = "Berry";
            itemTypes[9] = "Key Item";
            itemTypes[10] = "Treasure";
            itemTypes[255] = "None";

            var types = gameData.GetAllLabels(Constants.TYPE_MESSAGEFILE_NAME);
            for (int i = 0; i < types.Count; i++)
                typings0[i] = types[i];
            typings0[31] = "None";

            itemGroups[0] = "None";
            itemGroups[1] = "Poké Ball";
            itemGroups[2] = "Bag Pocket";
            itemGroups[3] = "Berry";
            itemGroups[4] = "TM";
            itemGroups[6] = "Gem";
            itemGroups[7] = "Mega Stone";
            itemGroups[8] = "Z-Crystal";
            itemGroups[9] = "Z-Crystal (unequip)";
            itemGroups[10] = "Rotom Power";
            itemGroups[11] = "Candy";

            fieldFunctions[0] = "None";
            fieldFunctions[1] = "Alter Pokémon";
            fieldFunctions[2] = "Use TM";
            fieldFunctions[3] = "Mount Bike";
            fieldFunctions[4] = "Honey";
            fieldFunctions[5] = "Toggle Passive";
            fieldFunctions[6] = "Trigger Evolution";
            fieldFunctions[7] = "Escape Rope";
            fieldFunctions[8] = "Change Ability";
            fieldFunctions[9] = "Eon Flute";
            fieldFunctions[11] = "Mail";
            fieldFunctions[12] = "Berry";
            fieldFunctions[13] = "Super Rod";
            fieldFunctions[14] = "Vs. Recorder";
            fieldFunctions[15] = "Change Form";
            fieldFunctions[17] = "Change Form (dual)";
            fieldFunctions[18] = "Activate Rotom Power";
            fieldFunctions[19] = "Explorer Kit";
            fieldFunctions[20] = "Guidebook";
            fieldFunctions[21] = "Poffin Case";
            fieldFunctions[22] = "Poké Radar";
            fieldFunctions[23] = "Sprayduck";
            fieldFunctions[24] = "Vs. Seeker";
            fieldFunctions[25] = "Old Rod";
            fieldFunctions[26] = "Good Rod";
            fieldFunctions[27] = "Azure Flute";
            fieldFunctions[28] = "Point Card";
            fieldFunctions[29] = "DS Sounds";

            items = new();
            items.AddRange(gameData.itemTable.Item);
            InitializeComponent();

            listBox.DataSource = gameData.GetAllLabels(Constants.ITEM_MESSAGEFILE_NAME);
            listBox.SelectedIndex = 0;
            i = items[0];

            itemTypeComboBox.DataSource = itemTypes.Values.ToArray();
            itemGroupComboBox.DataSource = itemGroups.Values.ToArray();
            bagPocketComboBox.DataSource = bagPockets;

            naturalGiftTypeComboBox.DataSource = typings0.Values.ToArray();
            pluckEffectComboBox.DataSource = pluckEffects;

            fieldFunctionComboBox.DataSource = fieldFunctions.Values.ToArray();
            battleFunctionComboBox.DataSource = battleFunctions;

            RefreshItemDisplay();
            ActivateControls();
        }

        private void ItemChanged(object sender, EventArgs e)
        {
            DeactivateControls();

            i = items[listBox.SelectedIndex];
            RefreshItemDisplay();

            ActivateControls();
        }

        private void RefreshItemDisplay()
        {
            textBox.Text = string.Format("{0} - {1}", i.no, gameData.GetLabelByIndex(Constants.ITEM_MESSAGEFILE_NAME, i.no));

            bool[] flags = i.Flags;

            checkBox25.Checked = i.Enabled;
            iconIDNumericUpDown.Value = i.iconid;
            priceNumericUpDown.Value = i.price;
            bpPriceNumericUpDown.Value = i.bp_price;

            itemTypeComboBox.SelectedItem = itemTypes[i.type];
            sortNumericUpDown.Value = i.sort;
            itemGroupComboBox.SelectedItem = itemGroups[i.group];
            groupIDNumericUpDown.Value = i.group_id;
            bagPocketComboBox.SelectedIndex = i.fld_pocket;

            flingPowerNumericUpDown.Value = i.nage_atc;
            checkBox1.Checked = flags[0];
            naturalGiftTypeComboBox.SelectedItem = typings0[i.sizen_type];
            naturalGiftPowerNumericUpDown.Value = i.sizen_atc;
            pluckEffectComboBox.SelectedIndex = i.tuibamu_eff;

            fieldFunctionComboBox.SelectedItem = fieldFunctions[i.field_func];
            numericUpDown10.Value = i.wk_prm_hp_exp;
            numericUpDown11.Value = i.wk_prm_pow_exp;
            numericUpDown12.Value = i.wk_prm_def_exp;
            numericUpDown13.Value = i.wk_prm_spa_exp;
            numericUpDown14.Value = i.wk_prm_spd_exp;
            numericUpDown15.Value = i.wk_prm_agi_exp;
            numericUpDown16.Value = i.wk_friend1;
            numericUpDown17.Value = i.wk_friend2;
            numericUpDown18.Value = i.wk_friend3;
            checkBox9.Checked = flags[9];
            checkBox12.Checked = flags[12];
            checkBox13.Checked = flags[13];
            checkBox14.Checked = flags[14];
            battleFunctionComboBox.SelectedIndex = i.battle_func;
            numericUpDown3.Value = i.wk_critical_up;
            numericUpDown4.Value = i.wk_atc_up;
            numericUpDown5.Value = i.wk_def_up;
            numericUpDown6.Value = i.wk_spa_up;
            numericUpDown7.Value = i.wk_spd_up;
            numericUpDown8.Value = i.wk_agi_up;
            numericUpDown9.Value = i.wk_hit_up;
            numericUpDown1.Value = i.wk_prm_hp_rcv;
            numericUpDown2.Value = i.wk_prm_pp_rcv;
            checkBox8.Checked = flags[8];
            checkBox10.Checked = flags[10];
            checkBox11.Checked = flags[11];

            checkBox2.Checked = flags[1];
            checkBox3.Checked = flags[2];
            checkBox4.Checked = flags[3];
            checkBox26.Checked = flags[4];
            checkBox5.Checked = flags[5];
            checkBox6.Checked = flags[6];
            checkBox7.Checked = flags[7];
            checkBox15.Checked = flags[15];
            checkBox16.Checked = flags[16];
            checkBox17.Checked = flags[17];
            checkBox18.Checked = flags[18];
            checkBox19.Checked = flags[19];
            checkBox20.Checked = flags[20];
            checkBox21.Checked = flags[21];
            checkBox22.Checked = flags[22];
            checkBox23.Checked = flags[23];
            checkBox24.Checked = flags[24];
        }

        private void CommitEdit(object sender, EventArgs e)
        {
            i.iconid = (int)iconIDNumericUpDown.Value;
            i.price = (int)priceNumericUpDown.Value;
            i.bp_price = (int)bpPriceNumericUpDown.Value;

            i.type = (byte)itemTypes.Keys.ToArray()[itemTypeComboBox.SelectedIndex];
            i.sort = (byte)sortNumericUpDown.Value;
            i.group = (byte)itemGroups.Keys.ToArray()[itemGroupComboBox.SelectedIndex];
            i.group_id = (byte)groupIDNumericUpDown.Value;
            i.fld_pocket = (byte)(bagPocketComboBox.SelectedIndex == -1 ? 0 : bagPocketComboBox.SelectedIndex);

            i.nage_atc = (byte)flingPowerNumericUpDown.Value;
            i.sizen_type = (byte)typings0.Keys.ToArray()[naturalGiftTypeComboBox.SelectedIndex];
            i.sizen_atc = (byte)naturalGiftPowerNumericUpDown.Value;
            i.tuibamu_eff = (byte)(pluckEffectComboBox.SelectedIndex == -1 ? 0 : pluckEffectComboBox.SelectedIndex);

            i.field_func = (byte)fieldFunctions.Keys.ToArray()[fieldFunctionComboBox.SelectedIndex];
            i.wk_prm_hp_exp = (sbyte)numericUpDown10.Value;
            i.wk_prm_pow_exp = (sbyte)numericUpDown11.Value;
            i.wk_prm_def_exp = (sbyte)numericUpDown12.Value;
            i.wk_prm_spa_exp = (sbyte)numericUpDown13.Value;
            i.wk_prm_spd_exp = (sbyte)numericUpDown14.Value;
            i.wk_prm_agi_exp = (sbyte)numericUpDown15.Value;
            i.wk_friend1 = (sbyte)numericUpDown16.Value;
            i.wk_friend2 = (sbyte)numericUpDown17.Value;
            i.wk_friend3 = (sbyte)numericUpDown18.Value;
            i.battle_func = (byte)(battleFunctionComboBox.SelectedIndex == -1 ? 0 : battleFunctionComboBox.SelectedIndex);
            i.wk_critical_up = (byte)numericUpDown3.Value;
            i.wk_atc_up = (byte)numericUpDown4.Value;
            i.wk_def_up = (byte)numericUpDown5.Value;
            i.wk_spa_up = (byte)numericUpDown6.Value;
            i.wk_spd_up = (byte)numericUpDown7.Value;
            i.wk_agi_up = (byte)numericUpDown8.Value;
            i.wk_hit_up = (byte)numericUpDown9.Value;
            i.wk_prm_hp_rcv = (byte)numericUpDown1.Value;
            i.wk_prm_pp_rcv = (byte)numericUpDown2.Value;

            bool[] flags = new bool[32];
            flags[0] = checkBox1.Checked;
            flags[1] = checkBox2.Checked;
            flags[2] = checkBox3.Checked;
            flags[3] = checkBox4.Checked;
            flags[4] = checkBox26.Checked;
            flags[5] = checkBox5.Checked;
            flags[6] = checkBox6.Checked;
            flags[7] = checkBox7.Checked;
            flags[8] = checkBox8.Checked;
            flags[9] = checkBox9.Checked;
            flags[10] = checkBox10.Checked;
            flags[11] = checkBox11.Checked;
            flags[12] = checkBox12.Checked;
            flags[13] = checkBox13.Checked;
            flags[14] = checkBox14.Checked;
            flags[15] = checkBox15.Checked;
            flags[16] = checkBox16.Checked;
            flags[17] = checkBox17.Checked;
            flags[18] = checkBox18.Checked;
            flags[19] = checkBox19.Checked;
            flags[20] = checkBox20.Checked;
            flags[21] = checkBox21.Checked;
            flags[22] = checkBox22.Checked;
            flags[23] = checkBox23.Checked;
            flags[24] = checkBox24.Checked;
            flags[31] = !checkBox25.Checked;

            i.Flags = flags;
        }

        private void ActivateControls()
        {
            listBox.SelectedIndexChanged += ItemChanged;

            checkBox25.CheckedChanged += CommitEdit;
            iconIDNumericUpDown.ValueChanged += CommitEdit;
            priceNumericUpDown.ValueChanged += CommitEdit;
            bpPriceNumericUpDown.ValueChanged += CommitEdit;

            itemTypeComboBox.SelectedIndexChanged += CommitEdit;
            sortNumericUpDown.ValueChanged += CommitEdit;
            itemGroupComboBox.SelectedIndexChanged += CommitEdit;
            groupIDNumericUpDown.ValueChanged += CommitEdit;
            bagPocketComboBox.SelectedIndexChanged += CommitEdit;

            flingPowerNumericUpDown.ValueChanged += CommitEdit;
            checkBox1.CheckedChanged += CommitEdit;
            naturalGiftTypeComboBox.SelectedIndexChanged += CommitEdit;
            naturalGiftPowerNumericUpDown.ValueChanged += CommitEdit;
            pluckEffectComboBox.SelectedIndexChanged += CommitEdit;

            fieldFunctionComboBox.SelectedIndexChanged += CommitEdit;
            numericUpDown10.ValueChanged += CommitEdit;
            numericUpDown11.ValueChanged += CommitEdit;
            numericUpDown12.ValueChanged += CommitEdit;
            numericUpDown13.ValueChanged += CommitEdit;
            numericUpDown14.ValueChanged += CommitEdit;
            numericUpDown15.ValueChanged += CommitEdit;
            numericUpDown16.ValueChanged += CommitEdit;
            numericUpDown17.ValueChanged += CommitEdit;
            numericUpDown18.ValueChanged += CommitEdit;
            checkBox9.CheckedChanged += CommitEdit;
            checkBox12.CheckedChanged += CommitEdit;
            checkBox13.CheckedChanged += CommitEdit;
            checkBox14.CheckedChanged += CommitEdit;
            battleFunctionComboBox.SelectedIndexChanged += CommitEdit;
            numericUpDown3.ValueChanged += CommitEdit;
            numericUpDown4.ValueChanged += CommitEdit;
            numericUpDown5.ValueChanged += CommitEdit;
            numericUpDown6.ValueChanged += CommitEdit;
            numericUpDown7.ValueChanged += CommitEdit;
            numericUpDown8.ValueChanged += CommitEdit;
            numericUpDown9.ValueChanged += CommitEdit;
            numericUpDown1.ValueChanged += CommitEdit;
            numericUpDown2.ValueChanged += CommitEdit;
            checkBox8.CheckedChanged += CommitEdit;
            checkBox10.CheckedChanged += CommitEdit;
            checkBox11.CheckedChanged += CommitEdit;

            checkBox2.CheckedChanged += CommitEdit;
            checkBox3.CheckedChanged += CommitEdit;
            checkBox4.CheckedChanged += CommitEdit;
            checkBox26.CheckedChanged += CommitEdit;
            checkBox5.CheckedChanged += CommitEdit;
            checkBox6.CheckedChanged += CommitEdit;
            checkBox7.CheckedChanged += CommitEdit;
            checkBox8.CheckedChanged += CommitEdit;
            checkBox9.CheckedChanged += CommitEdit;
            checkBox10.CheckedChanged += CommitEdit;
            checkBox11.CheckedChanged += CommitEdit;
            checkBox12.CheckedChanged += CommitEdit;
            checkBox13.CheckedChanged += CommitEdit;
            checkBox14.CheckedChanged += CommitEdit;
            checkBox15.CheckedChanged += CommitEdit;
            checkBox16.CheckedChanged += CommitEdit;
            checkBox17.CheckedChanged += CommitEdit;
            checkBox18.CheckedChanged += CommitEdit;
            checkBox19.CheckedChanged += CommitEdit;
            checkBox20.CheckedChanged += CommitEdit;
            checkBox21.CheckedChanged += CommitEdit;
            checkBox22.CheckedChanged += CommitEdit;
            checkBox23.CheckedChanged += CommitEdit;
            checkBox24.CheckedChanged += CommitEdit;
        }

        private void DeactivateControls()
        {
            listBox.SelectedIndexChanged -= ItemChanged;

            checkBox25.CheckedChanged -= CommitEdit;
            iconIDNumericUpDown.ValueChanged -= CommitEdit;
            priceNumericUpDown.ValueChanged -= CommitEdit;
            bpPriceNumericUpDown.ValueChanged -= CommitEdit;

            itemTypeComboBox.SelectedIndexChanged -= CommitEdit;
            sortNumericUpDown.ValueChanged -= CommitEdit;
            itemGroupComboBox.SelectedIndexChanged -= CommitEdit;
            groupIDNumericUpDown.ValueChanged -= CommitEdit;
            bagPocketComboBox.SelectedIndexChanged -= CommitEdit;

            flingPowerNumericUpDown.ValueChanged -= CommitEdit;
            checkBox1.CheckedChanged -= CommitEdit;
            naturalGiftTypeComboBox.SelectedIndexChanged -= CommitEdit;
            naturalGiftPowerNumericUpDown.ValueChanged -= CommitEdit;
            pluckEffectComboBox.SelectedIndexChanged -= CommitEdit;

            fieldFunctionComboBox.SelectedIndexChanged -= CommitEdit;
            numericUpDown10.ValueChanged -= CommitEdit;
            numericUpDown11.ValueChanged -= CommitEdit;
            numericUpDown12.ValueChanged -= CommitEdit;
            numericUpDown13.ValueChanged -= CommitEdit;
            numericUpDown14.ValueChanged -= CommitEdit;
            numericUpDown15.ValueChanged -= CommitEdit;
            numericUpDown16.ValueChanged -= CommitEdit;
            numericUpDown17.ValueChanged -= CommitEdit;
            numericUpDown18.ValueChanged -= CommitEdit;
            checkBox9.CheckedChanged -= CommitEdit;
            checkBox12.CheckedChanged -= CommitEdit;
            checkBox13.CheckedChanged -= CommitEdit;
            checkBox14.CheckedChanged -= CommitEdit;
            battleFunctionComboBox.SelectedIndexChanged -= CommitEdit;
            numericUpDown3.ValueChanged -= CommitEdit;
            numericUpDown4.ValueChanged -= CommitEdit;
            numericUpDown5.ValueChanged -= CommitEdit;
            numericUpDown6.ValueChanged -= CommitEdit;
            numericUpDown7.ValueChanged -= CommitEdit;
            numericUpDown8.ValueChanged -= CommitEdit;
            numericUpDown9.ValueChanged -= CommitEdit;
            numericUpDown1.ValueChanged -= CommitEdit;
            numericUpDown2.ValueChanged -= CommitEdit;
            checkBox8.CheckedChanged -= CommitEdit;
            checkBox10.CheckedChanged -= CommitEdit;
            checkBox11.CheckedChanged -= CommitEdit;

            checkBox2.CheckedChanged -= CommitEdit;
            checkBox3.CheckedChanged -= CommitEdit;
            checkBox4.CheckedChanged -= CommitEdit;
            checkBox26.CheckedChanged -= CommitEdit;
            checkBox5.CheckedChanged -= CommitEdit;
            checkBox6.CheckedChanged -= CommitEdit;
            checkBox7.CheckedChanged -= CommitEdit;
            checkBox8.CheckedChanged -= CommitEdit;
            checkBox9.CheckedChanged -= CommitEdit;
            checkBox10.CheckedChanged -= CommitEdit;
            checkBox11.CheckedChanged -= CommitEdit;
            checkBox12.CheckedChanged -= CommitEdit;
            checkBox13.CheckedChanged -= CommitEdit;
            checkBox14.CheckedChanged -= CommitEdit;
            checkBox15.CheckedChanged -= CommitEdit;
            checkBox16.CheckedChanged -= CommitEdit;
            checkBox17.CheckedChanged -= CommitEdit;
            checkBox18.CheckedChanged -= CommitEdit;
            checkBox19.CheckedChanged -= CommitEdit;
            checkBox20.CheckedChanged -= CommitEdit;
            checkBox21.CheckedChanged -= CommitEdit;
            checkBox22.CheckedChanged -= CommitEdit;
            checkBox23.CheckedChanged -= CommitEdit;
            checkBox24.CheckedChanged -= CommitEdit;
        }
    }
}
