using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ImpostersOrdeal.GameDataTypes;
using static ImpostersOrdeal.GlobalData;
using static ImpostersOrdeal.JsonConverterStructs;

namespace ImpostersOrdeal
{
    public partial class TrainerPokemonEditorForm : Form
    {
        private GameDataSet gameData;

        private TrainerEditorForm tef;
        private List<string> dexEntries;
        private List<string> natures;
        private List<string> abilities;
        private List<string> moves;
        private TrainerTable.SheetTrainerData.TrainerPoke tp;

        private readonly string[] genders = new string[]
        {
            "Male", "Female", "Genderless", "Random"
        };

        public TrainerPokemonEditorForm(TrainerEditorForm tef, GameDataSet gameData)
        {
            this.gameData = gameData;

            this.tef = tef;
            dexEntries = gameData.GetAllLabels(Constants.POKEMONSPECIES_MESSAGEFILE_NAME);
            natures = gameData.GetAllLabels(Constants.NATURE_MESSAGEFILE_NAME);
            abilities = gameData.GetAllLabels(Constants.ABILITY_MESSAGEFILE_NAME);
            moves = gameData.GetAllLabels(Constants.MOVE_MESSAGEFILE_NAME);

            InitializeComponent();

            speciesComboBox.DataSource = dexEntries.ToArray();
            comboBox3.DataSource = genders.ToArray();
            comboBox4.DataSource = natures.ToArray();
            comboBox5.DataSource = abilities.ToArray();
            comboBox6.DataSource = tef.items.ToArray();
            comboBox7.DataSource = moves.ToArray();
            comboBox8.DataSource = moves.ToArray();
            comboBox9.DataSource = moves.ToArray();
            comboBox10.DataSource = moves.ToArray();
        }

        public void SetTP(TrainerTable.SheetTrainerData t, int tpIdx)
        {
            tp = t.Pokes[tpIdx];
            Text = string.Format("Trainer Pokémon Editor: {0} {1} Pokémon {2}", tef.trainerTypeNames[t.TypeID], gameData.GetLabelByName(Constants.TRAINERNAME_MESSAGEFILE_NAME, t.NameLabel), tpIdx + 1);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            speciesComboBox.SelectedIndex = tp.MonsNo;
            ResetFormComboBox();
            levelNumericUpDown.Value = tp.Level;

            numericUpDown4.Value = tp.TalentHp;
            numericUpDown5.Value = tp.TalentAtk;
            numericUpDown6.Value = tp.TalentDef;
            numericUpDown7.Value = tp.TalentSpAtk;
            numericUpDown8.Value = tp.TalentSpDef;
            numericUpDown9.Value = tp.TalentAgi;

            checkBox1.Checked = tp.IsRare;
            numericUpDown2.Value = tp.Ball;
            numericUpDown3.Value = tp.Seal;

            numericUpDown15.Value = tp.EffortHp;
            numericUpDown14.Value = tp.EffortAtk;
            numericUpDown13.Value = tp.EffortDef;
            numericUpDown12.Value = tp.EffortSpAtk;
            numericUpDown11.Value = tp.EffortSpDef;
            numericUpDown10.Value = tp.EffortAgi;

            comboBox3.SelectedIndex = tp.Sex;
            comboBox4.SelectedIndex = tp.Seikaku;
            comboBox5.SelectedIndex = tp.Tokusei;
            comboBox6.SelectedIndex = tp.Item;

            comboBox7.SelectedIndex = tp.Waza1;
            comboBox8.SelectedIndex = tp.Waza2;
            comboBox9.SelectedIndex = tp.Waza3;
            comboBox10.SelectedIndex = tp.Waza4;

            ActivateControls();
        }

        private void OnClosed(object sender, FormClosedEventArgs e)
        {
            DeactivateControls();
        }

        private void CommitSpeciesEdit(object sender, EventArgs e)
        {
            DeactivateControls();

            tp.MonsNo = (ushort)(speciesComboBox.SelectedIndex == -1 ? 0 : speciesComboBox.SelectedIndex);
            tp.FormNo = 0;
            ResetFormComboBox();

            CommitEdit(sender, e);
            ActivateControls();
        }

        private void CommitEdit(object sender, EventArgs e)
        {
            tp.FormNo = (ushort)(formComboBox.SelectedIndex == -1 ? 0 : formComboBox.SelectedIndex);
            tp.Level = (byte)levelNumericUpDown.Value;

            tp.TalentHp = (byte)numericUpDown4.Value;
            tp.TalentAtk = (byte)numericUpDown5.Value;
            tp.TalentDef = (byte)numericUpDown6.Value;
            tp.TalentSpAtk = (byte)numericUpDown7.Value;
            tp.TalentSpDef = (byte)numericUpDown8.Value;
            tp.TalentAgi = (byte)numericUpDown9.Value;

            tp.IsRare = checkBox1.Checked;
            tp.Ball = (byte)numericUpDown2.Value;
            tp.Seal = (int)numericUpDown3.Value;

            tp.EffortHp = (byte)numericUpDown15.Value;
            tp.EffortAtk = (byte)numericUpDown14.Value;
            tp.EffortDef = (byte)numericUpDown13.Value;
            tp.EffortSpAtk = (byte)numericUpDown12.Value;
            tp.EffortSpDef = (byte)numericUpDown11.Value;
            tp.EffortAgi = (byte)numericUpDown10.Value;

            tp.Sex = (byte)(comboBox3.SelectedIndex == -1 ? 0 : comboBox3.SelectedIndex);
            tp.Seikaku = (byte)(comboBox4.SelectedIndex == -1 ? 0 : comboBox4.SelectedIndex);
            tp.Tokusei = (ushort)(comboBox5.SelectedIndex == -1 ? 0 : comboBox5.SelectedIndex);
            tp.Item = (ushort)(comboBox6.SelectedIndex == -1 ? 0 : comboBox6.SelectedIndex);

            tp.Waza1 = (ushort)(comboBox7.SelectedIndex == -1 ? 0 : comboBox7.SelectedIndex);
            tp.Waza2 = (ushort)(comboBox8.SelectedIndex == -1 ? 0 : comboBox8.SelectedIndex);
            tp.Waza3 = (ushort)(comboBox9.SelectedIndex == -1 ? 0 : comboBox9.SelectedIndex);
            tp.Waza4 = (ushort)(comboBox10.SelectedIndex == -1 ? 0 : comboBox10.SelectedIndex);
        }

        private void ActivateControls()
        {
            speciesComboBox.SelectedIndexChanged += CommitSpeciesEdit;
            formComboBox.SelectedIndexChanged += CommitEdit;
            levelNumericUpDown.ValueChanged += CommitEdit;

            numericUpDown4.ValueChanged += CommitEdit;
            numericUpDown5.ValueChanged += CommitEdit;
            numericUpDown6.ValueChanged += CommitEdit;
            numericUpDown7.ValueChanged += CommitEdit;
            numericUpDown8.ValueChanged += CommitEdit;
            numericUpDown9.ValueChanged += CommitEdit;

            checkBox1.CheckedChanged += CommitEdit;
            numericUpDown2.ValueChanged += CommitEdit;
            numericUpDown3.ValueChanged += CommitEdit;

            numericUpDown15.ValueChanged += CommitEdit;
            numericUpDown14.ValueChanged += CommitEdit;
            numericUpDown13.ValueChanged += CommitEdit;
            numericUpDown12.ValueChanged += CommitEdit;
            numericUpDown11.ValueChanged += CommitEdit;
            numericUpDown10.ValueChanged += CommitEdit;

            comboBox3.SelectedIndexChanged += CommitEdit;
            comboBox4.SelectedIndexChanged += CommitEdit;
            comboBox5.SelectedIndexChanged += CommitEdit;
            comboBox6.SelectedIndexChanged += CommitEdit;

            comboBox7.SelectedIndexChanged += CommitEdit;
            comboBox8.SelectedIndexChanged += CommitEdit;
            comboBox9.SelectedIndexChanged += CommitEdit;
            comboBox10.SelectedIndexChanged += CommitEdit;
        }

        private void DeactivateControls()
        {
            speciesComboBox.SelectedIndexChanged -= CommitSpeciesEdit;
            formComboBox.SelectedIndexChanged -= CommitEdit;
            levelNumericUpDown.ValueChanged -= CommitEdit;

            numericUpDown4.ValueChanged -= CommitEdit;
            numericUpDown5.ValueChanged -= CommitEdit;
            numericUpDown6.ValueChanged -= CommitEdit;
            numericUpDown7.ValueChanged -= CommitEdit;
            numericUpDown8.ValueChanged -= CommitEdit;
            numericUpDown9.ValueChanged -= CommitEdit;

            checkBox1.CheckedChanged -= CommitEdit;
            numericUpDown2.ValueChanged -= CommitEdit;
            numericUpDown3.ValueChanged -= CommitEdit;

            numericUpDown15.ValueChanged -= CommitEdit;
            numericUpDown14.ValueChanged -= CommitEdit;
            numericUpDown13.ValueChanged -= CommitEdit;
            numericUpDown12.ValueChanged -= CommitEdit;
            numericUpDown11.ValueChanged -= CommitEdit;
            numericUpDown10.ValueChanged -= CommitEdit;

            comboBox3.SelectedIndexChanged -= CommitEdit;
            comboBox4.SelectedIndexChanged -= CommitEdit;
            comboBox5.SelectedIndexChanged -= CommitEdit;
            comboBox6.SelectedIndexChanged -= CommitEdit;

            comboBox7.SelectedIndexChanged -= CommitEdit;
            comboBox8.SelectedIndexChanged -= CommitEdit;
            comboBox9.SelectedIndexChanged -= CommitEdit;
            comboBox10.SelectedIndexChanged -= CommitEdit;
        }

        private void ResetFormComboBox()
        {
            formComboBox.SelectedIndex = -1;
            formComboBox.DataSource = gameData.GetAllFormNames(tp.MonsNo);
            formComboBox.SelectedIndex = tp.FormNo;
        }
    }
}
