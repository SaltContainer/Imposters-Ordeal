using static ImpostersOrdeal.Distributions;
using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class RandomizerSetupConfig
    {
        public (IDistribution[], List<string>, int) evolutionDestinationPokemon;
        public (IDistribution[], int) evolutionLevel;
        public (IDistribution[], int) baseStats;
        public (IDistribution[], List<string>, int) pokemonTyping;
        public double doubleTypingP;
        public double tmCompatibilityP;
        public double tmCompatibilityTypeBiasP;
        public (IDistribution[], List<string>, int) wildHeldItems;
        public (IDistribution[], List<string>, int) growthRate;
        public (IDistribution[], List<string>, int) abilities;
        public (IDistribution[], int) catchRate;
        public (IDistribution[], int) evYields;
        public (IDistribution[], int) initialFriendship;
        public (IDistribution[], int) expYield;
        public (IDistribution[], List<string>, int) eggMoves;
        public double eggMoveTypeBiasP;
        public (IDistribution[], int) eggMoveCount;
        public (IDistribution[], List<string>, int) levelUpMoves;
        public double levelUpMoveTypeBiasP;
        public (IDistribution[], int) levelUpMoveLevels;
        public (IDistribution[], int) levelUpMoveCount;

        public (IDistribution[], List<string>, int) moveTyping;
        public (IDistribution[], List<string>, int) damageCategory;
        public (IDistribution[], List<string>, int) tmMoves;
        public (IDistribution[], int) movePower;
        public (IDistribution[], int) moveAccuracy;
        public (IDistribution[], int) movePp;
        public (IDistribution[], int) itemPrices;
        public (IDistribution[], List<string>, int) pickupItems;
        public (IDistribution[], List<string>, int) shopItems;

        public (IDistribution[], List<string>, int) wildPokemon;
        public (IDistribution[], int) wildPokemonLevels;
        public (IDistribution[], List<string>, int) trainerItems;
        public (IDistribution[], int) trainerItemCount;
        public (IDistribution[], List<string>, int) trainerPokemonSpecies;
        public (IDistribution[], List<string>, int) trainerPokemonMoves;
        public double trainerPokemonMoveTypeBiasP;
        public (IDistribution[], int) trainerPokemonCount;
        public (IDistribution[], int) trainerPokemonLevels;
        public (IDistribution[], List<string>, int) trainerPokemonHeldItems;
        public double trainerPokemonShinyP;
        public (IDistribution[], List<string>, int) trainerPokemonNatures;
        public (IDistribution[], List<string>, int) trainerPokemonAbilities;
        public (IDistribution[], int) trainerPokemonIvs;
        public (IDistribution[], int) trainerPokemonEvs;

        public (IDistribution[], List<string>, int) typeMatchups;
        public (IDistribution[], List<string>, int) scriptedPokemon;
        public (IDistribution[], List<string>, int) scriptedItems;
        public double levelCoefficient;

        public IDistribution evolutionLogicTypingCorrelationDistribution;
        public IDistribution evolutionMoveCount;
    }
}
