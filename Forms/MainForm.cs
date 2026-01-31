using System;
using System.Threading;
using System.Windows.Forms;
using static ImpostersOrdeal.Distributions;

namespace ImpostersOrdeal
{
    /// <summary>
    ///  Root form from which the rest of the application is accessible.
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
        ///  Confirms with user to cancel loading dump.
        /// </summary>
        private static DialogResult RetryLoadDumpDialog()
        {
            return MessageBox.Show("Game dump is required to continue. Load dump?",
                "Load Cancelled", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        ///  Initializes controls using the specified config.
        /// </summary>
        private void SetupConfig(RandomizerSetupConfig rsc)
        {
            this.rsc = rsc;

            button2.Initialize(rsc.evolutionDestinationPokemon);
            groupBox1.Initialize(rsc.evolutionLevel);
            numericDistributionControl1.Initialize(rsc.baseStats);
            itemDistributionControl1.Initialize(rsc.pokemonTyping);
            numericUpDown1.Value = (decimal)rsc.doubleTypingP;
            numericUpDown2.Value = (decimal)rsc.tmCompatibilityP;
            numericUpDown3.Value = (decimal)rsc.tmCompatibilityTypeBiasP;
            itemDistributionControl2.Initialize(rsc.wildHeldItems);
            itemDistributionControl3.Initialize(rsc.growthRate);
            itemDistributionControl4.Initialize(rsc.abilities);
            numericDistributionControl2.Initialize(rsc.catchRate);
            numericDistributionControl3.Initialize(rsc.evYields);
            numericDistributionControl4.Initialize(rsc.initialFriendship);
            numericDistributionControl5.Initialize(rsc.expYield);
            itemDistributionControl7.Initialize(rsc.eggMoves);
            numericUpDown5.Value = (decimal)rsc.eggMoveTypeBiasP;
            numericDistributionControl9.Initialize(rsc.eggMoveCount);
            itemDistributionControl6.Initialize(rsc.levelUpMoves);
            numericUpDown4.Value = (decimal)rsc.levelUpMoveTypeBiasP;
            numericDistributionControl7.Initialize(rsc.levelUpMoveLevels);
            numericDistributionControl6.Initialize(rsc.levelUpMoveCount);
            itemDistributionControl8.Initialize(rsc.moveTyping);
            itemDistributionControl9.Initialize(rsc.damageCategory);
            itemDistributionControl18.Initialize(rsc.tmMoves);
            numericDistributionControl8.Initialize(rsc.movePower);
            numericDistributionControl10.Initialize(rsc.moveAccuracy);
            numericDistributionControl11.Initialize(rsc.movePp);
            numericDistributionControl18.Initialize(rsc.itemPrices);
            itemDistributionControl16.Initialize(rsc.pickupItems);
            itemDistributionControl19.Initialize(rsc.shopItems);
            itemDistributionControl10.Initialize(rsc.wildPokemon);
            numericDistributionControl12.Initialize(rsc.wildPokemonLevels);
            itemDistributionControl11.Initialize(rsc.trainerItems);
            numericDistributionControl13.Initialize(rsc.trainerItemCount);
            itemDistributionControl12.Initialize(rsc.trainerPokemonSpecies);
            itemDistributionControl15.Initialize(rsc.trainerPokemonHeldItems);
            itemDistributionControl13.Initialize(rsc.trainerPokemonNatures);
            numericUpDown7.Value = (decimal)rsc.trainerPokemonMoveTypeBiasP;
            itemDistributionControl14.Initialize(rsc.trainerPokemonMoves);
            numericUpDown6.Value = (decimal)rsc.trainerPokemonShinyP;
            itemDistributionControl17.Initialize(rsc.trainerPokemonAbilities);
            numericDistributionControl14.Initialize(rsc.trainerPokemonCount);
            numericDistributionControl15.Initialize(rsc.trainerPokemonLevels);
            numericDistributionControl16.Initialize(rsc.trainerPokemonIvs);
            numericDistributionControl17.Initialize(rsc.trainerPokemonEvs);
            itemDistributionControl5.Initialize(rsc.typeMatchups);
            itemDistributionControl20.Initialize(rsc.scriptedPokemon);
            itemDistributionControl21.Initialize(rsc.scriptedItems);
            numericUpDown8.Value = (decimal)rsc.levelCoefficient;

            comboBox1.SelectedIndex = 0;
        }

        /// <summary>
        ///  Starts up the LoadingForm.
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
            SetupConfig(controller.GetSetupConfig());
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
            SetupConfig(controller.GetSetupConfig());
            loadingForm.Finish();
        }

        private void Randomize(object sender, EventArgs e)
        {
            //Notify if already randomized.
            /*if (randomizeClicked && MessageBox.Show("You uh... You already made me randomize the files, and I\n" +
                "wouldn't really recommend doing it multiple times...\n" +
                "Randomize again anyway?",
                   "Again?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            loadingForm = new("Makin' a mess...", flavor.GetThought());
            loadingDisplay = new(StartLoadingDisplay);
            loadingDisplay.Start();
            Thread.Sleep(100);
            randomizer.Randomize();
            loadingForm.Finish();

            randomizeClicked = true;
            MessageBox.Show(
                "Randomization complete!",
                  "All done!", MessageBoxButtons.OK, MessageBoxIcon.Information);*/
        }

        private void Export(object sender, EventArgs e)
        {
            //Notify if not randomized.
            if (!randomizeClicked && MessageBox.Show("I haven't randomized anything yet...\n" +
                "Just thought I'd mention it in case you forgot.\n" +
                "Proceed anyway? I'll still export any changed files for you.",
                   "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
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
