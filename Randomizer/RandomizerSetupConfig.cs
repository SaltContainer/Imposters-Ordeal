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
            public bool EvolutionRandomDestinations { get; set; }
            public bool EvolutionBSTLogic { get; set; }
            public IDistribution EvolutionDestinationPokemonDist { get; set; }
            public bool EvolutionLevel { get; set; }
            public IDistribution EvolutionLevelDist { get; set; }

            public bool BaseStatsShuffle { get; set; }
            public bool BaseStatsBSTLogic { get; set; }
            public bool BaseStats { get; set; }
            public IDistribution BaseStatsDist { get; set; }

            public bool PokemonTyping { get; set; }
            public bool PokemonTypingEvoLogic { get; set; }
            public IDistribution PokemonTypingDist { get; set; }
            public double DoubleTypingP { get; set; }
            public IDistribution PokemonTypingEvoLogicCorrelationDist { get; set; }

            public bool TMCompatibility { get; set; }
            public bool TMCompatibilityEvoLogic { get; set; }
            public double TMCompatibilityP { get; set; }
            public double TMCompatibilityTypeBiasP { get; set; }

            public bool WildHeldItems { get; set; }
            public IDistribution WildHeldItemsDist { get; set; }

            public bool GrowthRate { get; set; }
            public IDistribution GrowthRateDist { get; set; }

            public bool Abilities { get; set; }
            public IDistribution AbilitiesDist { get; set; }

            public bool CatchRate { get; set; }
            public IDistribution CatchRateDist { get; set; }

            public bool EVYield { get; set; }
            public IDistribution EVYieldDist { get; set; }

            public bool InitialFriendship { get; set; }
            public IDistribution InitialFriendshipDist { get; set; }

            public bool ExpYield { get; set; }
            public IDistribution ExpYieldDist { get; set; }

            public bool EggMoves { get; set; }
            public IDistribution EggMovesDist { get; set; }
            public double EggMoveTypeBiasP { get; set; }
            public bool EggMovesCount { get; set; }
            public IDistribution EggMovesCountDist { get; set; }

            public bool LevelUpMoves { get; set; }
            public bool LevelUpMovesSortByPower { get; set; }
            public IDistribution LevelUpMovesDist { get; set; }
            public double LevelUpMovesTypeBiasP { get; set; }
            public bool LevelUpMovesLevel { get; set; }
            public IDistribution LevelUpMovesLevelDist { get; set; }
            public bool LevelUpMovesCount { get; set; }
            public IDistribution LevelUpMovesCountDist { get; set; }
            public IDistribution EvoMovesCountDist { get; set; }
        }
        
        public class MovesAndItemsConfig
        {
            public bool MoveTyping { get; set; }
            public IDistribution MoveTypingDist { get; set; }

            public bool DamageCategory { get; set; }
            public IDistribution DamageCategoryDist { get; set; }

            public bool TMMoves { get; set; }
            public IDistribution TMMovesDist { get; set; }

            public bool MovePower { get; set; }
            public IDistribution MovePowerDist { get; set; }

            public bool MoveAccuracy { get; set; }
            public IDistribution MoveAccuracyDist { get; set; }

            public bool MovePP { get; set; }
            public IDistribution MovePPDist { get; set; }

            public bool ItemPrices { get; set; }
            public IDistribution ItemPricesDist { get; set; }

            public bool PickupItems { get; set; }
            public IDistribution PickupItemsDist { get; set; }

            public bool ShopItems { get; set; }
            public bool ShopItemsPreserveRegularMart { get; set; }
            public IDistribution ShopItemsDist { get; set; }
        }

        public class EncountersConfig
        {
            public bool WildEncountersRandomPokemon { get; set; }
            public bool WildEncountersHighLevelLegends { get; set; }
            public bool WildEncountersEvolutionLogic { get; set; }
            public IDistribution WildEncountersWildPokemonDist { get; set; }
            public bool WildEncountersWildPokemonLevels { get; set; }
            public IDistribution WildEncountersWildPokemonLevelsDist { get; set; }

            public bool TrainerItems { get; set; }
            public IDistribution TrainerItemsDist { get; set; }
            public bool TrainerItemCount { get; set; }
            public IDistribution TrainerItemCountDist { get; set; }

            public bool TrainerRandomPokemon { get; set; }
            public bool TrainerHighLevelLegends { get; set; }
            public bool TrainerTypeThemes { get; set; }
            public bool TrainerEvolutionLogic { get; set; }
            public IDistribution TrainerSpeciesDist { get; set; }

            public int TrainerMovesMode { get; set; }
            public double TrainerMoveTypeBiasP { get; set; }
            public IDistribution TrainerMovesDist { get; set; }

            public bool TrainerPokemonCount { get; set; }
            public IDistribution TrainerPokemonCountDist { get; set; }

            public bool TrainerLevels { get; set; }
            public IDistribution TrainerLevelsDist { get; set; }

            public bool TrainerHeldItems { get; set; }
            public bool TrainerHighLevelHeldItems { get; set; }
            public IDistribution TrainerHeldItemsDist { get; set; }

            public bool TrainerShiny { get; set; }
            public double TrainerShinyP { get; set; }

            public bool TrainerNatures { get; set; }
            public IDistribution TrainerNaturesDist { get; set; }

            public bool TrainerAbilities { get; set; }
            public bool TrainerAbilitiesIncludeUnobtainable { get; set; }
            public IDistribution TrainerAbilitiesDist { get; set; }

            public bool TrainerIVs { get; set; }
            public IDistribution TrainerIVsDist { get; set; }

            public bool TrainerEVs { get; set; }
            public IDistribution TrainerEVsDist { get; set; }
        }

        public class MiscConfig
        {
            public bool TypeMatchups { get; set; }
            public IDistribution TypeMatchupsDist { get; set; }

            public bool ShuffleText { get; set; }
            public bool ShuffleTextPreserveStringLength { get; set; }
            public bool ShuffleBGM { get; set; }

            public bool RandomScriptedPokemon { get; set; }
            public IDistribution RandomScriptedPokemonDist { get; set; }

            public bool RandomScriptedItems { get; set; }
            public IDistribution RandomScriptedItemsDist { get; set; }

            public bool LevelMultiplierEvolutionLevels { get; set; }
            public bool LevelMultiplierLevelUpMoves { get; set; }
            public bool LevelMultiplierWildEncounters { get; set; }
            public bool LevelMultiplierTrainerLevels { get; set; }
            public double LevelMultiplierCoefficient { get; set; }
        }
    }
}
