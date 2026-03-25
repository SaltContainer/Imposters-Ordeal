using System.Collections.Generic;
using static ImpostersOrdeal.Distributions;

namespace ImpostersOrdeal
{
    public class DistributionsSetupConfig
    {
        public PokemonConfig Pokemon { get; set; } = new PokemonConfig();
        public MovesAndItemsConfig MovesAndItems { get; set; } = new MovesAndItemsConfig();
        public EncountersConfig Encounters { get; set; } = new EncountersConfig();
        public MiscConfig Misc { get; set; } = new MiscConfig();

        public class PokemonConfig
        {
            public (IDistribution[], List<string>, int) EvolutionDestinationPokemonDists { get; set; }
            public (IDistribution[], int) EvolutionLevelDists { get; set; }
            public (IDistribution[], int) BaseStatsDists { get; set; }
            public (IDistribution[], List<string>, int) PokemonTypingDists { get; set; }
            public double DoubleTypingP { get; set; }
            public IDistribution PokemonTypingEvoLogicCorrelationDist { get; set; }
            public double TMCompatibilityP { get; set; }
            public double TMCompatibilityTypeBiasP { get; set; }
            public (IDistribution[], List<string>, int) WildHeldItemsDists { get; set; }
            public (IDistribution[], List<string>, int) GrowthRateDists { get; set; }
            public (IDistribution[], List<string>, int) AbilitiesDists { get; set; }
            public (IDistribution[], int) CatchRateDists { get; set; }
            public (IDistribution[], int) EVYieldDists { get; set; }
            public (IDistribution[], int) InitialFriendshipDists { get; set; }
            public (IDistribution[], int) ExpYieldDists { get; set; }
            public (IDistribution[], List<string>, int) EggMovesDists { get; set; }
            public (IDistribution[], int) EggMovesCountDists { get; set; }
            public double EggMoveTypeBiasP { get; set; }
            public (IDistribution[], List<string>, int) LevelUpMovesDists { get; set; }
            public double LevelUpMovesTypeBiasP { get; set; }
            public (IDistribution[], int) LevelUpMovesLevelDists { get; set; }
            public (IDistribution[], int) LevelUpMovesCountDists { get; set; }
            public IDistribution EvoMovesCountDist { get; set; }
        }

        public class MovesAndItemsConfig
        {
            public (IDistribution[], List<string>, int) MoveTypingDists { get; set; }
            public (IDistribution[], List<string>, int) DamageCategoryDists { get; set; }
            public (IDistribution[], List<string>, int) TMMovesDists { get; set; }
            public (IDistribution[], int) MovePowerDists { get; set; }
            public (IDistribution[], int) MoveAccuracyDists { get; set; }
            public (IDistribution[], int) MovePPDists { get; set; }
            public (IDistribution[], int) ItemPricesDists { get; set; }
            public (IDistribution[], List<string>, int) PickupItemsDists { get; set; }
            public (IDistribution[], List<string>, int) ShopItemsDists { get; set; }
        }

        public class EncountersConfig
        {
            public (IDistribution[], List<string>, int) WildEncountersWildPokemonDists { get; set; }
            public (IDistribution[], int) WildEncountersWildPokemonLevelsDists { get; set; }
            public (IDistribution[], List<string>, int) TrainerItemsDists { get; set; }
            public (IDistribution[], int) TrainerItemCountDists { get; set; }
            public (IDistribution[], List<string>, int) TrainerSpeciesDists { get; set; }
            public double TrainerMoveTypeBiasP { get; set; }
            public (IDistribution[], List<string>, int) TrainerMovesDists { get; set; }
            public (IDistribution[], int) TrainerPokemonCountDists { get; set; }
            public (IDistribution[], int) TrainerLevelsDists { get; set; }
            public (IDistribution[], List<string>, int) TrainerHeldItemsDists { get; set; }
            public double TrainerShinyP { get; set; }
            public (IDistribution[], List<string>, int) TrainerNaturesDists { get; set; }
            public (IDistribution[], List<string>, int) TrainerAbilitiesDists { get; set; }
            public (IDistribution[], int) TrainerIVsDists { get; set; }
            public (IDistribution[], int) TrainerEVsDists { get; set; }
        }

        public class MiscConfig
        {
            public (IDistribution[], List<string>, int) TypeMatchupsDists { get; set; }
            public (IDistribution[], List<string>, int) RandomScriptedPokemonDists { get; set; }
            public (IDistribution[], List<string>, int) RandomScriptedItemsDists { get; set; }
        }
    }
}
