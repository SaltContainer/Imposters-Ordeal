using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ImpostersOrdeal
{
    public partial class EvolutionEditorForm : Form
    {
        private GameDataSet gameData;

        private readonly PokemonEditorForm pef;
        private readonly PokemonDataTable.PokemonData p;
        private PokemonDataTable.PokemonData.SheetEvolve.EvolutionPath m;

        private readonly string[] evolutionMethods = new string[]
        {
            "", "On LvUp: high friendship", "On LvUp: high friendship & is day", "On LvUp: high friendship & is night",
            "On LvUp: Lv ≥ LvReq", "On Trade", "On Trade: holds item", "Karrablast/Shelmet Trade",
            "On UseItem", "On LvUp: Lv ≥ LvReq & Atk > Def", "On LvUp: Lv ≥ LvReq & Def > Atk", "On LvUp: Lv ≥ LvReq & Atk = Def",
            "On LvUp: Lv ≥ LvReq & rng(0-9) ≤ 4", "On LvUp: Lv ≥ LvReq & rng(0-9) > 4", "On LvUp: Lv ≥ LvReq → Get Shedinja", "SPECIAL_NUKENIN",
            "On LvUp: high beauty", "On UseItem: is male", "On UseItem: is female", "On LvUp: Lv ≥ LvReq & holds item & is day",
            "On LvUp: Lv ≥ LvReq & holds item & is night", "On LvUp: has move", "On LvUp: Pokémon in party", "On LvUp: Lv ≥ LvReq & is male",
            "On LvUp: Lv ≥ LvReq & is female", "On LvUp: is by magnetic field", "On LvUp: is by moss rock", "On LvUp: is by ice rock",
            "On LvUp: Lv ≥ LvReq & device upside down", "On LvUp: high friendship & has move of type", "On LvUp: Lv ≥ LvReq & Dark Pokémon in party", "On LvUp: Lv ≥ LvReq & is raining",
            "On LvUp: Lv ≥ LvReq & is day", "On LvUp: Lv ≥ LvReq & is night", "On LvUp: Lv ≥ LvReq & is female → set form to 1", "FRIENDLY",
            "On LvUp: Lv ≥ LvReq & is game version", "On LvUp: Lv ≥ LvReq & is game version & is day", "On LvUp: Lv ≥ LvReq & is game version & is night", "On LvUp: is by summit",
            "On LvUp: Lv ≥ LvReq & is dusk", "On LvUp: Lv ≥ LvReq & is outside region", "On UseItem: is outside region", "Galarian Farfetch'd Evolution",
            "Galarian Yamask Evolution", "Milcery Evolution", "On LvUp: Lv ≥ LvReq & has amped nature", "On LvUp: Lv ≥ LvReq & has low-key nature"
        };

        private readonly bool[] lvReqMethods = new bool[]
        {
            false, true, true, true, true, false, false, false, false, true, true, true, true, true, true, true,
            true, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true,
            true, true, true, true, true, true, true, true, true, true, false, true, false, false, true, true
        };

        private readonly EvolutionParamType[] paramTypes = new EvolutionParamType[]
        {
            EvolutionParamType.None, EvolutionParamType.None, EvolutionParamType.None, EvolutionParamType.None,
            EvolutionParamType.None, EvolutionParamType.None, EvolutionParamType.Item, EvolutionParamType.None,
            EvolutionParamType.Item, EvolutionParamType.None, EvolutionParamType.None, EvolutionParamType.None,
            EvolutionParamType.None, EvolutionParamType.None, EvolutionParamType.None, EvolutionParamType.None,
            EvolutionParamType.None, EvolutionParamType.Item, EvolutionParamType.Item, EvolutionParamType.Item,
            EvolutionParamType.Item, EvolutionParamType.Move, EvolutionParamType.Pokemon, EvolutionParamType.None,
            EvolutionParamType.None, EvolutionParamType.None, EvolutionParamType.None, EvolutionParamType.None,
            EvolutionParamType.None, EvolutionParamType.Typing, EvolutionParamType.None, EvolutionParamType.None,
            EvolutionParamType.None, EvolutionParamType.None, EvolutionParamType.None, EvolutionParamType.None,
            EvolutionParamType.GameVersion, EvolutionParamType.GameVersion, EvolutionParamType.GameVersion, EvolutionParamType.None,
            EvolutionParamType.None, EvolutionParamType.None, EvolutionParamType.Item, EvolutionParamType.Byte,
            EvolutionParamType.None, EvolutionParamType.None, EvolutionParamType.None, EvolutionParamType.None,
        };

        private enum EvolutionParamType
        {
            None, Item, Move, Pokemon, Typing, GameVersion, Byte
        }

        public EvolutionEditorForm(PokemonEditorForm pef, GameDataSet gameData)
        {
            this.pef = pef;
            p = pef.p;
            this.gameData = gameData;

            InitializeComponent();
            Text = string.Format("Evolution Editor: {0}", gameData.GetLabelByIndex(Constants.POKEMONSPECIES_MESSAGEFILE_NAME, p.personal.monsno));

            destinationDexIDColumn.DataSource = gameData.GetAllLabels(Constants.POKEMONSPECIES_MESSAGEFILE_NAME);
            methodColumn.DataSource = GetEvolutionMethods();

            p.evolutionPaths.paths.ForEach(e => dataGridView.Rows.Add(new object[] { gameData.GetAllFormNames(e.toMonsno)[e.toFormno], GetEvolutionMethods()[e.method] }));

            RefreshEvolutionPathDisplay();

            ActivateControls();
        }

        private List<string> GetEvolutionMethods()
        {
            // TODO: support extra evos
            List<string> ems = evolutionMethods.ToList();
            /*int emCount = gameData.GetEvolutionMethodCount();
            for (int i = ems.Count; i < emCount; i++)
                ems.Add(i.ToString());*/
            return ems;
        }

        private List<bool> GetLvReqMethods()
        {
            // TODO: support extra evos
            List<bool> lrms = lvReqMethods.ToList();
            /*int emCount = gameData.GetEvolutionMethodCount();
            for (int i = lrms.Count; i < emCount; i++)
                lrms.Add(true);*/
            return lrms;
        }

        private List<EvolutionParamType> GetParamTypes()
        {
            // TODO: support extra evos
            List<EvolutionParamType> pts = paramTypes.ToList();
            /*int emCount = gameData.GetEvolutionMethodCount();
            for (int i = pts.Count; i < emCount; i++)
                pts.Add(EvolutionParamType.Byte);*/
            return pts;
        }

        private void FocusEvoPathChanged(object sender, EventArgs e)
        {
            DeactivateControls();

            if (dataGridView.CurrentCell == null || dataGridView.CurrentCell.RowIndex >= p.evolutionPaths.paths.Count)
                m = null;
            else
                m = p.evolutionPaths.paths[dataGridView.CurrentCell.RowIndex];

            RefreshEvolutionPathDisplay();
            ActivateControls();
        }

        private void CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DeactivateControls();
            if (m == null)
            {
                m = new();
                p.evolutionPaths.paths.Add(m);
            }
            if (e.ColumnIndex == 0)
            {
                m.toMonsno = (ushort)pef.pokemon.IndexOf((string)dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                if (m.toMonsno == ushort.MaxValue)
                    m.toMonsno = 0;
                m.toFormno = 0;
            }
            if (e.ColumnIndex == 1)
            {
                int evoMethod = GetEvolutionMethods().IndexOf((string)dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                m.method = evoMethod == -1 ? (ushort)0 : (ushort)evoMethod;
                m.param = 0;
            }

            RefreshEvolutionPathDisplay();
            ActivateControls();
        }

        private void UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            p.evolutionPaths.paths.RemoveAt(e.Row.Index);
        }

        private void RefreshEvolutionPathDisplay()
        {
            if (m == null)
            {
                label1.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                formIDComboBox.Visible = false;
                lvReqNumericUpDown.Visible = false;
                evoParamComboBox.Visible = false;
                return;
            }


            label1.Visible = true;
            formIDComboBox.Visible = true;
            formIDComboBox.DataSource = gameData.GetAllFormNames(m.toMonsno);
            formIDComboBox.SelectedIndex = m.toFormno;

            if (GetLvReqMethods()[m.method])
            {
                label2.Visible = true;
                lvReqNumericUpDown.Visible = true;
                lvReqNumericUpDown.Value = m.level;
            }
            else
            {
                label2.Visible = false;
                lvReqNumericUpDown.Visible = false;
            }

            switch (GetParamTypes()[m.method])
            {
                case EvolutionParamType.None:
                    label3.Visible = false;
                    evoParamComboBox.Visible = false;
                    break;

                case EvolutionParamType.Item:
                    label3.Visible = true;
                    label3.Text = "Item";
                    evoParamComboBox.Visible = true;
                    evoParamComboBox.DataSource = pef.items.ToArray();
                    evoParamComboBox.SelectedIndex = m.param;
                    break;

                case EvolutionParamType.Move:
                    label3.Visible = true;
                    label3.Text = "Move";
                    evoParamComboBox.Visible = true;
                    evoParamComboBox.DataSource = pef.moves.ToArray();
                    evoParamComboBox.SelectedIndex = m.param;
                    break;

                case EvolutionParamType.Pokemon:
                    label3.Visible = true;
                    label3.Text = "Pokémon";
                    evoParamComboBox.Visible = true;
                    evoParamComboBox.DataSource = pef.pokemon.ToArray();
                    evoParamComboBox.SelectedIndex = m.param;
                    break;

                case EvolutionParamType.Typing:
                    label3.Visible = true;
                    label3.Text = "Type";
                    evoParamComboBox.Visible = true;
                    evoParamComboBox.DataSource = pef.typings.ToArray();
                    evoParamComboBox.SelectedIndex = m.param;
                    break;

                case EvolutionParamType.GameVersion:
                    label3.Visible = true;
                    label3.Text = "Game Version";
                    evoParamComboBox.Visible = true;
                    evoParamComboBox.DataSource = Enumerable.Range(0, 256).Select(i => i.ToString()).ToArray();
                    evoParamComboBox.SelectedIndex = m.param;
                    break;

                case EvolutionParamType.Byte:
                    label3.Visible = true;
                    label3.Text = "Argument";
                    evoParamComboBox.Visible = true;
                    evoParamComboBox.DataSource = Enumerable.Range(0, 256).Select(i => i.ToString()).ToArray();
                    evoParamComboBox.SelectedIndex = m.param;
                    break;
            }
        }

        private void CommitEdit(object sender, EventArgs e)
        {
            if (formIDComboBox.Visible)
                m.toFormno = (ushort)(formIDComboBox.SelectedIndex == -1 ? 0 : formIDComboBox.SelectedIndex);
            if (lvReqNumericUpDown.Visible)
                m.level = (ushort)lvReqNumericUpDown.Value;
            if (evoParamComboBox.Visible)
                m.param = (ushort)(evoParamComboBox.SelectedIndex == -1 ? 0 : evoParamComboBox.SelectedIndex);
        }

        private void ActivateControls()
        {
            dataGridView.CellEnter += FocusEvoPathChanged;
            dataGridView.CellEndEdit += CellEndEdit;
            dataGridView.UserDeletingRow += UserDeletingRow;
            formIDComboBox.SelectedIndexChanged += CommitEdit;
            lvReqNumericUpDown.ValueChanged += CommitEdit;
            evoParamComboBox.SelectedIndexChanged += CommitEdit;
        }

        private void DeactivateControls()
        {
            dataGridView.CellEnter -= FocusEvoPathChanged;
            dataGridView.CellEndEdit -= CellEndEdit;
            dataGridView.UserDeletingRow -= UserDeletingRow;
            formIDComboBox.SelectedIndexChanged -= CommitEdit;
            lvReqNumericUpDown.ValueChanged -= CommitEdit;
            evoParamComboBox.SelectedIndexChanged -= CommitEdit;
        }
    }
}
