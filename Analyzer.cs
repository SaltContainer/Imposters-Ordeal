using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    /// <summary>
    /// Responsible for figuring out a good starting configuration given the loaded files.
    /// </summary>
    public class Analyzer
    {
        /// <summary>
        /// Generates DistributionsSetupConfig through statistical analysis of gamefiles.
        /// </summary>
        public DistributionsSetupConfig GetSetupConfig(GameDataSet gameData)
        {
            DistributionsSetupConfig distributionsSetupConfig = new();

            // Evolution Destinations
            distributionsSetupConfig.Pokemon.EvolutionDestinationPokemonDists = gameData.pokemonDataTable.Data.Skip(1).SelectMany(p => p.nextPokemon)
                .GetItemDistributionConfig(p => p.personal.id,
                    gameData.pokemonDataTable.Data,
                    e => e.personal.Valid,
                    e => gameData.GetFormName(e.personal.monsno, e.formID));

            // Evolution Levels
            distributionsSetupConfig.Pokemon.EvolutionLevelDists = gameData.pokemonDataTable.Data.Skip(1).SelectMany(p => p.evolutionPaths.paths)
                .GetNumericDistributionConfig(p => p.level, AbsoluteBoundaries.Boundary.Level);

            // Base Stats
            distributionsSetupConfig.Pokemon.BaseStatsDists = gameData.pokemonDataTable.Data.Skip(1).SelectMany(p => p.personal.BaseStats)
                .GetNumericDistributionConfig(p => p, AbsoluteBoundaries.Boundary.BaseStat);

            // Pokémon Typing
            distributionsSetupConfig.Pokemon.PokemonTypingDists = gameData.pokemonDataTable.Data.Skip(1).SelectMany(p => p.personal.Types)
                .GetItemDistributionConfig(p => p,
                    gameData.GetAllLabels(Constants.TYPE_MESSAGEFILE_NAME),
                    e => true,
                    e => e);

            // Pokémon Typing
            distributionsSetupConfig.Pokemon.DoubleTypingP = gameData.pokemonDataTable.Data.GetOccurrencePercent(p => p.personal.type1 != p.personal.type2);

            // Typing Evolution Logic
            distributionsSetupConfig.Pokemon.PokemonTypingEvoLogicCorrelationDist = gameData.pokemonDataTable.Data.Skip(1).SelectMany(p => p.nextPokemon.Select(n => (p, n)))
                .GetItemDistributionConfig(ps => (int)CompareTyping(ps.p, ps.n),
                    gameData.pokemonDataTable.Data,
                    e => e.personal.Valid,
                    e => string.Empty).Item1[0]; // Only Empirical

            // TM Compatibility
            distributionsSetupConfig.Pokemon.TMCompatibilityP = gameData.pokemonDataTable.Data.Skip(1).Select(p => p.personal.TMFlags.GetOccurrencePercent(f => f)).Average();

            // TM Type Bias (NOTE: logic changed)
            distributionsSetupConfig.Pokemon.TMCompatibilityTypeBiasP = gameData.pokemonDataTable.Data.Skip(1).SelectMany(p => p.personal.CompatibleTMs
                    .Select(t => (p, gameData.moveTable.Waza[gameData.itemTable.WazaMachine[t].wazaNo­])))
                .GetOccurrencePercent(pm => pm.Item1.personal.Types.Contains(pm.Item2.type));

            // Wild Held Items
            distributionsSetupConfig.Pokemon.WildHeldItemsDists = gameData.pokemonDataTable.Data.Skip(1).SelectMany(p => p.personal.HeldItems)
                .GetItemDistributionConfig(p => p,
                    gameData.itemTable.Item,
                    e => e.Enabled,
                    e => gameData.GetLabelByIndex(Constants.ITEM_MESSAGEFILE_NAME, e.no));

            // Growth Rate
            distributionsSetupConfig.Pokemon.GrowthRateDists = gameData.pokemonDataTable.Data.Select(p => p.personal.grow)
                .GetItemDistributionConfig(g => g,
                    gameData.growthRateTable.Rates,
                    e => e.id > 0 && e.id < 6,
                    e => string.Empty); // TODO: Growth Rate names

            // Abilities
            distributionsSetupConfig.Pokemon.AbilitiesDists = gameData.pokemonDataTable.Data.Skip(1).SelectMany(p => p.personal.Abilities)
                .GetItemDistributionConfig(a => a,
                    gameData.GetAllLabels(Constants.ABILITY_MESSAGEFILE_NAME),
                    e => true,
                    e => e);

            // Catch Rate
            distributionsSetupConfig.Pokemon.CatchRateDists = gameData.pokemonDataTable.Data.GetNumericDistributionConfig(p => p.personal.get_rate, AbsoluteBoundaries.Boundary.CatchRate);

            // EV Yield
            distributionsSetupConfig.Pokemon.EVYieldDists = gameData.pokemonDataTable.Data.GetNumericDistributionConfig(p => p.personal.EVYieldTotal, AbsoluteBoundaries.Boundary.EvYieldTotal);

            // Initial Friendship
            distributionsSetupConfig.Pokemon.InitialFriendshipDists = gameData.pokemonDataTable.Data.GetNumericDistributionConfig(p => p.personal.initial_friendship, AbsoluteBoundaries.Boundary.InitialFriendship);

            // Exp Yield
            distributionsSetupConfig.Pokemon.ExpYieldDists = gameData.pokemonDataTable.Data.GetNumericDistributionConfig(p => p.personal.give_exp, AbsoluteBoundaries.Boundary.ExpYield);

            // Egg Moves
            distributionsSetupConfig.Pokemon.EggMovesDists = gameData.pokemonDataTable.Data.Skip(1).SelectMany(p => p.eggMoves.wazaNo)
                .GetItemDistributionConfig(m => m,
                    gameData.moveTable.Waza,
                    e => e.isValid,
                    e => gameData.GetLabelByIndex(Constants.MOVE_MESSAGEFILE_NAME, e.wazaNo));

            // Egg Move Type Bias (NOTE: logic changed)
            distributionsSetupConfig.Pokemon.EggMoveTypeBiasP = gameData.pokemonDataTable.Data.SelectMany(p => p.eggMoves.wazaNo
                    .Select(m => (p, gameData.moveTable.Waza[m])))
                .GetOccurrencePercent(pm => pm.Item1.personal.Types.Contains(pm.Item2.type));

            // Egg Move Count
            distributionsSetupConfig.Pokemon.EggMovesCountDists = gameData.pokemonDataTable.Data.GetNumericDistributionConfig(p => p.eggMoves.wazaNo.Count, AbsoluteBoundaries.Boundary.EggMoveCount);

            // Level Up Moves
            distributionsSetupConfig.Pokemon.LevelUpMovesDists = gameData.pokemonDataTable.Data.Skip(1).SelectMany(p => p.levelUpMoves.moves)
                .GetItemDistributionConfig(m => m.move,
                    gameData.moveTable.Waza,
                    e => e.isValid,
                    e => gameData.GetLabelByIndex(Constants.MOVE_MESSAGEFILE_NAME, e.wazaNo));

            // Level Up Move Type Bias
            distributionsSetupConfig.Pokemon.LevelUpMovesTypeBiasP = gameData.pokemonDataTable.Data.SelectMany(p => p.levelUpMoves.moves
                    .Select(m => (p, gameData.moveTable.Waza[m.move])))
                .GetOccurrencePercent(pm => pm.Item1.personal.Types.Contains(pm.Item2.type));

            // Level Up Move Levels
            distributionsSetupConfig.Pokemon.EggMovesCountDists = gameData.pokemonDataTable.Data.Skip(1).SelectMany(p => p.levelUpMoves.moves)
                .GetNumericDistributionConfig(m => m.level, AbsoluteBoundaries.Boundary.Level);

            // Evolution Move Count
            distributionsSetupConfig.Pokemon.EvoMovesCountDist = gameData.pokemonDataTable.Data.Where(p => p.pastPokemon.Count > 0)
                .GetNumericDistributionConfig(p => p.levelUpMoves.moves.Where(m => m.level == 0).Count(), AbsoluteBoundaries.Boundary.LevelUpMoveCount).Item1[3]; // Only NormalConstant

            // Level Up Move Count
            distributionsSetupConfig.Pokemon.LevelUpMovesCountDists = gameData.pokemonDataTable.Data
                .GetNumericDistributionConfig(p => p.levelUpMoves.moves.Count, AbsoluteBoundaries.Boundary.LevelUpMoveCount);

            // Move Typing
            distributionsSetupConfig.MovesAndItems.MoveTypingDists = gameData.moveTable.Waza.Where(m => m.isValid)
                .GetItemDistributionConfig(m => m.type,
                    gameData.GetAllLabels(Constants.TYPE_MESSAGEFILE_NAME),
                    e => true,
                    e => e);

            // Move Damage Category
            distributionsSetupConfig.MovesAndItems.DamageCategoryDists = gameData.moveTable.Waza.Where(m => m.isValid && m.damageType != 0)
                .GetItemDistributionConfig(m => m.damageType,
                    Enumerable.Range(0, 3).ToList(),
                    e => e >= 0 && e < 3,
                    e => string.Empty); // TODO: Damage Category names

            // TM Moves
            distributionsSetupConfig.MovesAndItems.TMMovesDists = gameData.itemTable.WazaMachine.Where(gameData.itemTable.IsTMValid)
                .GetItemDistributionConfig(t => t.wazaNo,
                    gameData.moveTable.Waza,
                    e => e.isValid,
                    e => gameData.GetLabelByIndex(Constants.MOVE_MESSAGEFILE_NAME, e.wazaNo));

            // Move Power
            distributionsSetupConfig.MovesAndItems.MovePowerDists = gameData.moveTable.Waza.Where(m => m.isValid)
                .GetNumericDistributionConfig(m => m.power, AbsoluteBoundaries.Boundary.Power);

            // Move Accuracy
            distributionsSetupConfig.MovesAndItems.MoveAccuracyDists = gameData.moveTable.Waza.Where(m => m.isValid)
                .GetNumericDistributionConfig(m => m.hitPer, AbsoluteBoundaries.Boundary.Accuracy);

            // Move PP
            distributionsSetupConfig.MovesAndItems.MovePPDists = gameData.moveTable.Waza.Where(m => m.isValid)
                .GetNumericDistributionConfig(m => m.basePP, AbsoluteBoundaries.Boundary.Pp);

            // Item Prices
            distributionsSetupConfig.MovesAndItems.ItemPricesDists = gameData.itemTable.Item.Where(i => i.Enabled && i.Purchasable)
                .GetNumericDistributionConfig(i => i.price, AbsoluteBoundaries.Boundary.Price);

            // Pickup Items
            distributionsSetupConfig.MovesAndItems.PickupItemsDists = gameData.pickupTable.PickupItems
                .GetItemDistributionConfig(i => i.ID,
                    gameData.itemTable.Item,
                    e => e.Enabled,
                    e => gameData.GetLabelByIndex(Constants.ITEM_MESSAGEFILE_NAME, e.no));

            // Shop Items TODO: Add more shops?
            distributionsSetupConfig.MovesAndItems.ShopItemsDists = gameData.shopTable.FS.Select(s => s.ItemNo)
                .Concat(gameData.shopTable.FixedShop.Select(s => s.ItemNo))
                .GetItemDistributionConfig(i => i,
                    gameData.itemTable.Item,
                    e => e.Enabled,
                    e => gameData.GetLabelByIndex(Constants.ITEM_MESSAGEFILE_NAME, e.no));

            // Wild Pokémon
            // TODO: UG
            /*for (int file = 0; file < gameData.ugEncounterFiles.Count; file++)
                for (int i = 0; i < gameData.ugEncounterFiles[file].ugEncounters.Count; i++)
                    if (gameData.ugEncounterFiles[file].ugEncounters[i].dexID > 0)
                        instances[(ushort)gameData.ugEncounterFiles[file].ugEncounters[i].dexID]++;*/
            distributionsSetupConfig.Encounters.WildEncountersWildPokemonDists = gameData.encounterTableFiles
                .SelectMany(f => f.table.SelectMany(t => t.GetAllTables().SelectMany(l => l.Select(m => m.monsNo)))
                    .Concat(f.urayama.Select(u => u.monsNo))
                    .Concat(f.mistu.SelectMany(m => new int[] { m.Normal, m.Rare, m.SuperRare }))
                    .Concat(f.safari.Select(s => s.MonsNo)))
                .Where(m => m > 0)
                .GetItemDistributionConfig(m => m,
                    gameData.pokemonDataTable.Data.Where(p => p.formID == 0),
                    e => e.personal.Valid,
                    e => gameData.GetFormName(e.personal.monsno, e.formID));

            // Wild Pokémon Levels
            // TODO: UG
            /*for (int set = 0; set < gameData.ugEncounterLevelSets.Count; set++)
            {
                observations.Add(gameData.ugEncounterLevelSets[set].minLv);
                observations.Add(gameData.ugEncounterLevelSets[set].maxLv);
            }*/
            distributionsSetupConfig.Encounters.WildEncountersWildPokemonLevelsDists = gameData.encounterTableFiles
                .SelectMany(f => f.table.SelectMany(t => t.GetAllTables().SelectMany(l => l.SelectMany(m => new int[] { m.minlv, m.maxlv }))))
                .Where(m => m > 0)
                .GetNumericDistributionConfig(l => l);

            // Trainer Items
            distributionsSetupConfig.Encounters.TrainerItemsDists = gameData.trainerTable.TrainerData.SelectMany(t => t.UseItem)
                .GetItemDistributionConfig(i => i,
                    gameData.itemTable.Item,
                    e => e.Enabled,
                    e => gameData.GetLabelByIndex(Constants.ITEM_MESSAGEFILE_NAME, e.no));

            // Trainer Item Count
            distributionsSetupConfig.Encounters.TrainerItemCountDists = gameData.trainerTable.TrainerData.Select(t => t.UseItem.Count)
                .GetNumericDistributionConfig(c => c);

            // Trainer Pokémon
            distributionsSetupConfig.Encounters.TrainerSpeciesDists = gameData.trainerTable.TrainerData.SelectMany(t => t.Pokes)
                .GetItemDistributionConfig(p => p.MonsNo,
                    gameData.pokemonDataTable.Data.Where(p => p.formID == 0),
                    e => e.personal.Valid,
                    e => gameData.GetFormName(e.personal.monsno, e.formID));

            // Trainer Pokémon Moves (NOTE: logic changed)
            distributionsSetupConfig.Encounters.TrainerMovesDists = gameData.trainerTable.TrainerData.SelectMany(t => t.Pokes)
                .SelectMany(p => p.Moves)
                .Where(m => m > 0)
                .GetItemDistributionConfig(m => m,
                    gameData.moveTable.Waza,
                    e => e.isValid,
                    e => gameData.GetLabelByIndex(Constants.MOVE_MESSAGEFILE_NAME, e.wazaNo));

            // Trainer Pokémon Move Type Bias (NOTE: logic changed)
            distributionsSetupConfig.Encounters.TrainerMoveTypeBiasP = gameData.trainerTable.TrainerData.SelectMany(t => t.Pokes)
                .SelectMany(p => p.Moves.Select(m => (gameData.pokemonDataTable.GetDataForPokemon(p.MonsNo, p.FormNo), gameData.moveTable.Waza[m])))
                .GetOccurrencePercent(pm => pm.Item1.personal.Types.Contains(pm.Item2.type));

            // Trainer Pokémon Count
            distributionsSetupConfig.Encounters.TrainerPokemonCountDists = gameData.trainerTable.TrainerData.Select(t => t.Pokes.Count)
                .GetNumericDistributionConfig(c => c, AbsoluteBoundaries.Boundary.TrainerPokemonCount);

            // Trainer Pokémon Levels
            distributionsSetupConfig.Encounters.TrainerLevelsDists = gameData.trainerTable.TrainerData.SelectMany(t => t.Pokes)
                .GetNumericDistributionConfig(p => p.Level, AbsoluteBoundaries.Boundary.Level);

            // Trainer Pokémon Held Items
            distributionsSetupConfig.Encounters.TrainerHeldItemsDists = gameData.trainerTable.TrainerData.SelectMany(t => t.Pokes)
                .Where(p => p.Item > 0)
                .GetItemDistributionConfig(p => p.Item,
                    gameData.itemTable.Item,
                    e => e.Enabled,
                    e => gameData.GetLabelByIndex(Constants.ITEM_MESSAGEFILE_NAME, e.no));

            // Trainer Pokémon Shininess
            distributionsSetupConfig.Encounters.TrainerShinyP = gameData.trainerTable.TrainerData.SelectMany(t => t.Pokes).GetOccurrencePercent(p => p.IsRare);

            // Trainer Pokémon Natures
            distributionsSetupConfig.Encounters.TrainerHeldItemsDists = gameData.trainerTable.TrainerData.SelectMany(t => t.Pokes)
                .GetItemDistributionConfig(p => p.Seikaku,
                    gameData.GetAllLabels(Constants.NATURE_MESSAGEFILE_NAME),
                    e => true,
                    e => e);

            // Trainer Pokémon Abilities
            distributionsSetupConfig.Encounters.TrainerAbilitiesDists = gameData.trainerTable.TrainerData.SelectMany(t => t.Pokes)
                .GetItemDistributionConfig(p => p.Tokusei,
                    gameData.GetAllLabels(Constants.ABILITY_MESSAGEFILE_NAME),
                    e => true,
                    e => e);

            // Trainer Pokémon IVs
            distributionsSetupConfig.Encounters.TrainerIVsDists = gameData.trainerTable.TrainerData.SelectMany(t => t.Pokes)
                .SelectMany(p => p.IVs)
                .GetNumericDistributionConfig(i => i, AbsoluteBoundaries.Boundary.Iv);

            // Trainer Pokémon EVs
            distributionsSetupConfig.Encounters.TrainerEVsDists = gameData.trainerTable.TrainerData.SelectMany(t => t.Pokes)
                .GetNumericDistributionConfig(p => p.TotalEVs, AbsoluteBoundaries.Boundary.EvTotal);

            // Scripted Pokémon
            distributionsSetupConfig.Misc.RandomScriptedPokemonDists = gameData.evScriptFiles.SelectMany(f => f.Scripts)
                .SelectMany(s => s.Commands)
                .Where(c => c.Arg.Count > 0 && c.Arg[0].data == 322 && c.Arg[2].argType == EvData.ArgType.Float)
                .GetItemDistributionConfig(c => FloatHelper.ConvertToRoundedFloat(c.Arg[2].data),
                    gameData.pokemonDataTable.Data.Where(p => p.formID == 0),
                    e => e.personal.Valid,
                    e => gameData.GetFormName(e.personal.monsno, e.formID));

            // Scripted Items
            distributionsSetupConfig.Misc.RandomScriptedItemsDists = gameData.evScriptFiles.SelectMany(f => f.Scripts)
                .SelectMany(s => s.Commands)
                .Where(c => c.Arg.Count > 0 && c.Arg[0].data == 187 && c.Arg[1].argType == EvData.ArgType.Float)
                .GetItemDistributionConfig(c => FloatHelper.ConvertToRoundedFloat(c.Arg[1].data),
                    gameData.itemTable.Item,
                    e => e.Purchasable,
                    e => gameData.GetLabelByIndex(Constants.ITEM_MESSAGEFILE_NAME, e.no));

            // Type Matchups
            List<string> affinities = ["0x", "1/2x", "1x", "2x"]; // TODO: Affinity names
            distributionsSetupConfig.Misc.TypeMatchupsDists = Enumerable.Range(0, gameData.GetAllLabels(Constants.TYPE_MESSAGEFILE_NAME).Count)
                .SelectMany(o => Enumerable.Range(0, gameData.GetAllLabels(Constants.TYPE_MESSAGEFILE_NAME).Count).Select(d => (o, d)))
                .GetItemDistributionConfig(od => gameData.globalMetadata.GetTypeMatchup(od.o, od.d) switch
                        {
                            0 => 0,
                            2 => 1,
                            4 => 2,
                            8 => 3,
                            _ => -1
                        },
                    affinities,
                    e => true,
                    e => e);

            return distributionsSetupConfig;
        }

        /// <summary>
        /// Finds the particular correlation of typings between two Pokémon.
        /// </summary>
        public static TypingCorrelation CompareTyping(PokemonDataTable.PokemonData p1, PokemonDataTable.PokemonData p2)
        {
            var typing1 = p1.personal.Types;
            var typing2 = p2.personal.Types;

            int matches = 0;
            if (typing1.Contains(typing2[0]))
                matches++;
            if (typing2.Count() > 1 && typing1.Contains(typing2[1]))
                matches++;

            if (matches == 0 && !(typing1.Count() == 1 && typing2.Count() == 1))
                return TypingCorrelation.NoCorrelation;
            else if (matches == 2 || typing1.Count() == 1 && typing2.Count() == 1 && matches == 1)
                return TypingCorrelation.Identical;
            else if (typing1.Count() != typing2.Count())
                return TypingCorrelation.Addition;
            else
                return TypingCorrelation.Swap;
        }

        public enum TypingCorrelation
        {
            Identical,
            Addition,
            Swap,
            NoCorrelation
        }
    }
}
