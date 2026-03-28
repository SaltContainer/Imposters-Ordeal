using static ImpostersOrdeal.Distributions;

namespace ImpostersOrdeal
{
    public class RandomizerSetupConfig
    {
        public PokemonConfig Pokemon { get; set; } = new PokemonConfig();
        public MovesAndItemsConfig MovesAndItems { get; set; } = new MovesAndItemsConfig();
        public EncountersConfig Encounters { get; set; } = new EncountersConfig();
        public MiscConfig Misc { get; set; } = new MiscConfig();

        public class PokemonConfig
        {
            public bool EvolutionRandomDestinations { get; set; } = false;
            public bool EvolutionBSTLogic { get; set; } = false;
            public IDistribution EvolutionDestinationPokemonDist { get; set; } = null;
            public bool EvolutionLevel { get; set; } = false;
            public IDistribution EvolutionLevelDist { get; set; } = null;

            public bool BaseStatsShuffle { get; set; } = false;
            public bool BaseStatsBSTLogic { get; set; } = false;
            public bool BaseStats { get; set; } = false;
            public IDistribution BaseStatsDist { get; set; } = null;

            public bool PokemonTyping { get; set; } = false;
            public bool PokemonTypingEvoLogic { get; set; } = false;
            public IDistribution PokemonTypingDist { get; set; } = null;
            public double DoubleTypingP { get; set; } = 0.0;
            public IDistribution PokemonTypingEvoLogicCorrelationDist { get; set; } = null;

            public bool TMCompatibility { get; set; } = false;
            public bool TMCompatibilityEvoLogic { get; set; } = false;
            public double TMCompatibilityP { get; set; } = 0.0;
            public double TMCompatibilityTypeBiasP { get; set; } = 0.0;

            public bool WildHeldItems { get; set; } = false;
            public IDistribution WildHeldItemsDist { get; set; } = null;

            public bool GrowthRate { get; set; } = false;
            public IDistribution GrowthRateDist { get; set; } = null;

            public bool Abilities { get; set; } = false;
            public IDistribution AbilitiesDist { get; set; } = null;

            public bool CatchRate { get; set; } = false;
            public IDistribution CatchRateDist { get; set; } = null;

            public bool EVYield { get; set; } = false;
            public IDistribution EVYieldDist { get; set; } = null;

            public bool InitialFriendship { get; set; } = false;
            public IDistribution InitialFriendshipDist { get; set; } = null;

            public bool ExpYield { get; set; } = false;
            public IDistribution ExpYieldDist { get; set; } = null;

            public bool EggMoves { get; set; } = false;
            public IDistribution EggMovesDist { get; set; } = null;
            public double EggMoveTypeBiasP { get; set; } = 0.0;
            public bool EggMovesCount { get; set; } = false;
            public IDistribution EggMovesCountDist { get; set; } = null;

            public bool LevelUpMoves { get; set; } = false;
            public bool LevelUpMovesSortByPower { get; set; } = false;
            public IDistribution LevelUpMovesDist { get; set; } = null;
            public double LevelUpMovesTypeBiasP { get; set; } = 0.0;
            public bool LevelUpMovesLevel { get; set; } = false;
            public IDistribution LevelUpMovesLevelDist { get; set; } = null;
            public bool LevelUpMovesCount { get; set; } = false;
            public IDistribution LevelUpMovesCountDist { get; set; } = null;
            public IDistribution EvoMovesCountDist { get; set; } = null;
        }
        
        public class MovesAndItemsConfig
        {
            public bool MoveTyping { get; set; } = false;
            public IDistribution MoveTypingDist { get; set; } = null;

            public bool DamageCategory { get; set; } = false;
            public IDistribution DamageCategoryDist { get; set; } = null;

            public bool TMMoves { get; set; } = false;
            public IDistribution TMMovesDist { get; set; } = null;

            public bool MovePower { get; set; } = false;
            public IDistribution MovePowerDist { get; set; } = null;

            public bool MoveAccuracy { get; set; } = false;
            public IDistribution MoveAccuracyDist { get; set; } = null;

            public bool MovePP { get; set; } = false;
            public IDistribution MovePPDist { get; set; } = null;

            public bool ItemPrices { get; set; } = false;
            public IDistribution ItemPricesDist { get; set; } = null;

            public bool PickupItems { get; set; } = false;
            public IDistribution PickupItemsDist { get; set; } = null;

            public bool ShopItems { get; set; } = false;
            public bool ShopItemsPreserveRegularMart { get; set; } = false;
            public IDistribution ShopItemsDist { get; set; } = null;
        }

        public class EncountersConfig
        {
            public bool WildEncountersRandomPokemon { get; set; } = false;
            public bool WildEncountersHighLevelLegends { get; set; } = false;
            public bool WildEncountersEvolutionLogic { get; set; } = false;
            public IDistribution WildEncountersWildPokemonDist { get; set; } = null;
            public bool WildEncountersWildPokemonLevels { get; set; } = false;
            public IDistribution WildEncountersWildPokemonLevelsDist { get; set; } = null;

            public bool TrainerItems { get; set; } = false;
            public IDistribution TrainerItemsDist { get; set; } = null;
            public bool TrainerItemCount { get; set; } = false;
            public IDistribution TrainerItemCountDist { get; set; } = null;

            public bool TrainerRandomPokemon { get; set; } = false;
            public bool TrainerHighLevelLegends { get; set; } = false;
            public bool TrainerTypeThemes { get; set; } = false;
            public bool TrainerEvolutionLogic { get; set; } = false;
            public IDistribution TrainerSpeciesDist { get; set; } = null;

            public int TrainerMovesMode { get; set; } = 0;
            public double TrainerMoveTypeBiasP { get; set; } = 0.0;
            public IDistribution TrainerMovesDist { get; set; } = null;

            public bool TrainerPokemonCount { get; set; } = false;
            public IDistribution TrainerPokemonCountDist { get; set; } = null;

            public bool TrainerLevels { get; set; } = false;
            public IDistribution TrainerLevelsDist { get; set; } = null;

            public bool TrainerHeldItems { get; set; } = false;
            public bool TrainerHighLevelHeldItems { get; set; } = false;
            public IDistribution TrainerHeldItemsDist { get; set; } = null;

            public bool TrainerShiny { get; set; } = false;
            public double TrainerShinyP { get; set; } = 0.0;

            public bool TrainerNatures { get; set; } = false;
            public IDistribution TrainerNaturesDist { get; set; } = null;

            public bool TrainerAbilities { get; set; } = false;
            public bool TrainerAbilitiesIncludeUnobtainable { get; set; } = false;
            public IDistribution TrainerAbilitiesDist { get; set; } = null;

            public bool TrainerIVs { get; set; } = false;
            public IDistribution TrainerIVsDist { get; set; } = null;

            public bool TrainerEVs { get; set; } = false;
            public IDistribution TrainerEVsDist { get; set; } = null;
        }

        public class MiscConfig
        {
            public bool TypeMatchups { get; set; } = false;
            public IDistribution TypeMatchupsDist { get; set; } = null;

            public bool ShuffleText { get; set; } = false;
            public bool ShuffleTextPreserveStringLength { get; set; } = false;
            public bool ShuffleBGM { get; set; } = false;

            public bool RandomScriptedPokemon { get; set; } = false;
            public IDistribution RandomScriptedPokemonDist { get; set; } = null;

            public bool RandomScriptedItems { get; set; } = false;
            public IDistribution RandomScriptedItemsDist { get; set; } = null;

            public bool LevelMultiplierEvolutionLevels { get; set; } = false;
            public bool LevelMultiplierLevelUpMoves { get; set; } = false;
            public bool LevelMultiplierWildEncounters { get; set; } = false;
            public bool LevelMultiplierTrainerLevels { get; set; } = false;
            public double LevelMultiplierCoefficient { get; set; } = 1.0;
        }
    }
}
