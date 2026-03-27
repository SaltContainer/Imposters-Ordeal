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
            {
                this.rsc = rsc;
                SetupFormFromConfig();
            }

            btnDistPokemonEvolutionDest.Initialize(dsc.Pokemon.EvolutionDestinationPokemonDists);
            grpDistPokemonEvolutionLevel.Initialize(dsc.Pokemon.EvolutionLevelDists);
            grpDistPokemonBaseStats.Initialize(dsc.Pokemon.BaseStatsDists);
            btnDistPokemonTyping.Initialize(dsc.Pokemon.PokemonTypingDists);
            numPokemonTypingDouble.Value = (decimal)dsc.Pokemon.DoubleTypingP;
            numPokemonTMCompatPercent.Value = (decimal)dsc.Pokemon.TMCompatibilityP;
            numPokemonTMCompatTypeBias.Value = (decimal)dsc.Pokemon.TMCompatibilityTypeBiasP;
            btnDistPokemonWildHeldItems.Initialize(dsc.Pokemon.WildHeldItemsDists);
            btnDistPokemonGrowth.Initialize(dsc.Pokemon.GrowthRateDists);
            btnDistPokemonAbilities.Initialize(dsc.Pokemon.AbilitiesDists);
            grpDistPokemonMiscCatchRate.Initialize(dsc.Pokemon.CatchRateDists);
            grpDistPokemonMiscEVs.Initialize(dsc.Pokemon.EVYieldDists);
            grpDistPokemonMiscInitFriendship.Initialize(dsc.Pokemon.InitialFriendshipDists);
            grpDistPokemonMiscExp.Initialize(dsc.Pokemon.ExpYieldDists);
            btnDistPokemonEggMoves.Initialize(dsc.Pokemon.EggMovesDists);
            numPokemonEggMovesTypeBias.Value = (decimal)dsc.Pokemon.EggMoveTypeBiasP;
            grpDistPokemonEggMovesCount.Initialize(dsc.Pokemon.EggMovesCountDists);
            btnDistPokemonLevelMovesMoves.Initialize(dsc.Pokemon.LevelUpMovesDists);
            numPokemonLevelMovesTypeBias.Value = (decimal)dsc.Pokemon.LevelUpMovesTypeBiasP;
            grpDistPokemonLevelMovesLearnLevels.Initialize(dsc.Pokemon.LevelUpMovesLevelDists);
            grpDistPokemonLevelMovesCount.Initialize(dsc.Pokemon.LevelUpMovesCountDists);

            btnDistMovesAndItemsMoveTyping.Initialize(dsc.MovesAndItems.MoveTypingDists);
            btnDistMovesAndItemsMoveCategory.Initialize(dsc.MovesAndItems.DamageCategoryDists);
            btnDistMovesAndItemsTMMoves.Initialize(dsc.MovesAndItems.TMMovesDists);
            grpDistMovesAndItemsPower.Initialize(dsc.MovesAndItems.MovePowerDists);
            grpDistMovesAndItemsAccuracy.Initialize(dsc.MovesAndItems.MoveAccuracyDists);
            grpDistMovesAndItemsPP.Initialize(dsc.MovesAndItems.MovePPDists);
            grpDistMovesAndItemsItemPrices.Initialize(dsc.MovesAndItems.ItemPricesDists);
            btnDistMovesAndItemsPickup.Initialize(dsc.MovesAndItems.PickupItemsDists);
            btnDistMovesAndItemsShopItems.Initialize(dsc.MovesAndItems.ShopItemsDists);

            btnDistEncountersWild.Initialize(dsc.Encounters.WildEncountersWildPokemonDists);
            grpDistEncountersWildLevels.Initialize(dsc.Encounters.WildEncountersWildPokemonLevelsDists);
            btnDistEncountersTrainerItems.Initialize(dsc.Encounters.TrainerItemsDists);
            grpDistEncountersTrainerItemsCount.Initialize(dsc.Encounters.TrainerItemCountDists);
            btnDistEncountersTrainerSpecies.Initialize(dsc.Encounters.TrainerSpeciesDists);
            btnDistEncountersTrainerHeldItems.Initialize(dsc.Encounters.TrainerHeldItemsDists);
            btnDistEncountersTrainerNatures.Initialize(dsc.Encounters.TrainerNaturesDists);
            numEncountersTrainerMovesTypeBias.Value = (decimal)dsc.Encounters.TrainerMoveTypeBiasP;
            btnDistEncountersTrainerMoves.Initialize(dsc.Encounters.TrainerMovesDists);
            numEncountersTrainerShiny.Value = (decimal)dsc.Encounters.TrainerShinyP;
            btnDistEncountersTrainerAbilities.Initialize(dsc.Encounters.TrainerAbilitiesDists);
            grpDistEncountersTrainerPokemonCount.Initialize(dsc.Encounters.TrainerPokemonCountDists);
            grpDistEncountersTrainerLevels.Initialize(dsc.Encounters.TrainerLevelsDists);
            grpDistEncountersTrainerIVs.Initialize(dsc.Encounters.TrainerIVsDists);
            grpDistEncountersTrainerEVs.Initialize(dsc.Encounters.TrainerEVsDists);

            btnDistMiscTypeMatchups.Initialize(dsc.Misc.TypeMatchupsDists);
            btnDistMiscScriptsPokemon.Initialize(dsc.Misc.RandomScriptedPokemonDists);
            btnDistMiscScriptsItems.Initialize(dsc.Misc.RandomScriptedItemsDists);

            this.rsc.Pokemon.PokemonTypingEvoLogicCorrelationDist = dsc.Pokemon.PokemonTypingEvoLogicCorrelationDist;
            this.rsc.Pokemon.EvoMovesCountDist = dsc.Pokemon.EvoMovesCountDist;
        }

        private void SetupFormFromConfig()
        {
            // TODO: Do we set the distributions?
            SetupFormFromConfigPokemon(rsc.Pokemon);
            SetupFormFromConfigMovesAndItems(rsc.MovesAndItems);
            SetupFormFromConfigEncounters(rsc.Encounters);
            SetupFormFromConfigMisc(rsc.Misc);
        }

        private void SetupFormFromConfigPokemon(RandomizerSetupConfig.PokemonConfig config)
        {
            checkPokemonEvolutionRandomDest.Checked = config.EvolutionRandomDestinations;
            checkPokemonEvolutionBSTLogic.Checked = config.EvolutionBSTLogic;
            //btnDistPokemonEvolutionDest.SetCurrent(config.EvolutionDestinationPokemonDist);
            checkDistPokemonEvolutionLevel.Checked = config.EvolutionLevel;
            //grpDistPokemonEvolutionLevel.SetCurrent(config.EvolutionLevelDist);

            checkPokemonBaseStatsShuffle.Checked = config.BaseStatsShuffle;
            checkPokemonBaseStatsBSTLogic.Checked = config.BaseStatsBSTLogic;
            checkDistPokemonBaseStats.Checked = config.BaseStats;
            //grpDistPokemonBaseStats.SetCurrent(config.BaseStatsDist);

            checkPokemonTypingRandom.Checked = config.PokemonTyping;
            checkPokemonTypingEvoLogic.Checked = config.PokemonTypingEvoLogic;
            //btnDistPokemonTyping.SetCurrent(config.PokemonTypingDist);
            numPokemonTypingDouble.Value = (decimal)config.DoubleTypingP;

            checkPokemonTMCompatRandom.Checked = config.TMCompatibility;
            checkPokemonTMCompatEvoLogic.Checked = config.TMCompatibilityEvoLogic;
            numPokemonTMCompatPercent.Value = (decimal)config.TMCompatibilityP;
            numPokemonTMCompatTypeBias.Value = (decimal)config.TMCompatibilityTypeBiasP;

            checkPokemonWildHeldItemsRandom.Checked = config.WildHeldItems;
            //btnDistPokemonWildHeldItems.SetCurrent(config.WildHeldItemsDist);

            checkPokemonGrowthRandom.Checked = config.GrowthRate;
            //btnDistPokemonGrowth.SetCurrent(config.GrowthRateDist);

            checkPokemonAbilities.Checked = config.Abilities;
            //btnDistPokemonAbilities.SetCurrent(config.AbilitiesDist);

            checkDistPokemonMiscCatchRate.Checked = config.CatchRate;
            //grpDistPokemonMiscCatchRate.SetCurrent(config.CatchRateDist);

            checkDistPokemonMiscEVs.Checked = config.EVYield;
            //grpDistPokemonMiscEVs.SetCurrent(config.EVYieldDist);

            checkDistPokemonMiscInitFriendship.Checked = config.InitialFriendship;
            //grpDistPokemonMiscInitFriendship.SetCurrent(config.InitialFriendshipDist);

            checkDistPokemonMiscExp.Checked = config.ExpYield;
            //grpDistPokemonMiscExp.SetCurrent(config.ExpYieldDist);

            checkPokemonEggMovesRandom.Checked = config.EggMoves;
            //btnDistPokemonEggMoves.SetCurrent(config.EggMovesDist);
            numPokemonEggMovesTypeBias.Value = (decimal)config.EggMoveTypeBiasP;
            checkDistPokemonEggMovesCount.Checked = config.EggMovesCount;
            //grpDistPokemonEggMovesCount.SetCurrent(config.EggMovesCountDist);

            checkPokemonLevelMovesRandom.Checked = config.LevelUpMoves;
            checkPokemonLevelMovesSortPower.Checked = config.LevelUpMovesSortByPower;
            //btnDistPokemonLevelMovesMoves.SetCurrent(config.LevelUpMovesDist);
            numPokemonLevelMovesTypeBias.Value = (decimal)config.LevelUpMovesTypeBiasP;
            checkDistPokemonLevelMovesLearnLevels.Checked = config.LevelUpMovesLevel;
            //grpDistPokemonLevelMovesLearnLevels.SetCurrent(config.LevelUpMovesLevelDist);
            checkDistPokemonLevelMovesCount.Checked = config.LevelUpMovesCount;
            //grpDistPokemonLevelMovesCount.SetCurrent(config.LevelUpMovesCountDist);
        }

        private void SetupFormFromConfigMovesAndItems(RandomizerSetupConfig.MovesAndItemsConfig config)
        {
            checkMovesAndItemsMoveTypingRandom.Checked = config.MoveTyping;
            //btnDistMovesAndItemsMoveTyping.SetCurrent(config.MoveTypingDist);

            checkMovesAndItemsMoveCategoryRandom.Checked = config.DamageCategory;
            //btnDistMovesAndItemsMoveCategory.SetCurrent(config.DamageCategoryDist);

            checkMovesAndItemsTMMovesRandom.Checked = config.TMMoves;
            //btnDistMovesAndItemsTMMoves.SetCurrent(config.TMMovesDist);

            checkDistMovesAndItemsPower.Checked = config.MovePower;
            //grpDistMovesAndItemsPower.SetCurrent(config.MovePowerDist);

            checkDistMovesAndItemsAccuracy.Checked = config.MoveAccuracy;
            //grpDistMovesAndItemsAccuracy.SetCurrent(config.MoveAccuracyDist);

            checkDistMovesAndItemsPP.Checked = config.MovePP;
            //grpDistMovesAndItemsPP.SetCurrent(config.MovePPDist);

            checkDistMovesAndItemsItemPrices.Checked = config.ItemPrices;
            //grpDistMovesAndItemsItemPrices.SetCurrent(config.ItemPricesDist);

            checkMovesAndItemsPickupRandom.Checked = config.PickupItems;
            //btnDistMovesAndItemsPickup.SetCurrent(config.PickupItemsDist);

            checkMovesAndItemsShopItemsRandom.Checked = config.ShopItems;
            checkMovesAndItemsShopItemsKeepMart.Checked = config.ShopItemsPreserveRegularMart;
            //btnDistMovesAndItemsShopItems.SetCurrent(config.ShopItemsDist);
        }

        private void SetupFormFromConfigEncounters(RandomizerSetupConfig.EncountersConfig config)
        {
            checkEncountersWildRandom.Checked = config.WildEncountersRandomPokemon;
            checkEncountersWildHighLevelLegends.Checked = config.WildEncountersHighLevelLegends;
            checkEncountersWildEvoLogic.Checked = config.WildEncountersEvolutionLogic;
            //btnDistEncountersWild.SetCurrent(config.WildEncountersWildPokemonDist);
            checkDistEncountersWildLevels.Checked = config.WildEncountersWildPokemonLevels;
            //grpDistEncountersWildLevels.SetCurrent(config.WildEncountersWildPokemonLevelsDist);

            checkEncountersTrainerItemsRandom.Checked = config.TrainerItems;
            //btnDistEncountersTrainerItems.SetCurrent(config.TrainerItemsDist);
            checkDistEncountersTrainerItemsCount.Checked = config.TrainerItemCount;
            //grpDistEncountersTrainerItemsCount.SetCurrent(config.TrainerItemCountDist);

            checkEncountersTrainerSpeciesRandom.Checked = config.TrainerRandomPokemon;
            checkEncountersTrainerSpeciesHighLevelLegends.Checked = config.TrainerHighLevelLegends;
            checkEncountersTrainerSpeciesTypeThemes.Checked = config.TrainerTypeThemes;
            checkEncountersTrainerSpeciesEvoLogic.Checked = config.TrainerEvolutionLogic;
            //btnDistEncountersTrainerSpecies.SetCurrent(config.TrainerSpeciesDist);

            comboEncountersTrainerMovesRandom.SelectedIndex = config.TrainerMovesMode;
            numEncountersTrainerMovesTypeBias.Value = (decimal)config.TrainerMoveTypeBiasP;
            //btnDistEncountersTrainerMoves.SetCurrent(config.TrainerMovesDist);

            checkDistEncountersTrainerPokemonCount.Checked = config.TrainerPokemonCount;
            //grpDistEncountersTrainerPokemonCount.SetCurrent(config.TrainerPokemonCountDist);

            checkDistEncountersTrainerLevels.Checked = config.TrainerLevels;
            //grpDistEncountersTrainerLevels.SetCurrent(config.TrainerLevelsDist);

            checkEncountersTrainerHeldItemsRandom.Checked = config.TrainerHeldItems;
            checkEncountersTrainerHeldItemsHighLevel.Checked = config.TrainerHighLevelHeldItems;
            //btnDistEncountersTrainerHeldItems.SetCurrent(config.TrainerHeldItemsDist);

            checkEncountersTrainerShinyRandom.Checked = config.TrainerShiny;
            numEncountersTrainerShiny.Value = (decimal)config.TrainerShinyP;

            checkEncountersTrainerNaturesRandom.Checked = config.TrainerNatures;
            //btnDistEncountersTrainerNatures.SetCurrent(config.TrainerNaturesDist);

            checkEncountersTrainerAbilitiesRandom.Checked = config.TrainerAbilities;
            checkEncountersTrainerAbilitiesIncludeUnobtainable.Checked = config.TrainerAbilitiesIncludeUnobtainable;
            //btnDistEncountersTrainerAbilities.SetCurrent(config.TrainerAbilitiesDist);

            checkDistEncountersTrainerIVs.Checked = config.TrainerIVs;
            //grpDistEncountersTrainerIVs.SetCurrent(config.TrainerIVsDist);

            checkDistEncountersTrainerEVs.Checked = config.TrainerEVs;
            //grpDistEncountersTrainerEVs.SetCurrent(config.TrainerEVsDist);
        }

        private void SetupFormFromConfigMisc(RandomizerSetupConfig.MiscConfig config)
        {
            checkMiscTypeMatchupsRandom.Checked = config.TypeMatchups;
            //btnDistMiscTypeMatchups.SetCurrent(config.TypeMatchupsDist);

            checkMiscShuffleText.Checked = config.ShuffleText;
            checkMiscShuffleTextKeepStrLength.Checked = config.ShuffleTextPreserveStringLength;
            checkMiscShuffleBGM.Checked = config.ShuffleBGM;

            checkMiscScriptsPokemonRandom.Checked = config.RandomScriptedPokemon;
            //btnDistMiscScriptsPokemon.SetCurrent(config.RandomScriptedPokemonDist);

            checkMiscScriptsItemsRandom.Checked = config.RandomScriptedItems;
            //btnDistMiscScriptsItems.SetCurrent(config.RandomScriptedItemsDist);

            checkMiscLevelMultEvoLevels.Checked = config.LevelMultiplierEvolutionLevels;
            checkMiscLevelMultLevelMoves.Checked = config.LevelMultiplierLevelUpMoves;
            checkMiscLevelMultWild.Checked = config.LevelMultiplierWildEncounters;
            checkMiscLevelMultTrainer.Checked = config.LevelMultiplierTrainerLevels;
            numMiscLevelMultMult.Value = (decimal)config.LevelMultiplierCoefficient;
        }

        private void UpdateConfigFromForm()
        {
            UpdateConfigFromFormPokemon(rsc.Pokemon);
            UpdateConfigFromFormMovesAndItems(rsc.MovesAndItems);
            UpdateConfigFromFormEncounters(rsc.Encounters);
            UpdateConfigFromFormMisc(rsc.Misc);
        }

        private void UpdateConfigFromFormPokemon(RandomizerSetupConfig.PokemonConfig config)
        {
            config.EvolutionRandomDestinations = checkPokemonEvolutionRandomDest.Checked;
            config.EvolutionBSTLogic = checkPokemonEvolutionBSTLogic.Checked;
            config.EvolutionDestinationPokemonDist = btnDistPokemonEvolutionDest.Get();
            config.EvolutionLevel = checkDistPokemonEvolutionLevel.Checked;
            config.EvolutionLevelDist = grpDistPokemonEvolutionLevel.Get();

            config.BaseStatsShuffle = checkPokemonBaseStatsShuffle.Checked;
            config.BaseStatsBSTLogic = checkPokemonBaseStatsBSTLogic.Checked;
            config.BaseStats = checkDistPokemonBaseStats.Checked;
            config.BaseStatsDist = grpDistPokemonBaseStats.Get();

            config.PokemonTyping = checkPokemonTypingRandom.Checked;
            config.PokemonTypingEvoLogic = checkPokemonTypingEvoLogic.Checked;
            config.PokemonTypingDist = btnDistPokemonTyping.Get();
            config.DoubleTypingP = (double)numPokemonTypingDouble.Value;

            config.TMCompatibility = checkPokemonTMCompatRandom.Checked;
            config.TMCompatibilityEvoLogic = checkPokemonTMCompatEvoLogic.Checked;
            config.TMCompatibilityP = (double)numPokemonTMCompatPercent.Value;
            config.TMCompatibilityTypeBiasP = (double)numPokemonTMCompatTypeBias.Value;

            config.WildHeldItems = checkPokemonWildHeldItemsRandom.Checked;
            config.WildHeldItemsDist = btnDistPokemonWildHeldItems.Get();

            config.GrowthRate = checkPokemonGrowthRandom.Checked;
            config.GrowthRateDist = btnDistPokemonGrowth.Get();

            config.Abilities = checkPokemonAbilities.Checked;
            config.AbilitiesDist = btnDistPokemonAbilities.Get();

            config.CatchRate = checkDistPokemonMiscCatchRate.Checked;
            config.CatchRateDist = grpDistPokemonMiscCatchRate.Get();

            config.EVYield = checkDistPokemonMiscEVs.Checked;
            config.EVYieldDist = grpDistPokemonMiscEVs.Get();

            config.InitialFriendship = checkDistPokemonMiscInitFriendship.Checked;
            config.InitialFriendshipDist = grpDistPokemonMiscInitFriendship.Get();

            config.ExpYield = checkDistPokemonMiscExp.Checked;
            config.ExpYieldDist = grpDistPokemonMiscExp.Get();

            config.EggMoves = checkPokemonEggMovesRandom.Checked;
            config.EggMovesDist = btnDistPokemonEggMoves.Get();
            config.EggMoveTypeBiasP = (double)numPokemonEggMovesTypeBias.Value;
            config.EggMovesCount = checkDistPokemonEggMovesCount.Checked;
            config.EggMovesCountDist = grpDistPokemonEggMovesCount.Get();

            config.LevelUpMoves = checkPokemonLevelMovesRandom.Checked;
            config.LevelUpMovesSortByPower = checkPokemonLevelMovesSortPower.Checked;
            config.LevelUpMovesDist = btnDistPokemonLevelMovesMoves.Get();
            config.LevelUpMovesTypeBiasP = (double)numPokemonLevelMovesTypeBias.Value;
            config.LevelUpMovesLevel = checkDistPokemonLevelMovesLearnLevels.Checked;
            config.LevelUpMovesLevelDist = grpDistPokemonLevelMovesLearnLevels.Get();
            config.LevelUpMovesCount = checkDistPokemonLevelMovesCount.Checked;
            config.LevelUpMovesCountDist = grpDistPokemonLevelMovesCount.Get();
        }

        private void UpdateConfigFromFormMovesAndItems(RandomizerSetupConfig.MovesAndItemsConfig config)
        {
            config.MoveTyping = checkMovesAndItemsMoveTypingRandom.Checked;
            config.MoveTypingDist = btnDistMovesAndItemsMoveTyping.Get();

            config.DamageCategory = checkMovesAndItemsMoveCategoryRandom.Checked;
            config.DamageCategoryDist = btnDistMovesAndItemsMoveCategory.Get();

            config.TMMoves = checkMovesAndItemsTMMovesRandom.Checked;
            config.TMMovesDist = btnDistMovesAndItemsTMMoves.Get();

            config.MovePower = checkDistMovesAndItemsPower.Checked;
            config.MovePowerDist = grpDistMovesAndItemsPower.Get();

            config.MoveAccuracy = checkDistMovesAndItemsAccuracy.Checked;
            config.MoveAccuracyDist = grpDistMovesAndItemsAccuracy.Get();

            config.MovePP = checkDistMovesAndItemsPP.Checked;
            config.MovePPDist = grpDistMovesAndItemsPP.Get();

            config.ItemPrices = checkDistMovesAndItemsItemPrices.Checked;
            config.ItemPricesDist = grpDistMovesAndItemsItemPrices.Get();

            config.PickupItems = checkMovesAndItemsPickupRandom.Checked;
            config.PickupItemsDist = btnDistMovesAndItemsPickup.Get();

            config.ShopItems = checkMovesAndItemsShopItemsRandom.Checked;
            config.ShopItemsPreserveRegularMart = checkMovesAndItemsShopItemsKeepMart.Checked;
            config.ShopItemsDist = btnDistMovesAndItemsShopItems.Get();
        }

        private void UpdateConfigFromFormEncounters(RandomizerSetupConfig.EncountersConfig config)
        {
            config.WildEncountersRandomPokemon = checkEncountersWildRandom.Checked;
            config.WildEncountersHighLevelLegends = checkEncountersWildHighLevelLegends.Checked;
            config.WildEncountersEvolutionLogic = checkEncountersWildEvoLogic.Checked;
            config.WildEncountersWildPokemonDist = btnDistEncountersWild.Get();
            config.WildEncountersWildPokemonLevels = checkDistEncountersWildLevels.Checked;
            config.WildEncountersWildPokemonLevelsDist = grpDistEncountersWildLevels.Get();

            config.TrainerItems = checkEncountersTrainerItemsRandom.Checked;
            config.TrainerItemsDist = btnDistEncountersTrainerItems.Get();
            config.TrainerItemCount = checkDistEncountersTrainerItemsCount.Checked;
            config.TrainerItemCountDist = grpDistEncountersTrainerItemsCount.Get();

            config.TrainerRandomPokemon = checkEncountersTrainerSpeciesRandom.Checked;
            config.TrainerHighLevelLegends = checkEncountersTrainerSpeciesHighLevelLegends.Checked;
            config.TrainerTypeThemes = checkEncountersTrainerSpeciesTypeThemes.Checked;
            config.TrainerEvolutionLogic = checkEncountersTrainerSpeciesEvoLogic.Checked;
            config.TrainerSpeciesDist = btnDistEncountersTrainerSpecies.Get();

            config.TrainerMovesMode = comboEncountersTrainerMovesRandom.SelectedIndex;
            config.TrainerMoveTypeBiasP = (double)numEncountersTrainerMovesTypeBias.Value;
            config.TrainerMovesDist = btnDistEncountersTrainerMoves.Get();

            config.TrainerPokemonCount = checkDistEncountersTrainerPokemonCount.Checked;
            config.TrainerPokemonCountDist = grpDistEncountersTrainerPokemonCount.Get();

            config.TrainerLevels = checkDistEncountersTrainerLevels.Checked;
            config.TrainerLevelsDist = grpDistEncountersTrainerLevels.Get();

            config.TrainerHeldItems = checkEncountersTrainerHeldItemsRandom.Checked;
            config.TrainerHighLevelHeldItems = checkEncountersTrainerHeldItemsHighLevel.Checked;
            config.TrainerHeldItemsDist = btnDistEncountersTrainerHeldItems.Get();

            config.TrainerShiny = checkEncountersTrainerShinyRandom.Checked;
            config.TrainerShinyP = (double)numEncountersTrainerShiny.Value;

            config.TrainerNatures = checkEncountersTrainerNaturesRandom.Checked;
            config.TrainerNaturesDist = btnDistEncountersTrainerNatures.Get();

            config.TrainerAbilities = checkEncountersTrainerAbilitiesRandom.Checked;
            config.TrainerAbilitiesIncludeUnobtainable = checkEncountersTrainerAbilitiesIncludeUnobtainable.Checked;
            config.TrainerAbilitiesDist = btnDistEncountersTrainerAbilities.Get();

            config.TrainerIVs = checkDistEncountersTrainerIVs.Checked;
            config.TrainerIVsDist = grpDistEncountersTrainerIVs.Get();

            config.TrainerEVs = checkDistEncountersTrainerEVs.Checked;
            config.TrainerEVsDist = grpDistEncountersTrainerEVs.Get();
        }

        private void UpdateConfigFromFormMisc(RandomizerSetupConfig.MiscConfig config)
        {
            config.TypeMatchups = checkMiscTypeMatchupsRandom.Checked;
            config.TypeMatchupsDist = btnDistMiscTypeMatchups.Get();

            config.ShuffleText = checkMiscShuffleText.Checked;
            config.ShuffleTextPreserveStringLength = checkMiscShuffleTextKeepStrLength.Checked;
            config.ShuffleBGM = checkMiscShuffleBGM.Checked;

            config.RandomScriptedPokemon = checkMiscScriptsPokemonRandom.Checked;
            config.RandomScriptedPokemonDist = btnDistMiscScriptsPokemon.Get();

            config.RandomScriptedItems = checkMiscScriptsItemsRandom.Checked;
            config.RandomScriptedItemsDist = btnDistMiscScriptsItems.Get();

            config.LevelMultiplierEvolutionLevels = checkMiscLevelMultEvoLevels.Checked;
            config.LevelMultiplierLevelUpMoves = checkMiscLevelMultLevelMoves.Checked;
            config.LevelMultiplierWildEncounters = checkMiscLevelMultWild.Checked;
            config.LevelMultiplierTrainerLevels = checkMiscLevelMultTrainer.Checked;
            config.LevelMultiplierCoefficient = (double)numMiscLevelMultMult.Value;
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

            gridMiscAbsoluteBoundaries.DataSource = controller.GetAbsoluteBoundariesTable();
            foreach (DataGridViewColumn c in gridMiscAbsoluteBoundaries.Columns)
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
