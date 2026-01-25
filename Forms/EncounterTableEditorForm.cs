using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static ImpostersOrdeal.GlobalData;

namespace ImpostersOrdeal
{
    public partial class EncounterTableEditorForm : Form
    {
        public GameDataSet gameData;

        public List<string> pokemon;
        public List<FieldEncountTable.Sheettable> encounterTables;
        public FieldEncountTable.Sheettable encounterTable;
        public GBAEncounterEditorForm gbaeef;
        public bool uint16DexID;

        public BindingSource pokemonSource = new BindingSource();

        public readonly string[] gameVersions = new string[]
        {
            "Diamond", "Pearl"
        };

        private readonly string[] groundRates = new string[]
        {
            "20%", "20%", "10%", "10%", "5%", "5%", "4%", "4%", "1%", "1%"
        };

        private readonly string[] swarmRates = new string[]
        {
            "20%", "20%"
        };

        private readonly string[] timeRates = new string[]
        {
            "10%", "10%"
        };

        private readonly string[] pokeradarRates = new string[]
        {
            "10%", "10%", "1%", "1%"
        };

        private readonly string[] waterRates = new string[]
        {
            "60%", "30%", "5%", "4%", "1%"
        };

        private readonly string[] sortNames = new string[]
        {
            "Sort by zoneID",
            "Sort by name",
            "Sort by level"
        };

        private readonly Comparison<FieldEncountTable.Sheettable>[] sortComparisons = new Comparison<FieldEncountTable.Sheettable>[]
        {
            (e0, e1) => e0.zoneID.CompareTo(e1.zoneID),
            (e0, e1) => GetZoneName((int)e0.zoneID).CompareTo(GetZoneName((int)e1.zoneID)),
            (e0, e1) => e0.GetAvgLevel().CompareTo(e1.GetAvgLevel())
        };

        public EncounterTableEditorForm(GameDataSet gameData)
        {
            InitializeComponent();

            this.gameData = gameData;

            pokemon = gameData.GetAllLabels(Constants.POKEMONSPECIES_MESSAGEFILE_NAME);
            encounterTables = new();
            encounterTables.AddRange(gameData.encounterTableFiles[0].table);

            sortComboBox.DataSource = sortNames;
            sortComboBox.SelectedIndex = 0;
            encounterTables.Sort(sortComparisons[sortComboBox.SelectedIndex]);

            PopulateListBox();

            versionComboBox.DataSource = gameVersions;
            pokemonSource.DataSource = pokemon.ToArray();
            monsNoGround.DataSource = pokemonSource;
            monsNoSwarm.DataSource = pokemonSource;
            monsNoMorning.DataSource = pokemonSource;
            monsNoDay.DataSource = pokemonSource;
            monsNoNight.DataSource = pokemonSource;
            monsNoPokeradar.DataSource = pokemonSource;
            monsNoSurf.DataSource = pokemonSource;
            monsNoOldRod.DataSource = pokemonSource;
            monsNoGoodRod.DataSource = pokemonSource;
            monsNoSuperRod.DataSource = pokemonSource;

            groundMonsDataGridView.Columns[1].ValueType = typeof(int);
            groundMonsDataGridView.Columns[2].ValueType = typeof(int);
            swarmDataGridView.Columns[1].ValueType = typeof(int);
            swarmDataGridView.Columns[2].ValueType = typeof(int);
            morningDataGridView.Columns[1].ValueType = typeof(int);
            morningDataGridView.Columns[2].ValueType = typeof(int);
            dayDataGridView.Columns[1].ValueType = typeof(int);
            dayDataGridView.Columns[2].ValueType = typeof(int);
            nightDataGridView.Columns[1].ValueType = typeof(int);
            nightDataGridView.Columns[2].ValueType = typeof(int);
            pokeradarDataGridView.Columns[1].ValueType = typeof(int);
            pokeradarDataGridView.Columns[2].ValueType = typeof(int);
            waterDataGridView.Columns[1].ValueType = typeof(int);
            waterDataGridView.Columns[2].ValueType = typeof(int);
            oldRodDataGridView6.Columns[1].ValueType = typeof(int);
            oldRodDataGridView6.Columns[2].ValueType = typeof(int);
            goodRodDataGridView7.Columns[1].ValueType = typeof(int);
            goodRodDataGridView7.Columns[2].ValueType = typeof(int);
            superRodDataGridView8.Columns[1].ValueType = typeof(int);
            superRodDataGridView8.Columns[2].ValueType = typeof(int);

            //encounterTables[0].groundMons[0].dexID = ushort.MaxValue + 1;
            // TODO: Handle extra features in plugins?
            //uint16DexID = gameData.Uint16EncounterTables();
            uint16DexID = true;
            if (uint16DexID)
            {
                DataGridViewTextBoxColumn dgvtbc1 = new();
                DataGridViewTextBoxColumn fcSwarm = new();
                DataGridViewTextBoxColumn fcMorning = new();
                DataGridViewTextBoxColumn fcDay = new();
                DataGridViewTextBoxColumn fcNight = new();
                DataGridViewTextBoxColumn dgvtbc4 = new();
                DataGridViewTextBoxColumn dgvtbc5 = new();
                DataGridViewTextBoxColumn dgvtbc6 = new();
                DataGridViewTextBoxColumn dgvtbc7 = new();
                DataGridViewTextBoxColumn dgvtbc8 = new();
                string formIDColumnName = "FormID";
                dgvtbc1.Name = formIDColumnName;
                fcSwarm.Name = formIDColumnName;
                fcMorning.Name = formIDColumnName;
                fcDay.Name = formIDColumnName;
                fcNight.Name = formIDColumnName;
                dgvtbc4.Name = formIDColumnName;
                dgvtbc5.Name = formIDColumnName;
                dgvtbc6.Name = formIDColumnName;
                dgvtbc7.Name = formIDColumnName;
                dgvtbc8.Name = formIDColumnName;
                dgvtbc1.ValueType = typeof(ushort);
                fcSwarm.ValueType = typeof(ushort);
                fcMorning.ValueType = typeof(ushort);
                fcDay.ValueType = typeof(ushort);
                fcNight.ValueType = typeof(ushort);
                dgvtbc4.ValueType = typeof(ushort);
                dgvtbc5.ValueType = typeof(ushort);
                dgvtbc6.ValueType = typeof(ushort);
                dgvtbc7.ValueType = typeof(ushort);
                dgvtbc8.ValueType = typeof(ushort);
                groundMonsDataGridView.Columns.Add(dgvtbc1);
                swarmDataGridView.Columns.Add(fcSwarm);
                morningDataGridView.Columns.Add(fcMorning);
                dayDataGridView.Columns.Add(fcDay);
                nightDataGridView.Columns.Add(fcNight);
                pokeradarDataGridView.Columns.Add(dgvtbc4);
                waterDataGridView.Columns.Add(dgvtbc5);
                oldRodDataGridView6.Columns.Add(dgvtbc6);
                goodRodDataGridView7.Columns.Add(dgvtbc7);
                superRodDataGridView8.Columns.Add(dgvtbc8);
            }

            encounterTable = encounterTables[0];

            groundMonsDataGridView.Rows.Add(encounterTable.ground_mons.Count - 2);
            swarmDataGridView.Rows.Add(encounterTable.tairyo.Count);
            morningDataGridView.Rows.Add(2);
            dayDataGridView.Rows.Add(encounterTable.day.Count);
            nightDataGridView.Rows.Add(encounterTable.night.Count);
            pokeradarDataGridView.Rows.Add(encounterTable.swayGrass.Count);
            waterDataGridView.Rows.Add(encounterTable.water_mons.Count);
            oldRodDataGridView6.Rows.Add(encounterTable.boro_mons.Count);
            goodRodDataGridView7.Rows.Add(encounterTable.ii_mons.Count);
            superRodDataGridView8.Rows.Add(encounterTable.sugoi_mons.Count);

            groundMonsDataGridView.DataError += DataError;
            swarmDataGridView.DataError += DataError;
            morningDataGridView.DataError += DataError;
            dayDataGridView.DataError += DataError;
            nightDataGridView.DataError += DataError;
            pokeradarDataGridView.DataError += DataError;
            waterDataGridView.DataError += DataError;
            oldRodDataGridView6.DataError += DataError;
            goodRodDataGridView7.DataError += DataError;
            superRodDataGridView8.DataError += DataError;

            RefreshDisplay();

            ActivateControls();
        }

        private void RefreshGroundMonsDisplay()
        {
            // Ground Mons
            for (int i = 0; i < encounterTable.ground_mons.Count - 2; i++)
            {
                int index = i < 2 ? i : i + 2;
                var encounter = encounterTable.ground_mons[index];
                DataGridViewRow iRow = groundMonsDataGridView.Rows[i];
                iRow.Cells[0].Value = pokemon[(ushort)encounter.monsNo];
                iRow.Cells[1].Value = encounter.minlv;
                iRow.Cells[2].Value = encounter.maxlv;
                iRow.Cells[3].Value = GetGroundRate(i);
                if (uint16DexID)
                    iRow.Cells[4].Value = (ushort)(encounter.monsNo >> 16);
            }
        }

        private void RefreshDisplay()
        {
            RefreshGroundMonsDisplay();

            // Swarm
            for (int i = 0; i < encounterTable.tairyo.Count; i++)
            {
                var encounter = encounterTable.tairyo[i];
                DataGridViewRow iRow = swarmDataGridView.Rows[i];
                iRow.Cells[0].Value = pokemon[(ushort)encounter.monsNo];
                iRow.Cells[1].Value = encounter.minlv;
                iRow.Cells[2].Value = encounter.maxlv;
                iRow.Cells[3].Value = GetSwarmRate(i);
                if (uint16DexID)
                    iRow.Cells[4].Value = (ushort)(encounter.monsNo >> 16);
            }

            // Morning
            for (int i = 0; i < 2; i++)
            {
                int index = i + 2;
                var encounter = encounterTable.ground_mons[index];
                DataGridViewRow iRow = morningDataGridView.Rows[i];
                iRow.Cells[0].Value = pokemon[(ushort)encounter.monsNo];
                iRow.Cells[1].Value = encounter.minlv;
                iRow.Cells[2].Value = encounter.maxlv;
                iRow.Cells[3].Value = GetTimeRate(i);
                if (uint16DexID)
                    iRow.Cells[4].Value = (ushort)(encounter.monsNo >> 16);
            }

            // Day
            for (int i = 0; i < encounterTable.day.Count; i++)
            {
                var encounter = encounterTable.day[i];
                DataGridViewRow iRow = dayDataGridView.Rows[i];
                iRow.Cells[0].Value = pokemon[(ushort)encounter.monsNo];
                iRow.Cells[1].Value = encounter.minlv;
                iRow.Cells[2].Value = encounter.maxlv;
                iRow.Cells[3].Value = GetTimeRate(i);
                if (uint16DexID)
                    iRow.Cells[4].Value = (ushort)(encounter.monsNo >> 16);
            }

            // Night
            for (int i = 0; i < encounterTable.night.Count; i++)
            {
                var encounter = encounterTable.night[i];
                DataGridViewRow iRow = nightDataGridView.Rows[i];
                iRow.Cells[0].Value = pokemon[(ushort)encounter.monsNo];
                iRow.Cells[1].Value = encounter.minlv;
                iRow.Cells[2].Value = encounter.maxlv;
                iRow.Cells[3].Value = GetTimeRate(i);
                if (uint16DexID)
                    iRow.Cells[4].Value = (ushort)(encounter.monsNo >> 16);
            }

            // Pokeradar Mons
            for (int i = 0; i < encounterTable.swayGrass.Count; i++)
            {
                var encounter = encounterTable.swayGrass[i];
                DataGridViewRow iRow = pokeradarDataGridView.Rows[i];
                iRow.Cells[0].Value = pokemon[(ushort)encounter.monsNo];
                iRow.Cells[1].Value = encounter.minlv;
                iRow.Cells[2].Value = encounter.maxlv;
                iRow.Cells[3].Value = GetPokeradarRate(i);
                if (uint16DexID)
                    iRow.Cells[4].Value = (ushort)(encounter.monsNo >> 16);
            }

            // Water Mons
            for (int i = 0; i < encounterTable.water_mons.Count; i++)
            {
                var encounter = encounterTable.water_mons[i];
                DataGridViewRow iRow = waterDataGridView.Rows[i];
                iRow.Cells[0].Value = pokemon[(ushort)encounter.monsNo];
                iRow.Cells[1].Value = encounter.minlv;
                iRow.Cells[2].Value = encounter.maxlv;
                iRow.Cells[3].Value = GetWaterRate(i);
                if (uint16DexID)
                    iRow.Cells[4].Value = (ushort)(encounter.monsNo >> 16);
            }

            // Old Rod
            for (int i = 0; i < encounterTable.boro_mons.Count; i++)
            {
                var encounter = encounterTable.boro_mons[i];
                DataGridViewRow iRow = oldRodDataGridView6.Rows[i];
                iRow.Cells[0].Value = pokemon[(ushort)encounter.monsNo];
                iRow.Cells[1].Value = encounter.minlv;
                iRow.Cells[2].Value = encounter.maxlv;
                iRow.Cells[3].Value = GetWaterRate(i);
                if (uint16DexID)
                    iRow.Cells[4].Value = (ushort)(encounter.monsNo >> 16);
            }
            
            // Good Rod
            for (int i = 0; i < encounterTable.ii_mons.Count; i++)
            {
                var encounter = encounterTable.ii_mons[i];
                DataGridViewRow iRow = goodRodDataGridView7.Rows[i];
                iRow.Cells[0].Value = pokemon[(ushort)encounter.monsNo];
                iRow.Cells[1].Value = encounter.minlv;
                iRow.Cells[2].Value = encounter.maxlv;
                iRow.Cells[3].Value = GetWaterRate(i);
                if (uint16DexID)
                    iRow.Cells[4].Value = (ushort)(encounter.monsNo >> 16);
            }

            // Super Rod
            for (int i = 0; i < encounterTable.sugoi_mons.Count; i++)
            {
                var encounter = encounterTable.sugoi_mons[i];
                DataGridViewRow iRow = superRodDataGridView8.Rows[i];
                iRow.Cells[0].Value = pokemon[(ushort)encounter.monsNo];
                iRow.Cells[1].Value = encounter.minlv;
                iRow.Cells[2].Value = encounter.maxlv;
                iRow.Cells[3].Value = GetWaterRate(i);
                if (uint16DexID)
                    iRow.Cells[4].Value = (ushort)(encounter.monsNo >> 16);
            }

            encRateGround.Value = encounterTable.encRate_gr;
            encRateWater.Value = encounterTable.encRate_wat;
            encRateOldRod.Value = encounterTable.encRate_turi_boro;
            encRateGoodRod.Value = encounterTable.encRate_turi_ii;
            encRateSuperRod.Value = encounterTable.encRate_sugoi;

            // TODO: Adjust these to actually use the full array
            formProbNumericUpDown.Value = encounterTable.FormProb[0];
            unownTableNumericUpDown.Value = encounterTable.AnnoonTable[0];

            gbaeef?.ZoneChanged();
        }

        private void CommitEdit(object sender, EventArgs e)
        {
            CommitGroundAndMorning();

            List<FieldEncountTable.Sheettable.MonsLv> swarm = new();
            List<FieldEncountTable.Sheettable.MonsLv> swayGrass = new();
            List<FieldEncountTable.Sheettable.MonsLv> day = new();
            List<FieldEncountTable.Sheettable.MonsLv> night = new();
            List<FieldEncountTable.Sheettable.MonsLv> waterMons = new();
            List<FieldEncountTable.Sheettable.MonsLv> oldRodMons = new();
            List<FieldEncountTable.Sheettable.MonsLv> goodRodMons = new();
            List<FieldEncountTable.Sheettable.MonsLv> superRodMons = new();

            // Swarm
            for (int i = 0; i < encounterTable.tairyo.Count; i++)
            {
                DataGridViewRow iRow = swarmDataGridView.Rows[i];
                var enc = new FieldEncountTable.Sheettable.MonsLv();
                enc.monsNo = pokemon.IndexOf((string)iRow.Cells[0].Value);
                enc.minlv = (int)iRow.Cells[1].Value;
                enc.maxlv = (int)iRow.Cells[2].Value;
                if (uint16DexID)
                    enc.monsNo += (ushort)iRow.Cells[4].Value << 16;
                swarm.Add(enc);
            }

            // Day
            for (int i = 0; i < encounterTable.day.Count; i++)
            {
                DataGridViewRow iRow = dayDataGridView.Rows[i];
                var enc = new FieldEncountTable.Sheettable.MonsLv();
                enc.monsNo = pokemon.IndexOf((string)iRow.Cells[0].Value);
                enc.minlv = (int)iRow.Cells[1].Value;
                enc.maxlv = (int)iRow.Cells[2].Value;
                if (uint16DexID)
                    enc.monsNo += (ushort)iRow.Cells[4].Value << 16;
                day.Add(enc);
            }

            // Night
            for (int i = 0; i < encounterTable.night.Count; i++)
            {
                DataGridViewRow iRow = nightDataGridView.Rows[i];
                var enc = new FieldEncountTable.Sheettable.MonsLv();
                enc.monsNo = pokemon.IndexOf((string)iRow.Cells[0].Value);
                enc.minlv = (int)iRow.Cells[1].Value;
                enc.maxlv = (int)iRow.Cells[2].Value;
                if (uint16DexID)
                    enc.monsNo += (ushort)iRow.Cells[4].Value << 16;
                night.Add(enc);
            }

            // Pokeradar Mons
            for (int i = 0; i < encounterTable.swayGrass.Count; i++)
            {
                DataGridViewRow iRow = pokeradarDataGridView.Rows[i];
                var swayGrassEnc = new FieldEncountTable.Sheettable.MonsLv();
                swayGrassEnc.monsNo = pokemon.IndexOf((string)iRow.Cells[0].Value);
                swayGrassEnc.minlv = (int)iRow.Cells[1].Value;
                swayGrassEnc.maxlv = (int)iRow.Cells[2].Value;
                if (uint16DexID)
                    swayGrassEnc.monsNo += (ushort)iRow.Cells[4].Value << 16;
                swayGrass.Add(swayGrassEnc);
            }

            // Water Mons
            for (int i = 0; i < encounterTable.water_mons.Count; i++)
            {
                DataGridViewRow iRow = waterDataGridView.Rows[i];
                var waterMon = new FieldEncountTable.Sheettable.MonsLv();
                waterMon.monsNo = pokemon.IndexOf((string)iRow.Cells[0].Value);
                waterMon.minlv = (int)iRow.Cells[1].Value;
                waterMon.maxlv = (int)iRow.Cells[2].Value;
                if (uint16DexID)
                    waterMon.monsNo += (ushort)iRow.Cells[4].Value << 16;
                waterMons.Add(waterMon);
            }

            // Old Rod
            for (int i = 0; i < encounterTable.boro_mons.Count; i++)
            {
                DataGridViewRow iRow = oldRodDataGridView6.Rows[i];
                var oldRodMon = new FieldEncountTable.Sheettable.MonsLv();
                oldRodMon.monsNo = pokemon.IndexOf((string)iRow.Cells[0].Value);
                oldRodMon.minlv = (int)iRow.Cells[1].Value;
                oldRodMon.maxlv = (int)iRow.Cells[2].Value;
                if (uint16DexID)
                    oldRodMon.monsNo += (ushort)iRow.Cells[4].Value << 16;
                oldRodMons.Add(oldRodMon);
            }

            // Good Rod
            for (int i = 0; i < encounterTable.ii_mons.Count; i++)
            {
                DataGridViewRow iRow = goodRodDataGridView7.Rows[i];
                var goodRodMon = new FieldEncountTable.Sheettable.MonsLv();
                goodRodMon.monsNo = pokemon.IndexOf((string)iRow.Cells[0].Value);
                goodRodMon.minlv = (int)iRow.Cells[1].Value;
                goodRodMon.maxlv = (int)iRow.Cells[2].Value;
                if (uint16DexID)
                    goodRodMon.monsNo += (ushort)iRow.Cells[4].Value << 16;
                goodRodMons.Add(goodRodMon);
            }

            // Super Rod
            for (int i = 0; i < encounterTable.sugoi_mons.Count; i++)
            {
                DataGridViewRow iRow = superRodDataGridView8.Rows[i];
                var superRodMon = new FieldEncountTable.Sheettable.MonsLv();
                superRodMon.monsNo = pokemon.IndexOf((string)iRow.Cells[0].Value);
                superRodMon.minlv = (int)iRow.Cells[1].Value;
                superRodMon.maxlv = (int)iRow.Cells[2].Value;
                if (uint16DexID)
                    superRodMon.monsNo += (ushort)iRow.Cells[4].Value << 16;
                superRodMons.Add(superRodMon);
            }

            encounterTable.tairyo = swarm;
            encounterTable.day = day;
            encounterTable.night = night;
            encounterTable.swayGrass = swayGrass;
            encounterTable.water_mons = waterMons;
            encounterTable.boro_mons = oldRodMons;
            encounterTable.ii_mons = goodRodMons;
            encounterTable.sugoi_mons = superRodMons;

            encounterTable.encRate_gr = (int) encRateGround.Value;
            encounterTable.encRate_wat = (int) encRateWater.Value;
            encounterTable.encRate_turi_boro = (int) encRateOldRod.Value;
            encounterTable.encRate_turi_ii = (int) encRateGoodRod.Value;
            encounterTable.encRate_sugoi = (int) encRateSuperRod.Value;

            // TODO: Actually use the full array
            encounterTable.FormProb[0] = (int)formProbNumericUpDown.Value;
            encounterTable.AnnoonTable[0] = (int)unownTableNumericUpDown.Value;
        }

        private void CommitGroundAndMorning()
        {
            var groundMons = new FieldEncountTable.Sheettable.MonsLv[12];
            for (int i = 0; i < encounterTable.ground_mons.Count - 2; i++)
            {
                int index = i < 2 ? i : i + 2;
                DataGridViewRow iRow = groundMonsDataGridView.Rows[i];
                var groundMon = new FieldEncountTable.Sheettable.MonsLv();
                groundMon.monsNo = pokemon.IndexOf((string)iRow.Cells[0].Value);
                groundMon.minlv = (int)iRow.Cells[1].Value;
                groundMon.maxlv = (int)iRow.Cells[2].Value;
                if (uint16DexID)
                    groundMon.monsNo += (ushort)iRow.Cells[4].Value << 16;
                groundMons[index] = groundMon;
            }

            for (int i = 0; i < 2; i++)
            {
                int index = i + 2;

                
                DataGridViewRow iRow = morningDataGridView.Rows[i];

                var morningEnc = new FieldEncountTable.Sheettable.MonsLv();
                morningEnc.monsNo = pokemon.IndexOf((string)iRow.Cells[0].Value);
                morningEnc.minlv = (int)iRow.Cells[1].Value;
                morningEnc.maxlv = (int)iRow.Cells[2].Value;
                if (uint16DexID)
                    morningEnc.monsNo += (ushort)iRow.Cells[4].Value << 16;

                groundMons[index] = morningEnc;
            }

            encounterTable.ground_mons = groundMons.ToList();
        }

        private void ZoneIDChanged(object sender, EventArgs e)
        {
            DeactivateControls();

            encounterTable = encounterTables[zoneIDListBox.SelectedIndex];
            RefreshDisplay();

            ActivateControls();
        }

        private void VersionChanged(object sender, EventArgs e)
        {
            DeactivateControls();

            encounterTables = gameData.encounterTableFiles[versionComboBox.SelectedIndex].table;
            encounterTable = encounterTables[zoneIDListBox.SelectedIndex];

            RefreshDisplay();

            ActivateControls();
        }

        private void SortChanged(object sender, EventArgs e)
        {
            DeactivateControls();

            encounterTables.Sort(sortComparisons[sortComboBox.SelectedIndex]);
            PopulateListBox();
            zoneIDListBox.SelectedIndex = encounterTables.IndexOf(encounterTable);

            ActivateControls();
        }

        private void ActivateControls()
        {
            sortComboBox.SelectedIndexChanged += SortChanged;
            zoneIDListBox.SelectedIndexChanged += ZoneIDChanged;
            versionComboBox.SelectedIndexChanged += VersionChanged;

            encRateGround.ValueChanged += CommitEdit;
            encRateWater.ValueChanged += CommitEdit;
            encRateOldRod.ValueChanged += CommitEdit;
            encRateGoodRod.ValueChanged += CommitEdit;
            encRateSuperRod.ValueChanged += CommitEdit;

            formProbNumericUpDown.ValueChanged += CommitEdit;
            unownTableNumericUpDown.ValueChanged += CommitEdit;

            groundMonsDataGridView.CellEndEdit += CommitEdit;
            swarmDataGridView.CellEndEdit += CommitEdit;
            morningDataGridView.CellEndEdit += CommitEdit;
            dayDataGridView.CellEndEdit += CommitEdit;
            nightDataGridView.CellEndEdit += CommitEdit;
            pokeradarDataGridView.CellEndEdit += CommitEdit;
            waterDataGridView.CellEndEdit += CommitEdit;
            oldRodDataGridView6.CellEndEdit += CommitEdit;
            goodRodDataGridView7.CellEndEdit += CommitEdit;
            superRodDataGridView8.CellEndEdit += CommitEdit;
        }

        private void DeactivateControls()
        {
            sortComboBox.SelectedIndexChanged -= SortChanged;
            zoneIDListBox.SelectedIndexChanged -= ZoneIDChanged;
            versionComboBox.SelectedIndexChanged -= VersionChanged;

            encRateGround.ValueChanged -= CommitEdit;
            encRateWater.ValueChanged -= CommitEdit;
            encRateOldRod.ValueChanged -= CommitEdit;
            encRateGoodRod.ValueChanged -= CommitEdit;
            encRateSuperRod.ValueChanged -= CommitEdit;

            formProbNumericUpDown.ValueChanged -= CommitEdit;
            unownTableNumericUpDown.ValueChanged -= CommitEdit;

            groundMonsDataGridView.CellEndEdit -= CommitEdit;
            swarmDataGridView.CellEndEdit -= CommitEdit;
            morningDataGridView.CellEndEdit -= CommitEdit;
            dayDataGridView.CellEndEdit -= CommitEdit;
            nightDataGridView.CellEndEdit -= CommitEdit;
            pokeradarDataGridView.CellEndEdit -= CommitEdit;
            waterDataGridView.CellEndEdit -= CommitEdit;
            oldRodDataGridView6.CellEndEdit -= CommitEdit;
            goodRodDataGridView7.CellEndEdit -= CommitEdit;
            superRodDataGridView8.CellEndEdit -= CommitEdit;
        }

        private void PopulateListBox()
        {
            int index = zoneIDListBox.SelectedIndex;
            if (index < 0)
                index = 0;
            zoneIDListBox.DataSource = encounterTables.Select(e => (int)e.zoneID + " - " + GetZoneName((int)e.zoneID)).ToList();
            zoneIDListBox.SelectedIndex = index;
        }

        private void DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MainForm.ShowDataError();
        }

        private void OpenGBAEncounterEditor(object sender, EventArgs e)
        {
            gbaeef ??= new(this);
            gbaeef.Show();
        }

        private string GetGroundRate(int index)
        {
            if (index >= groundRates.Length)
                return "n/a";
            return groundRates[index];
        }

        private string GetSwarmRate(int index)
        {
            if (index >= swarmRates.Length)
                return "n/a";
            return swarmRates[index];
        }

        private string GetTimeRate(int index)
        {
            if (index >= timeRates.Length)
                return "n/a";
            return timeRates[index];
        }

        private string GetPokeradarRate(int index)
        {
            if (index >= pokeradarRates.Length)
                return "n/a";
            return pokeradarRates[index];
        }

        private string GetWaterRate(int index)
        {
            if (index >= waterRates.Length)
                return "n/a";
            return waterRates[index];
        }
    }
}
