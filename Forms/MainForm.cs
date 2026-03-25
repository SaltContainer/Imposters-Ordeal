using System;
using System.Threading;
using System.Windows.Forms;
using static ImpostersOrdeal.Distributions;

namespace ImpostersOrdeal
{
    /// <summary>
    /// Root form from which the rest of the application is accessible.
    /// </summary>
    public partial class MainForm : Form
    {
        // UI stuff
        private Thread loadingDisplay;
        private LoadingForm loadingForm;

        private Controller controller;

        private RandomizerSetupConfig rsc;

        private bool randomizeClicked = false;

        public MainForm()
        {
            InitializeComponent();

            controller = new Controller();
        }

        /// <summary>
        /// Confirms with user to cancel loading dump.
        /// </summary>
        private static DialogResult RetryLoadDumpDialog()
        {
            return MessageBox.Show("Game dump is required to continue. Load dump?",
                "Load Cancelled", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Initializes controls using the specified config.
        /// </summary>
        private void SetupConfig(DistributionsSetupConfig dsc, RandomizerSetupConfig rsc)
        {
            if (rsc != null)
                this.rsc = rsc;

            button2.Initialize(dsc.Pokemon.EvolutionDestinationPokemonDists);
            groupBox1.Initialize(dsc.Pokemon.EvolutionLevelDists);
            numericDistributionControl1.Initialize(dsc.Pokemon.BaseStatsDists);
            itemDistributionControl1.Initialize(dsc.Pokemon.PokemonTypingDists);
            numericUpDown1.Value = (decimal)dsc.Pokemon.DoubleTypingP;
            numericUpDown2.Value = (decimal)dsc.Pokemon.TMCompatibilityP;
            numericUpDown3.Value = (decimal)dsc.Pokemon.TMCompatibilityTypeBiasP;
            itemDistributionControl2.Initialize(dsc.Pokemon.WildHeldItemsDists);
            itemDistributionControl3.Initialize(dsc.Pokemon.GrowthRateDists);
            itemDistributionControl4.Initialize(dsc.Pokemon.AbilitiesDists);
            numericDistributionControl2.Initialize(dsc.Pokemon.CatchRateDists);
            numericDistributionControl3.Initialize(dsc.Pokemon.EVYieldDists);
            numericDistributionControl4.Initialize(dsc.Pokemon.InitialFriendshipDists);
            numericDistributionControl5.Initialize(dsc.Pokemon.ExpYieldDists);
            itemDistributionControl7.Initialize(dsc.Pokemon.EggMovesDists);
            numericUpDown5.Value = (decimal)dsc.Pokemon.EggMoveTypeBiasP;
            numericDistributionControl9.Initialize(dsc.Pokemon.EggMovesCountDists);
            itemDistributionControl6.Initialize(dsc.Pokemon.LevelUpMovesDists);
            numericUpDown4.Value = (decimal)dsc.Pokemon.LevelUpMovesTypeBiasP;
            numericDistributionControl7.Initialize(dsc.Pokemon.LevelUpMovesLevelDists);
            numericDistributionControl6.Initialize(dsc.Pokemon.LevelUpMovesCountDists);

            itemDistributionControl8.Initialize(dsc.MovesAndItems.MoveTypingDists);
            itemDistributionControl9.Initialize(dsc.MovesAndItems.DamageCategoryDists);
            itemDistributionControl18.Initialize(dsc.MovesAndItems.TMMovesDists);
            numericDistributionControl8.Initialize(dsc.MovesAndItems.MovePowerDists);
            numericDistributionControl10.Initialize(dsc.MovesAndItems.MoveAccuracyDists);
            numericDistributionControl11.Initialize(dsc.MovesAndItems.MovePPDists);
            numericDistributionControl18.Initialize(dsc.MovesAndItems.ItemPricesDists);
            itemDistributionControl16.Initialize(dsc.MovesAndItems.PickupItemsDists);
            itemDistributionControl19.Initialize(dsc.MovesAndItems.ShopItemsDists);

            itemDistributionControl10.Initialize(dsc.Encounters.WildEncountersWildPokemonDists);
            numericDistributionControl12.Initialize(dsc.Encounters.WildEncountersWildPokemonLevelsDists);
            itemDistributionControl11.Initialize(dsc.Encounters.TrainerItemsDists);
            numericDistributionControl13.Initialize(dsc.Encounters.TrainerItemCountDists);
            itemDistributionControl12.Initialize(dsc.Encounters.TrainerSpeciesDists);
            itemDistributionControl15.Initialize(dsc.Encounters.TrainerHeldItemsDists);
            itemDistributionControl13.Initialize(dsc.Encounters.TrainerNaturesDists);
            numericUpDown7.Value = (decimal)dsc.Encounters.TrainerMoveTypeBiasP;
            itemDistributionControl14.Initialize(dsc.Encounters.TrainerMovesDists);
            numericUpDown6.Value = (decimal)dsc.Encounters.TrainerShinyP;
            itemDistributionControl17.Initialize(dsc.Encounters.TrainerAbilitiesDists);
            numericDistributionControl14.Initialize(dsc.Encounters.TrainerPokemonCountDists);
            numericDistributionControl15.Initialize(dsc.Encounters.TrainerLevelsDists);
            numericDistributionControl16.Initialize(dsc.Encounters.TrainerIVsDists);
            numericDistributionControl17.Initialize(dsc.Encounters.TrainerEVsDists);

            itemDistributionControl5.Initialize(dsc.Misc.TypeMatchupsDists);
            itemDistributionControl20.Initialize(dsc.Misc.RandomScriptedPokemonDists);
            itemDistributionControl21.Initialize(dsc.Misc.RandomScriptedItemsDists);

            this.rsc.Pokemon.PokemonTypingEvoLogicCorrelationDist = dsc.Pokemon.PokemonTypingEvoLogicCorrelationDist;
            this.rsc.Pokemon.EvoMovesCountDist = dsc.Pokemon.EvoMovesCountDist;

            comboBox1.SelectedIndex = 0;

            if (rsc != null)
                SetupFormFromConfig();
        }

        private void SetupFormFromConfig()
        {
            // TODO: Update form from config
        }

        private void UpdateConfigFromForm()
        {
            // TODO: Update config from form
        }

        /// <summary>
        /// Starts up the LoadingForm.
        /// </summary>
        private void StartLoadingDisplay()
        {
            loadingForm.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //Check if valid dump path already is in config
            if (!controller.InitializeFromConfig())
            {
                //Confirm with user to get dump path. Abort if cancel.
                if (MessageBox.Show("Alright, to start out, could ya get me a dump of the game real quick?\n" +
                    "Gimme the folder that's got the \"romfs\" in it.",
                    "Load Dump", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel && RetryLoadDumpDialog() == DialogResult.No)
                {
                    this.Close();
                    return;
                }

                //Load dump
                while (!controller.InitializeFromInput())
                    if (RetryLoadDumpDialog() == DialogResult.No)
                    {
                        this.Close();
                        return;
                    }
            }

            loadingForm = new("Ferociously investigating your dump...", controller.GetFlavorSubTask());
            loadingDisplay = new(StartLoadingDisplay);
            loadingDisplay.Start();

            //Sometimes UpdateSubTask would be called before the loadingForm was done setting up, causing me great grief.
            //This is probably not a very good solution to that, but it works! ¯\_(ツ)_/¯
            Thread.Sleep(100);

            controller.ParseAllData();

            loadingForm.UpdateSubTask(controller.GetFlavorSubTask());
            SetupConfig(controller.GetInitialDistributionConfig(), new RandomizerSetupConfig());
            loadingForm.Finish();

            absoluteBoundaryDataGridView.DataSource = controller.GetAbsoluteBoundariesTable();
            foreach (DataGridViewColumn c in absoluteBoundaryDataGridView.Columns)
            {
                if (c.Name == "Value")
                    c.FillWeight = 300;
            }
        }

        private void OpenNumericDistributionForm(object sender, EventArgs e)
        {
            NumericDistributionControl ndc = (NumericDistributionControl)((Button)sender).Parent;
            NumericDistributionForm form = new(ndc);
            form.Show();
            ndc.UpdateTextBox();
        }

        private void NumericDistributionTextBoxChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            NumericDistributionControl ndc = (NumericDistributionControl)tb.Parent;
            IDistribution newDistribution = Parse(tb.Text);
            if (newDistribution == null)
            {
                tb.Text = ndc.Get().GetString();
                return;
            }
            ndc.idx = (int)newDistribution.GetConfig()[0];
            ndc.SetCurrent(newDistribution);
        }

        private void OpenItemDistributionForm(object sender, EventArgs e)
        {
            ItemDistributionControl idc = (ItemDistributionControl)sender;
            ItemDistributionForm form = new(idc);
            form.Show();
        }

        private void DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            ShowDataError();
        }

        public static void ShowDataError()
        {
            MessageBox.Show("Yeah, no. That's not gonna fly buster.\nInput some actual valid data please.",
                "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowParserError(string message)
        {
            MessageBox.Show(message, "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void AddMod(object sender, EventArgs e)
        {
            if (MessageBox.Show("Fancy! Let's see if we can merge in a mod, shall we?\n" +
                   "Gimme a folder that's got a \"romfs\" or \"exefs\" in it.",
                   "Add Mod", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                return;
            
            loadingForm = new("Some stuff changed...", controller.GetFlavorSubTask());
            loadingDisplay = new(StartLoadingDisplay);
            loadingDisplay.Start();

            Thread.Sleep(100);

            controller.AddMod();

            loadingForm.UpdateSubTask(controller.GetFlavorSubTask());
            SetupConfig(controller.GetInitialDistributionConfig(), null);
            loadingForm.Finish();
        }

        private void Randomize(object sender, EventArgs e)
        {
            //Notify if already randomized.
            if (randomizeClicked && MessageBox.Show("You uh... You already made me randomize the files, and I\n" +
                "wouldn't really recommend doing it multiple times...\n" +
                "Randomize again anyway?",
                   "Again?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            loadingForm = new("Makin' a mess...", controller.GetFlavorThought());
            loadingDisplay = new(StartLoadingDisplay);
            loadingDisplay.Start();

            Thread.Sleep(100);

            UpdateConfigFromForm();
            controller.Randomize(rsc);

            loadingForm.Finish();

            randomizeClicked = true;
            MessageBox.Show("Randomization complete!", "All done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Export(object sender, EventArgs e)
        {
            //Notify if not randomized.
            if (!randomizeClicked && MessageBox.Show("I haven't randomized anything yet...\n" +
                "Just thought I'd mention it in case you forgot.\n" +
                "Proceed anyway? I'll still export any changed files for you.",
                   "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                return;

            loadingForm = new("Finishing up...", controller.GetFlavorThought());
            loadingDisplay = new(StartLoadingDisplay);
            loadingDisplay.Start();

            Thread.Sleep(100);

            controller.SaveAllData();

            loadingForm.UpdateSubTask(controller.GetFlavorThought());

            controller.ExportMod();

            loadingForm.Finish();

            MessageBox.Show(
                "And that should be it! Your very own mod has\n" +
                "been created! I'll see myself out now.\n" +
                "Oh, and if you wonder where it is, I placed it right\n" +
                "alongside my executable, inside \"" + Constants.OUTPUT_FOLDER + "\".",
                  "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void OpenPokemonEditor(object sender, EventArgs e)
        {
            PokemonEditorForm pef = new(controller.GetGameData());
            pef.Show();
            controller.SetDataTypeModified(typeof(PokemonDataTable));
        }

        private void OpenMoveEditor(object sender, EventArgs e)
        {
            MoveEditorForm mef = new(controller.GetGameData());
            mef.Show();
            controller.SetDataTypeModified(typeof(MoveTable));
        }

        private void OpenTMEditor(object sender, EventArgs e)
        {
            TMEditorForm tmef = new(controller.GetGameData());
            tmef.Show();
            controller.SetDataTypeModified(typeof(ItemTable));
        }

        private void OpenItemEditor(object sender, EventArgs e)
        {
            ItemEditorForm ief = new(controller.GetGameData());
            ief.Show();
            controller.SetDataTypeModified(typeof(ItemTable));
        }

        private void OpenPickupEditor(object sender, EventArgs e)
        {
            PickupEditorForm pef = new(controller.GetGameData());
            pef.Show();
            controller.SetDataTypeModified(typeof(PickupTable));
        }

        private void OpenShopEditor(object sender, EventArgs e)
        {
            ShopEditorForm sef = new(controller.GetGameData());
            sef.Show();
            controller.SetDataTypeModified(typeof(ShopTable));
        }

        private void OpenWildEncounterEditors(object sender, EventArgs e)
        {
            WildEncounterForm wef = new(controller.GetGameData());
            wef.Show();
        }

        private void OpenTrainerEditor(object sender, EventArgs e)
        {
            TrainerEditorForm tef = new(controller.GetGameData());
            tef.Show();
            controller.SetDataTypeModified(typeof(TrainerTable));
        }

        private void OpenTypeMatchupEditor(object sender, EventArgs e)
        {
            TypeMatchupEditorForm tmef = new(controller.GetGameData());
            tmef.Show();
            controller.SetDataTypeModified(typeof(GlobalMetadata));
        }

        private void OpenGlobalMetadataEditor(object sender, EventArgs e)
        {
            GlobalMetadataEditorForm gmef = new(controller.GetGameData());
            gmef.Show();
            controller.SetDataTypeModified(typeof(GlobalMetadata));
        }

        private void OpenPokemonInserter(object sender, EventArgs e)
        {
            PokemonInserterForm pif = new(controller.GetGameData());
            pif.Show();
        }

        private void OpenJsonConverter(object sender, EventArgs e)
        {
            JsonConverterForm jcf = new();
            jcf.Show();
        }

        //Battle Tower Trainer Button
        private void Button34_Click(object sender, EventArgs e)
        {
            BattleTowerTrainerEditorForm tef = new(controller.GetGameData());
            tef.Show();
            controller.SetDataTypeModified(typeof(BattleTowerTable));
        }

        //Battle Tower Pokemon Button
        private void Button35_Click_1(object sender, EventArgs e)
        {
            BattleTowerPokemonForm tef = new(controller.GetGameData());
            tef.Show();
            controller.SetDataTypeModified(typeof(BattleTowerTable));
        }
    }
}
