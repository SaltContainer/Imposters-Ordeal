using ImpostersOrdeal.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ImpostersOrdeal.Distributions;
using static ImpostersOrdeal.ExternalJsonStructs;
using static ImpostersOrdeal.GameDataTypes;
using static ImpostersOrdeal.AbsoluteBoundaries;

namespace ImpostersOrdeal
{
    // TODO: Fix randomizer
    /// <summary>
    /// Responsible for all randomization related logic and execution.
    /// </summary>
    public class Randomizer
    {
        //private readonly MainForm m;
        private readonly Random rng;

        public Randomizer()
        {
            rng = new();
        }

        /// <summary>
        /// Randomizes everything in accordance with current configuration.
        /// </summary>
        public void Randomize(GameDataSet gameData, RandomizerSetupConfig config)
        {
            if (config.Misc.LevelMultiplierEvolutionLevels)
                ScaleEvolutionLevels(gameData, config.Misc.LevelMultiplierCoefficient);
            if (config.Misc.LevelMultiplierLevelUpMoves)
                ScaleLevelUpMoves(gameData, config.Misc.LevelMultiplierCoefficient);
            if (config.Misc.LevelMultiplierWildEncounters)
                ScaleWildEncounters(gameData, config.Misc.LevelMultiplierCoefficient);
            if (config.Misc.LevelMultiplierTrainerLevels)
                ScaleTrainerPokemon(gameData, config.Misc.LevelMultiplierCoefficient);

            if (config.MovesAndItems.MoveTyping)
                RandomizeMoveTyping(gameData, config.MovesAndItems.MoveTypingDist);
            if (config.MovesAndItems.DamageCategory)
                RandomizeDamageCategory(gameData, config.MovesAndItems.DamageCategoryDist);
            if (config.MovesAndItems.TMMoves)
                RandomizeTMMoves(gameData, config.MovesAndItems.TMMovesDist);
            if (config.MovesAndItems.MovePower)
                RandomizePower(gameData, config.MovesAndItems.MovePowerDist);
            if (config.MovesAndItems.MoveAccuracy)
                RandomizeAccuracy(gameData, config.MovesAndItems.MoveAccuracyDist);
            if (config.MovesAndItems.MovePP)
                RandomizePP(gameData, config.MovesAndItems.MovePPDist);
            if (config.MovesAndItems.ItemPrices)
                RandomizePrices(gameData, config.MovesAndItems.ItemPricesDist);
            if (config.MovesAndItems.PickupItems)
                RandomizePickupItems(gameData, config.MovesAndItems.PickupItemsDist);
            if (config.MovesAndItems.ShopItems)
                RandomizeShopItems(gameData, config.MovesAndItems.ShopItemsDist, config.MovesAndItems.ShopItemsPreserveRegularMart);

            if (config.Pokemon.EvolutionRandomDestinations)
                RandomizeEvolutionDestinations(gameData, config.Pokemon.EvolutionDestinationPokemonDist, config.Pokemon.EvolutionBSTLogic);
            if (config.Pokemon.EvolutionLevel)
                RandomizeEvolutionLevels(gameData, config.Pokemon.EvolutionLevelDist);
            if (config.Pokemon.BaseStatsShuffle || config.Pokemon.BaseStats)
                RandomizeStats(gameData, config.Pokemon.BaseStatsDist, config.Pokemon.BaseStatsShuffle, config.Pokemon.BaseStats, config.Pokemon.BaseStatsBSTLogic);
            if (config.Pokemon.PokemonTyping)
                RandomizePokemonTyping(gameData, config.Pokemon.PokemonTypingDist, config.Pokemon.PokemonTypingEvoLogic, config.Pokemon.DoubleTypingP, config.Pokemon.PokemonTypingEvoLogicCorrelationDist);
            if (config.Pokemon.TMCompatibility)
                RandomizeTMCompatibility(gameData, config.Pokemon.TMCompatibilityP, config.Pokemon.TMCompatibilityTypeBiasP, config.Pokemon.TMCompatibilityEvoLogic);
            if (config.Pokemon.WildHeldItems)
                RandomizeWildHeldItems(gameData, config.Pokemon.WildHeldItemsDist);
            if (config.Pokemon.GrowthRate)
                RandomizeGrowthRates(gameData, config.Pokemon.GrowthRateDist);
            if (config.Pokemon.Abilities)
                RandomizePersonalAbilites(gameData, config.Pokemon.AbilitiesDist);
            if (config.Pokemon.CatchRate)
                RandomizeCatchRates(gameData, config.Pokemon.CatchRateDist);
            if (config.Pokemon.InitialFriendship)
                RandomizeInitialFriendship(gameData, config.Pokemon.InitialFriendshipDist);
            if (config.Pokemon.EVYield)
                RandomizeEvYields(gameData, config.Pokemon.EVYieldDist);
            if (config.Pokemon.ExpYield)
                RandomizeExpYields(gameData, config.Pokemon.ExpYieldDist);
            if (config.Pokemon.EggMoves)
                RandomizeEggMoves(gameData, config.Pokemon.EggMovesDist, config.Pokemon.EggMoveTypeBiasP, config.Pokemon.EggMovesCount, config.Pokemon.EggMovesCountDist);
            if (config.Pokemon.LevelUpMoves || config.Pokemon.LevelUpMovesLevel)
                RandomizeLevelUpMoves(gameData, config.Pokemon.LevelUpMoves, config.Pokemon.LevelUpMovesDist, config.Pokemon.LevelUpMovesLevel, config.Pokemon.LevelUpMovesLevelDist, config.Pokemon.LevelUpMovesTypeBiasP, config.Pokemon.LevelUpMovesCount, config.Pokemon.LevelUpMovesCountDist, config.Pokemon.LevelUpMovesSortByPower, config.Pokemon.EvoMovesCountDist);

            if (config.Encounters.WildEncountersRandomPokemon || config.Encounters.WildEncountersWildPokemonLevels)
                RandomizeWildEncounters(gameData, config.Encounters.WildEncountersRandomPokemon, config.Encounters.WildEncountersWildPokemonDist, config.Encounters.WildEncountersWildPokemonLevels, config.Encounters.WildEncountersWildPokemonLevelsDist, config.Encounters.WildEncountersHighLevelLegends, config.Encounters.WildEncountersEvolutionLogic);
            if (config.Encounters.TrainerItems)
                RandomizeTrainerItems(gameData, config.Encounters.TrainerItemsDist, config.Encounters.TrainerItemCount, config.Encounters.TrainerItemCountDist);
            if (config.Encounters.TrainerPokemonCount)
                RandomizeTrainerPokemonCount(gameData, config.Encounters.TrainerPokemonCountDist);
            if (config.Encounters.TrainerLevels)
                RandomizeTrainerPokemonLevels(gameData, config.Encounters.TrainerLevelsDist);
            if (config.Encounters.TrainerRandomPokemon)
                RandomizeTrainerPokemonSpecies(gameData, config.Encounters.TrainerSpeciesDist, config.Encounters.TrainerHighLevelLegends, config.Encounters.TrainerTypeThemes, config.Encounters.TrainerEvolutionLogic);
            if (config.Encounters.TrainerHeldItems)
                RandomizeTrainerPokemonHeldItems(gameData, config.Encounters.TrainerHeldItemsDist, config.Encounters.TrainerHighLevelHeldItems);
            if (config.Encounters.TrainerNatures)
                RandomizeTrainerPokemonNatures(gameData, config.Encounters.TrainerNaturesDist);
            if (config.Encounters.TrainerMovesMode != 0)
                RandomizeTrainerPokemonMoves(gameData, config.Encounters.TrainerMovesMode == 1, config.Encounters.TrainerMovesDist, config.Encounters.TrainerMoveTypeBiasP);
            if (config.Encounters.TrainerShiny)
                RandomizeTrainerPokemonShininess(gameData, config.Encounters.TrainerShinyP);
            if (config.Encounters.TrainerAbilities)
                RandomizeTrainerPokemonAbilities(gameData, config.Encounters.TrainerAbilitiesIncludeUnobtainable, config.Encounters.TrainerAbilitiesDist);
            if (config.Encounters.TrainerIVs)
                RandomizeTrainerPokemonIVs(gameData, config.Encounters.TrainerIVsDist);
            if (config.Encounters.TrainerEVs)
                RandomizeTrainerPokemonEVs(gameData, config.Encounters.TrainerEVsDist);

            if (config.Misc.TypeMatchups)
                RandomizeTypeMatchups(gameData, config.Misc.TypeMatchupsDist);
            if (config.Misc.RandomScriptedPokemon)
                RandomizeScriptedPokemon(gameData, config.Misc.RandomScriptedPokemonDist);
            if (config.Misc.RandomScriptedItems)
                RandomizeScriptedItems(gameData, config.Misc.RandomScriptedItemsDist);
            if (config.Misc.ShuffleText)
                RandomizeText(gameData, config.Misc.ShuffleTextPreserveStringLength);

            if (config.Misc.ShuffleBGM)
                RandomizeMusic(gameData);
        }

        private void RandomizeTypeMatchups(GameDataSet gameData, IDistribution distribution)
        {
            int typeCount = 18;
            for (int o = 0; o < typeCount; o++)
                for (int d = 0; d < typeCount; d++)
                    gameData.globalMetadata.SetTypeMatchup(o, d, ToAffinity(distribution.Next(ToAffinityEnum(gameData.globalMetadata.GetTypeMatchup(o, d)))));

            gameData.SetModified(gameData.globalMetadata.GetType());
        }

        private static int ToAffinityEnum(byte affinity)
        {
            return affinity switch
            {
                0 => 0,
                2 => 1,
                4 => 2,
                8 => 3,
                _ => throw new ArgumentOutOfRangeException(nameof(affinity)),
            };
        }

        private static byte ToAffinity(int affinityEnum)
        {
            return affinityEnum switch
            {
                0 => 0,
                1 => 2,
                2 => 4,
                3 => 8,
                _ => throw new ArgumentOutOfRangeException(nameof(affinityEnum)),
            };
        }

        private void RandomizeMusic(GameDataSet gameData)
        {
            uint[] groupIDs = {
                2944413750, //BGM_BATTLE
                3369806648, //BGM_CONTEST
                1799075776, //BGM_EVENT
                3346466364  //BGM_FIELD
            };
            uint[] ignoreIDs = {
                0,
                748895195,  //NONE
                1454758594, //BA_SILENCE
                595984104,  //B_CON_SILENCE
                4176813980, //EV_SILENCE_100MS
                3652652993, //EV_SILENCE_2000MS
                789285764,  //EV_SILENCE_VS_TRAINER
                191715830,  //FI_SILENCE
                2460189219, //FI_SILENCE_ARC
                1302076938, //SILENCE_BA013
            };

            foreach (Wwise.WwiseObject wo in gameData.delphisMainBank.bankData.objectsByID.Values)
                if (wo is Wwise.MusicSwitchCntr msc)
                {
                    (Wwise.GameSync gs, int i) argumentIdx = msc.arguments.Select((gs, i) => (gs, i)).FirstOrDefault(p => groupIDs.Contains(p.gs.group));
                    if (argumentIdx.gs == null)
                        continue;
                    List<Wwise.Node> nodes = new() { msc.decisionTree };
                    for (int i = 0; i <= argumentIdx.i; i++)
                        nodes = nodes.SelectMany(n => n.nodes).ToList();
                    nodes = nodes.Where(n => !ignoreIDs.Contains(n.key)).ToList();
                    while (nodes.Any(n => n.childrenCount > 0))
                        nodes = nodes.SelectMany(n => n.nodes).ToList();
                    List<uint> anis = nodes.Select(n => n.audioNodeId).Distinct().ToList();
                    foreach (Wwise.Node n in nodes)
                        n.audioNodeId = rng.RandomElement(anis);
                }

            gameData.SetModified(gameData.delphisMainBank.GetType());
        }

        private void ScaleTrainerPokemon(GameDataSet gameData, double coefficient)
        {
            foreach (var trainer in gameData.trainerTable.TrainerData)
                foreach (var trainerPokemon in trainer.Pokes)
                    if (IsWithin(Boundary.Level, trainerPokemon.Level))
                        trainerPokemon.Level = (byte)Conform(Boundary.Level, (int)(trainerPokemon.Level * coefficient));

            gameData.SetModified(gameData.trainerTable.GetType());
        }

        private void ScaleWildEncounters(GameDataSet gameData, double coefficient)
        {
            foreach (var encounterTableFile in gameData.encounterTableFiles)
                foreach (var encounterTable in encounterTableFile.table)
                {
                    var encounters = encounterTable.GetAllTables().SelectMany(t => t);
                    foreach (var encounter in encounters)
                        if (IsWithin(Boundary.Level, (int)encounter.GetAvgLevel()))
                        {
                            encounter.minlv = Conform(Boundary.Level, (int)(encounter.minlv * coefficient));
                            encounter.maxlv = Conform(Boundary.Level, (int)(encounter.maxlv * coefficient));
                        }
                }

            // TODO: UG Encounters
            /*foreach (UgEncounterLevelSet ugEncounterLevelSet in gameData.ugEncounterLevelSets)
                if (IsWithin(Boundary.Level, (int)ugEncounterLevelSet.GetAvgLevel()))
                {
                    ugEncounterLevelSet.minLv = Conform(Boundary.Level, (int)(ugEncounterLevelSet.minLv * coefficient));
                    ugEncounterLevelSet.maxLv = Conform(Boundary.Level, (int)(ugEncounterLevelSet.maxLv * coefficient));
                }*/

            gameData.SetModified(gameData.encounterTableFiles.GetType());
            //gameData.SetModified(GameDataSet.DataField.UgEncounterLevelSets);
        }

        private void ScaleLevelUpMoves(GameDataSet gameData, double coefficient)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
                foreach (var levelUpMove in pokemon.levelUpMoves.moves)
                    if (IsWithin(Boundary.Level, levelUpMove.level))
                        levelUpMove.level = (ushort)Conform(Boundary.Level, (int)(levelUpMove.level * coefficient));

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        private void ScaleEvolutionLevels(GameDataSet gameData, double coefficient)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
            {
                foreach (var evolutionPath in pokemon.evolutionPaths.paths)
                    if (IsWithin(Boundary.Level, evolutionPath.level))
                        evolutionPath.level = (ushort)Conform(Boundary.Level, (int)(evolutionPath.level * coefficient));
                pokemon.pastEvoLvs = (0, 0);
                pokemon.nextEvoLvs = (ushort.MaxValue, ushort.MaxValue);
                pokemon.pastPokemon = new();
                pokemon.nextPokemon = new();
                pokemon.inferiorForms = new();
                pokemon.superiorForms = new();
            }

            // TODO: Setup lists in data
            //DataParser.SetFamilies();
            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        private void RandomizeText(GameDataSet gameData, bool preserveStringLength)
        {
            foreach (var (lang, messageFileSet) in gameData.messageFileTable.messageFilesByLanguage)
                foreach (var messageFile in messageFileSet)
                {
                    var labelDatas = messageFile.labelDataArray;
                    labelDatas.ForEach(l => l.wordDataArray.Last().eventID = MessageEnumData.MsgEventID.End);

                    if (!preserveStringLength)
                    {
                        rng.Shuffle(labelDatas);
                        messageFile.SetStrings(labelDatas);
                        continue;
                    }

                    List<int>[] indexes = new List<int>[10];
                    for (int i = 0; i < indexes.Length; i++)
                        indexes[i] = new();
                    int[] targetLists = new int[labelDatas.Count];

                    for (int i = 0; i < labelDatas.Count; i++)
                    {
                        int currentList;
                        if (labelDatas[i].wordDataArray.Count == 1)
                        {
                            if (labelDatas[i].GetString().Length < 3)
                                currentList = 0;
                            else if (labelDatas[i].GetString().Length < 8)
                                currentList = 1;
                            else if (labelDatas[i].GetString().Length < 21)
                                currentList = 2;
                            else if (labelDatas[i].GetString().Length < 55)
                                currentList = 3;
                            else
                                currentList = 4;
                        }
                        else if (labelDatas[i].wordDataArray.Count < 3)
                            currentList = 5;
                        else if (labelDatas[i].wordDataArray.Count < 8)
                            currentList = 6;
                        else if (labelDatas[i].wordDataArray.Count < 21)
                            currentList = 7;
                        else if (labelDatas[i].wordDataArray.Count < 55)
                            currentList = 8;
                        else
                            currentList = 9;

                        targetLists[i] = currentList;
                        indexes[currentList].Add(i);
                    }

                    var oldLabelDatas = new List<MsbtData.LabelData>();
                    oldLabelDatas.AddRange(labelDatas);

                    List<int>[] newIndexes = new List<int>[10];
                    for (int i = 0; i < newIndexes.Length; i++)
                    {
                        newIndexes[i] = new();
                        newIndexes[i].AddRange(indexes[i]);
                        rng.Shuffle(newIndexes[i]);
                    }

                    for (int i = 0; i < labelDatas.Count; i++)
                        labelDatas[newIndexes[targetLists[i]][indexes[targetLists[i]].IndexOf(i)]] = oldLabelDatas[i];

                    messageFile.SetStrings(labelDatas);
                }

            gameData.SetModified(gameData.messageFileTable.GetType());
        }

        private void RandomizeScriptedItems(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var evScript in gameData.evScriptFiles)
                foreach (var script in evScript.Scripts)
                    foreach (var command in script.Commands)
                        if (command.Arg.Count > 0 && command.Arg[0].data == 187)
                        {
                            var itemNo = FloatHelper.ConvertToRoundedFloat(command.Arg[1].data);
                            if (itemNo >= 0 && itemNo < gameData.itemTable.Item.Count && gameData.itemTable.Item[itemNo].Enabled)
                            {
                                command.Arg[1].argType = EvData.ArgType.Float;
                                command.Arg[1].data = FloatHelper.ConvertToInt(distribution.Next(itemNo));
                            }
                        }

            gameData.SetModified(gameData.evScriptFiles.GetType());
        }

        private void RandomizeScriptedPokemon(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var evScript in gameData.evScriptFiles)
                foreach (var script in evScript.Scripts)
                    foreach(var command in script.Commands)
                        if (command.Arg.Count > 0 && command.Arg[0].data == 322)
                        {
                            var monsNo = FloatHelper.ConvertToRoundedFloat(command.Arg[2].data);
                            command.Arg[2].argType = EvData.ArgType.Float;
                            command.Arg[2].data = FloatHelper.ConvertToInt(distribution.Next(monsNo));
                        }

            gameData.SetModified(gameData.evScriptFiles.GetType());
        }

        private void RandomizeTrainerPokemonEVs(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var trainer in gameData.trainerTable.TrainerData)
                foreach (var trainerPokemon in trainer.Pokes)
                {
                    var evs = new byte[6];
                    int evTotal = trainerPokemon.TotalEVs;
                    if (IsWithin(Boundary.EvTotal, evTotal))
                        evTotal = Conform(Boundary.EvTotal, distribution.Next(evTotal));

                    while (evTotal > 0)
                    {
                        int index = rng.Next(evs.Length);
                        int room = (int)GetBoundaries(Boundary.Ev)[2] - evs[index];
                        int add = rng.Next(Math.Min(room, 1), Math.Min(room, evTotal));
                        evs[index] += (byte)add;
                        evTotal -= add;
                    }

                    trainerPokemon.EffortHp = evs[0];
                    trainerPokemon.EffortAtk = evs[1];
                    trainerPokemon.EffortDef = evs[2];
                    trainerPokemon.EffortSpAtk = evs[3];
                    trainerPokemon.EffortSpDef = evs[4];
                    trainerPokemon.EffortAgi = evs[5];
                }

            gameData.SetModified(gameData.trainerTable.GetType());
        }

        private void RandomizeTrainerPokemonIVs(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var trainer in gameData.trainerTable.TrainerData)
                foreach (var trainerPokemon in trainer.Pokes)
                    if (IsWithin(Boundary.Iv, (int)trainerPokemon.AverageIVs))
                    {
                        trainerPokemon.TalentHp = (byte)Conform(Boundary.Iv, distribution.Next(trainerPokemon.TalentHp));
                        trainerPokemon.TalentAtk = (byte)Conform(Boundary.Iv, distribution.Next(trainerPokemon.TalentAtk));
                        trainerPokemon.TalentDef = (byte)Conform(Boundary.Iv, distribution.Next(trainerPokemon.TalentDef));
                        trainerPokemon.TalentSpAtk = (byte)Conform(Boundary.Iv, distribution.Next(trainerPokemon.TalentSpAtk));
                        trainerPokemon.TalentSpDef = (byte)Conform(Boundary.Iv, distribution.Next(trainerPokemon.TalentSpDef));
                        trainerPokemon.TalentAgi = (byte)Conform(Boundary.Iv, distribution.Next(trainerPokemon.TalentAgi));
                    }

            gameData.SetModified(gameData.trainerTable.GetType());
        }

        private void RandomizeTrainerPokemonLevels(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var trainer in gameData.trainerTable.TrainerData)
                foreach (var trainerPokemon in trainer.Pokes)
                    if (IsWithin(Boundary.Level, trainerPokemon.Level))
                        trainerPokemon.Level = (byte)Conform(Boundary.Level, distribution.Next(trainerPokemon.Level));

            gameData.SetModified(gameData.trainerTable.GetType());
        }

        private void RandomizeTrainerPokemonCount(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var trainer in gameData.trainerTable.TrainerData)
            {
                var trainerPokemon = trainer.Pokes;
                int trainerPokemonCount = trainerPokemon.Count;
                if (IsWithin(Boundary.TrainerPokemonCount, trainerPokemonCount))
                    trainerPokemonCount = Conform(Boundary.TrainerPokemonCount, distribution.Next(trainerPokemon.Count));

                while (trainerPokemon.Count < trainerPokemonCount)
                {
                    if (trainerPokemon.Count == 0)
                    {
                        trainerPokemon.Add(new());
                        continue;
                    }

                    trainerPokemon.Add(rng.RandomElement(trainerPokemon).Clone());
                }

                while (trainerPokemon.Count > trainerPokemonCount)
                    trainerPokemon.RemoveAt(rng.Next(trainerPokemon.Count));
            }

            gameData.SetModified(gameData.trainerTable.GetType());
        }

        private void RandomizeTrainerPokemonAbilities(GameDataSet gameData, bool includeUnobtainable, IDistribution distribution)
        {
            foreach (var trainer in gameData.trainerTable.TrainerData)
                foreach (var trainerPokemon in trainer.Pokes)
                {
                    if (includeUnobtainable)
                    {
                        trainerPokemon.Tokusei = (ushort)distribution.Next(trainerPokemon.Tokusei);
                        continue;
                    }

                    trainerPokemon.Tokusei = rng.RandomElement(gameData.pokemonDataTable.GetDataForPokemon(trainerPokemon.MonsNo, trainerPokemon.FormNo).personal.Abilities);
                }

            gameData.SetModified(gameData.trainerTable.GetType());
        }

        private void RandomizeTrainerPokemonShininess(GameDataSet gameData, double shinyP)
        {
            foreach (var trainer in gameData.trainerTable.TrainerData)
                foreach (var trainerPokemon in trainer.Pokes)
                    trainerPokemon.IsRare = rng.Percent(shinyP);

            gameData.SetModified(gameData.trainerTable.GetType());
        }

        private void RandomizeTrainerPokemonNatures(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var trainer in gameData.trainerTable.TrainerData)
                foreach (var trainerPokemon in trainer.Pokes)
                    trainerPokemon.Seikaku = (byte)distribution.Next(trainerPokemon.Seikaku);

            gameData.SetModified(gameData.trainerTable.GetType());
        }

        private void RandomizeTrainerPokemonHeldItems(GameDataSet gameData, IDistribution distribution, bool levelLogic)
        {
            foreach (var trainer in gameData.trainerTable.TrainerData)
                foreach (var trainerPokemon in trainer.Pokes)
                {
                    if (levelLogic && !rng.Percent(trainerPokemon.Level))
                        continue;

                    trainerPokemon.Item = (ushort)distribution.Next(trainerPokemon.Item);
                }

            gameData.SetModified(gameData.trainerTable.GetType());
        }

        private void RandomizeTrainerPokemonMoves(GameDataSet gameData, bool setToLevelUpMoves, IDistribution distribution, double typeBiasP)
        {
            foreach (var trainer in gameData.trainerTable.TrainerData)
                foreach (var trainerPokemon in trainer.Pokes)
                {
                    var moves = trainerPokemon.Moves;
                    var pokemon = gameData.pokemonDataTable.GetDataForPokemon(trainerPokemon.MonsNo, trainerPokemon.FormNo);

                    if (setToLevelUpMoves)
                    {
                        var newMoves = pokemon.levelUpMoves.moves.Where(l => l.level <= trainerPokemon.Level).TakeLast(4).Select(l => l.move);
                        if (newMoves.Count() < 4)
                            newMoves = newMoves.Concat(Enumerable.Repeat<ushort>(0, 4 - newMoves.Count()));

                        trainerPokemon.Moves = newMoves.ToArray();
                        continue;
                    }

                    for (int i=0; i<moves.Count(); i++)
                    {
                        moves[i] = (ushort)distribution.Next(moves[i]);
                        if (rng.Percent(typeBiasP))
                            while (!pokemon.personal.Types.Contains(gameData.moveTable.Waza[moves[i]].type))
                                moves[i] = (ushort)distribution.Next(moves[i]);
                    }

                    trainerPokemon.Moves = moves;
                }

            gameData.SetModified(gameData.trainerTable.GetType());
        }

        private void RandomizeTrainerPokemonSpecies(GameDataSet gameData, IDistribution distribution, bool legendLogic, bool typeThemes, bool evolveLogic)
        {
            var legendaries = gameData.encounterTableFiles[0].legendpoke;
            foreach (var trainer in gameData.trainerTable.TrainerData)
            {
                int typing = -1;
                if (trainer.TypeID >= 0 && trainer.TypeID < gameData.trainerTable.TrainerType.Count)
                    typing = gameData.trainerTable.TrainerType[trainer.TypeID].GetTypeTheme();

                foreach (var trainerPokemon in trainer.Pokes)
                {
                    bool acceptLegendary = !legendLogic || rng.Percent(trainerPokemon.Level);
                    var pokemon = gameData.pokemonDataTable.GetDataForPokemon(trainerPokemon.MonsNo, trainerPokemon.FormNo);
                    do
                    {
                        pokemon = rng.RandomElement(gameData.pokemonDataTable.GetAllFormsForPokemon(distribution.Next(pokemon.personal.monsno)));

                        if (evolveLogic)
                            pokemon = FindStage(pokemon, trainerPokemon.Level, false);
                    } while ((!pokemon.personal.Valid) ||
                        (typeThemes && typing != -1 && !pokemon.personal.Types.Contains((byte)typing)) ||
                        (!acceptLegendary && legendaries.Any(l => l.monsNo == pokemon.personal.monsno && l.formNo == pokemon.formID)));

                    trainerPokemon.MonsNo = pokemon.personal.monsno;
                    trainerPokemon.FormNo = pokemon.formID;
                    trainerPokemon.Sex = 3;
                }
            }

            gameData.SetModified(gameData.trainerTable.GetType());
        }

        private void RandomizeTrainerItems(GameDataSet gameData, IDistribution itemDistribution, bool randomizeItemCount, IDistribution itemCountdistribution)
        {
            foreach (var trainer in gameData.trainerTable.TrainerData)
            {
                var items = trainer.UseItem;
                if (randomizeItemCount)
                {
                    int itemCount = itemCountdistribution.Next(items.Count, 0, 4);
                    while (items.Count < itemCount)
                    {
                        if (items.Count == 0)
                        {
                            items.Add((ushort)itemDistribution.Next(1));
                            continue;
                        }

                        items.Add(rng.RandomElement(items));
                    }
                    while (items.Count > itemCount)
                        items.RemoveAt(rng.Next(items.Count));
                }

                for (int i = 0; i < items.Count; i++)
                    items[i] = (ushort)itemDistribution.Next(items[i]);

                trainer.UseItem = items;
                trainer.ItemFlag = true;
            }

            gameData.SetModified(gameData.trainerTable.GetType());
        }

        private void RandomizeWildEncounters(GameDataSet gameData, bool randomizeSpecies, IDistribution speciesDistribution, bool randomizeLevels, IDistribution levelDistribution, bool legendLogic, bool evolveLogic)
        {
            // TODO: Modded options
            //bool randomizeEncounterTableFormIDs = gameData.Uint16EncounterTables();
            //bool ugVersionsUnbounded = gameData.UgVersionsUnbounded();
            //bool uint16UgTables = gameData.Uint16UgTables();
            //bool randomizeUgEncounterTableFormIDs = ugVersionsUnbounded || uint16UgTables;

            bool randomizeEncounterTableFormIDs = false;
            bool ugVersionsUnbounded = false;
            bool uint16UgTables = false;
            bool randomizeUgEncounterTableFormIDs = false;

            foreach (var encounterTableFile in gameData.encounterTableFiles)
            {
                var legendaries = encounterTableFile.legendpoke;

                foreach (var encounterTable in encounterTableFile.table)
                {
                    RandomizeEncounterList(gameData, encounterTable.ground_mons, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                    RandomizeEncounterList(gameData, encounterTable.tairyo, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                    RandomizeEncounterList(gameData, encounterTable.day, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                    RandomizeEncounterList(gameData, encounterTable.night, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                    RandomizeEncounterList(gameData, encounterTable.swayGrass, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                    RandomizeEncounterList(gameData, encounterTable.gbaRuby, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                    RandomizeEncounterList(gameData, encounterTable.gbaSapp, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                    RandomizeEncounterList(gameData, encounterTable.gbaEme, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                    RandomizeEncounterList(gameData, encounterTable.gbaFire, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                    RandomizeEncounterList(gameData, encounterTable.gbaLeaf, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                    RandomizeEncounterList(gameData, encounterTable.water_mons, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                    RandomizeEncounterList(gameData, encounterTable.boro_mons, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                    RandomizeEncounterList(gameData, encounterTable.ii_mons, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                    RandomizeEncounterList(gameData, encounterTable.sugoi_mons, legendaries, randomizeSpecies, speciesDistribution, randomizeLevels, levelDistribution, legendLogic, evolveLogic, randomizeEncounterTableFormIDs);
                }

                if (randomizeSpecies)
                {
                    // TODO: Yes this is honey trees but re-check all of these (honeytree tables are for the table rates)
                    foreach (var honeyTreeEncounter in encounterTableFile.mistu)
                    {
                        honeyTreeEncounter.Normal = speciesDistribution.Next(honeyTreeEncounter.Normal);
                        honeyTreeEncounter.Rare = speciesDistribution.Next(honeyTreeEncounter.Rare);
                        honeyTreeEncounter.SuperRare = speciesDistribution.Next(honeyTreeEncounter.SuperRare);
                    }

                    foreach (var safariEncounter in encounterTableFile.safari)
                        safariEncounter.MonsNo = speciesDistribution.Next(safariEncounter.MonsNo);

                    foreach (var trophyGardenEncounter in encounterTableFile.urayama)
                        trophyGardenEncounter.monsNo = speciesDistribution.Next(trophyGardenEncounter.monsNo);
                }
            }

            // TODO: UG Encounter stuff
            /*if (randomizeSpecies)
                foreach (UgEncounterFile ugEncounterFile in gameData.ugEncounterFiles)
                    for (int i = 0; i < ugEncounterFile.ugEncounters.Count; i++)
                    {
                        ugEncounterFile.ugEncounters[i].dexID = speciesDistribution.Next((ushort)ugEncounterFile.ugEncounters[i].dexID);
                        if (randomizeUgEncounterTableFormIDs)
                        {
                            ushort formID = (ushort)rng.Next(gameData.dexEntries[(ushort)ugEncounterFile.ugEncounters[i].dexID].forms.Count);
                            if (gameData.dexEntries[(ushort)ugEncounterFile.ugEncounters[i].dexID].forms.Any(u => u.IsValid()))
                                formID = (ushort)GetRandom(gameData.dexEntries[(ushort)ugEncounterFile.ugEncounters[i].dexID].forms.Where(p => p.IsValid()).ToList()).formID;
                            if (ugVersionsUnbounded)
                                ugEncounterFile.ugEncounters[i].version = formID;
                            if (uint16UgTables)
                                ugEncounterFile.ugEncounters[i].dexID += formID << 16;
                        }
                    }*/

            /*if (randomizeSpecies)
                foreach (UgSpecialEncounter ugSpecialEncounter in gameData.ugSpecialEncounters)
                    ugSpecialEncounter.dexID = speciesDistribution.Next(ugSpecialEncounter.dexID);*/

            /*if (randomizeLevels)
                foreach (UgEncounterLevelSet ugEncounterLevelSet in gameData.ugEncounterLevelSets)
                    if (IsWithin(Boundary.Level, (int)ugEncounterLevelSet.GetAvgLevel()))
                    {
                        ugEncounterLevelSet.minLv = Conform(Boundary.Level, levelDistribution.Next(ugEncounterLevelSet.minLv));
                        ugEncounterLevelSet.maxLv = Conform(Boundary.Level, levelDistribution.Next(ugEncounterLevelSet.maxLv));
                    }*/

            gameData.SetModified(gameData.encounterTableFiles.GetType());
            //gameData.SetModified(GameDataSet.DataField.UgEncounterFiles);
            //gameData.SetModified(GameDataSet.DataField.UgEncounterLevelSets);
            //gameData.SetModified(GameDataSet.DataField.UgSpecialEncounters);

            // TODO: External JSON encounters
            /*if (gameData.externalStarters != null)
            {
                foreach ((string _, Starter starter) in gameData.externalStarters)
                {
                    if (randomizeLevels && IsWithin(Boundary.Level, starter.level))
                    {
                        starter.level = Conform(Boundary.Level, levelDistribution.Next(starter.level));

                        if (evolveLogic)
                        {
                            Pokemon p = FindStage(gameData.GetPokemon(starter.monsNo, starter.formNo), starter.level, true);
                            starter.monsNo = p.dexID;
                            starter.formNo = p.formID;
                        }
                    }

                    if (randomizeSpecies)
                    {
                        bool acceptLegendary = !legendLogic || P(starter.level);
                        Func<Pokemon, Pokemon> resolveStage = evolveLogic ? p => FindStage(p, starter.level, true) : p => p;

                        do
                        {
                            starter.monsNo = speciesDistribution.Next(starter.monsNo);
                            starter.formNo = rng.Next(gameData.dexEntries[starter.monsNo].forms.Count);
                            Pokemon p = resolveStage(gameData.GetPokemon(starter.monsNo, starter.formNo));
                            starter.monsNo = p.dexID;
                            starter.formNo = p.formID;
                        } while (!gameData.GetPokemon(starter.monsNo, starter.formNo).IsValid() ||
                            !acceptLegendary && legendaryDexIDs.Contains(starter.monsNo));
                    }
                }
                gameData.SetModified(GameDataSet.DataField.ExternalStarters);
            }

            if (gameData.externalHoneyTrees != null)
            {
                foreach ((string _, HoneyTreeZone honeyTree) in gameData.externalHoneyTrees)
                    foreach (HoneyTreeSlot slot in honeyTree.slots)
                    {
                        if (randomizeLevels && IsWithin(Boundary.Level, (int)slot.GetAvgLevel()))
                        {
                            slot.minlv = Conform(Boundary.Level, levelDistribution.Next(slot.minlv));
                            slot.maxlv = Conform(Boundary.Level, levelDistribution.Next(slot.maxlv));

                            if (slot.minlv > slot.maxlv)
                                (slot.maxlv, slot.minlv) = (slot.minlv, slot.maxlv);

                            if (evolveLogic)
                            {
                                Pokemon p = FindStage(gameData.GetPokemon(slot.monsNo, slot.formNo), (int)slot.GetAvgLevel(), true);
                                slot.monsNo = p.dexID;
                                slot.formNo = p.formID;
                            }
                        }

                        if (randomizeSpecies)
                        {
                            bool acceptLegendary = !legendLogic || P(slot.GetAvgLevel());
                            Func<Pokemon, Pokemon> resolveStage = evolveLogic ? p => FindStage(p, (int)slot.GetAvgLevel(), true) : p => p;

                            do
                            {
                                slot.monsNo = speciesDistribution.Next(slot.monsNo);
                                slot.formNo = rng.Next(gameData.dexEntries[slot.monsNo].forms.Count);
                                Pokemon p = resolveStage(gameData.GetPokemon(slot.monsNo, slot.formNo));
                                slot.monsNo = p.dexID;
                                slot.formNo = p.formID;
                            } while (!gameData.GetPokemon(slot.monsNo, slot.formNo).IsValid() ||
                                !acceptLegendary && legendaryDexIDs.Contains(slot.monsNo));
                        }
                    }
                gameData.SetModified(GameDataSet.DataField.ExternalHoneyTrees);
            }*/
        }

        /// <summary>
        /// Randomizes a list of Encounter objects.
        /// </summary>
        private void RandomizeEncounterList(GameDataSet gameData, List<FieldEncountTable.Sheettable.MonsLv> encounters, List<FieldEncountTable.Sheetlegendpoke> legendaries, bool randomizeSpecies, IDistribution speciesDistribution, bool randomizeLevels, IDistribution levelDistribution, bool legendLogic, bool evolveLogic, bool randomizeFormIDs)
        {
            foreach (var encounter in encounters)
            {
                if (randomizeLevels && IsWithin(Boundary.Level, (int)encounter.GetAvgLevel()))
                {
                    encounter.minlv = Conform(Boundary.Level, levelDistribution.Next(encounter.minlv));
                    encounter.maxlv = Conform(Boundary.Level, levelDistribution.Next(encounter.maxlv));

                    if (encounter.minlv > encounter.maxlv)
                        (encounter.maxlv, encounter.minlv) = (encounter.minlv, encounter.maxlv);

                    if (evolveLogic)
                    {
                        if (randomizeFormIDs)
                        {
                            var p = FindStage(gameData.pokemonDataTable.GetDataForPokemon((ushort)encounter.monsNo, encounter.monsNo >> 16), (int)encounter.GetAvgLevel(), true);
                            encounter.monsNo = p.personal.monsno + (p.formID << 16);
                        }
                        else
                            encounter.monsNo = FindStage(gameData.pokemonDataTable.Data[(ushort)encounter.monsNo], (int)encounter.GetAvgLevel(), true).personal.monsno;
                    }
                }

                if (randomizeSpecies)
                {
                    bool acceptLegendary = !legendLogic || rng.Percent(encounter.GetAvgLevel());
                    Func<PokemonDataTable.PokemonData, PokemonDataTable.PokemonData> resolveStage = evolveLogic ? p => FindStage(p, (int)encounter.GetAvgLevel(), true) : p => p;

                    do
                    {
                        encounter.monsNo = speciesDistribution.Next((ushort)encounter.monsNo);
                        if (randomizeFormIDs)
                        {
                            encounter.monsNo += rng.Next(gameData.pokemonDataTable.GetAllFormsForPokemon(encounter.monsNo).Count) << 16;
                            var p = resolveStage(gameData.pokemonDataTable.GetDataForPokemon((ushort)encounter.monsNo, encounter.monsNo >> 16));
                            encounter.monsNo = p.personal.monsno + (p.formID << 16);
                        }
                        else
                            encounter.monsNo = resolveStage(gameData.pokemonDataTable.Data[(ushort)encounter.monsNo]).personal.monsno;
                    } while (!gameData.pokemonDataTable.GetDataForPokemon((ushort)encounter.monsNo, encounter.monsNo >> 16).personal.Valid ||
                        !acceptLegendary && legendaries.Any(l => (!randomizeFormIDs && l.monsNo == encounter.monsNo) ||
                        (randomizeFormIDs && l.monsNo == (ushort)encounter.monsNo && l.formNo == (encounter.monsNo >> 16))));
                }
            }
        }

        /// <summary>
        /// Finds the evolution stage a certain pokemon is likely to be at for the specified level.
        /// </summary>
        private PokemonDataTable.PokemonData FindStage(PokemonDataTable.PokemonData pokemon, int level, bool wild)
        {
            //(wildLevel, trainerLevel)
            int pastEvoLevel = wild ? pokemon.pastEvoLvs.Item1 : pokemon.pastEvoLvs.Item2;
            if (pastEvoLevel > level)
                return FindStage(rng.RandomElement(pokemon.pastPokemon), level, wild);
            int nextEvoLevel = wild ? pokemon.nextEvoLvs.Item1 : pokemon.nextEvoLvs.Item2;
            if (nextEvoLevel <= level)
                return FindStage(rng.RandomElement(pokemon.nextPokemon), level, wild);
            return pokemon;
        }

        private void RandomizeShopItems(GameDataSet gameData, IDistribution distribution, bool preserveRegularMarts)
        {
            if (!preserveRegularMarts)
                foreach (var item in gameData.shopTable.FS)
                    item.ItemNo = (ushort)distribution.Next(item.ItemNo);

            foreach (var item in gameData.shopTable.FixedShop)
                item.ItemNo = (ushort)distribution.Next(item.ItemNo);

            foreach (var item in gameData.shopTable.BPShop)
                item.ItemNo = (ushort)distribution.Next(item.ItemNo);

            gameData.SetModified(gameData.shopTable.GetType());
        }

        private void RandomizePickupItems(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var item in gameData.pickupTable.PickupItems)
                item.ID = (ushort)distribution.Next(item.ID);

            gameData.SetModified(gameData.pickupTable.GetType());
        }

        private void RandomizePrices(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var item in gameData.itemTable.Item)
                if (IsWithin(Boundary.Price, item.price))
                    item.price = Conform(Boundary.Price, distribution.Next(item.price));

            gameData.SetModified(gameData.itemTable.GetType());
        }

        private void RandomizePP(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var move in gameData.moveTable.Waza)
                if (IsWithin(Boundary.Pp, move.basePP))
                    move.basePP = (byte)Conform(Boundary.Pp, distribution.Next(move.basePP));

            gameData.SetModified(gameData.moveTable.GetType());
        }

        private void RandomizeAccuracy(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var move in gameData.moveTable.Waza)
                if (IsWithin(Boundary.Accuracy, move.hitPer))
                    move.hitPer = (byte)Conform(Boundary.Accuracy, distribution.Next(move.hitPer));

            gameData.SetModified(gameData.moveTable.GetType());
        }

        private void RandomizePower(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var move in gameData.moveTable.Waza)
                if (IsWithin(Boundary.Power, move.power))
                    move.power = (byte)Conform(Boundary.Power, distribution.Next(move.power));

            gameData.SetModified(gameData.moveTable.GetType());
        }

        private void RandomizeTMMoves(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var tm in gameData.itemTable.WazaMachine)
                tm.wazaNo = distribution.Next(tm.wazaNo);

            gameData.SetModified(gameData.itemTable.GetType());
        }

        private void RandomizeDamageCategory(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var move in gameData.moveTable.Waza)
                if (move.damageType != 0)
                    move.damageType = (byte)distribution.Next(move.damageType);

            gameData.SetModified(gameData.moveTable.GetType());
        }

        private void RandomizeMoveTyping(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var move in gameData.moveTable.Waza)
                move.type = (byte)distribution.Next(move.type);

            gameData.SetModified(gameData.moveTable.GetType());
        }

        private void RandomizeLevelUpMoves(GameDataSet gameData, bool randomizeMoves, IDistribution moveDistribution, bool randomizeLevels, IDistribution levelDistribution, double typeBiasP, bool randomizeMoveCount, IDistribution moveCountDistribution, bool sortByPower, IDistribution evolutionMoveCountDistribution)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
            {
                var levelUpMoves = pokemon.levelUpMoves.moves;

                if (!IsWithin(Boundary.LevelUpMoveCount, levelUpMoves.Count))
                    continue;

                if (randomizeMoveCount)
                {
                    int moveCount = levelUpMoves.Count;
                        moveCount = Conform(Boundary.LevelUpMoveCount, moveCountDistribution.Next(levelUpMoves.Count));
                    while (levelUpMoves.Count < moveCount)
                    {
                        if (levelUpMoves.Count == 0)
                        {
                            levelUpMoves.Add(new PokemonDataTable.PokemonData.SheetWazaOboe.LearnedMove()
                            {
                                level = (ushort)Conform(Boundary.Level, levelDistribution.Next(1)),
                                move = (ushort)moveDistribution.Next(1),
                            });
                            continue;
                        }
                        levelUpMoves.Add(rng.RandomElement(levelUpMoves).Clone());
                    }
                    while (levelUpMoves.Count > moveCount)
                        levelUpMoves.RemoveAt(rng.Next(levelUpMoves.Count));
                }

                if (randomizeMoves)
                    foreach (var move in levelUpMoves)
                    {
                        move.move = (ushort)moveDistribution.Next(move.move);
                        if (rng.Percent(typeBiasP))
                            while (!pokemon.personal.Types.Contains(gameData.moveTable.Waza[move.move].type))
                                move.move = (ushort)moveDistribution.Next(move.move);
                    }

                if (randomizeLevels)
                {
                    foreach (var move in levelUpMoves)
                        if (IsWithin(Boundary.Level, move.level))
                            move.level = (ushort)Conform(Boundary.Level, levelDistribution.Next(move.level));

                    if (pokemon.pastPokemon.Count > 0)
                    {
                        int evolutionMoveCount = evolutionMoveCountDistribution.Next(0);
                        var evolutionMoves = new PokemonDataTable.PokemonData.SheetWazaOboe.LearnedMove[evolutionMoveCount];
                        for (int i = 0; i < evolutionMoveCount; i++)
                            evolutionMoves[i] = rng.RandomElement(levelUpMoves);
                        foreach (var move in evolutionMoves)
                            move.level = 0;
                    }
                }

                var firstMove = rng.RandomElement(levelUpMoves);
                firstMove.level = 1;
                var attacks = levelUpMoves.Where(l => IsWithin(Boundary.Power, gameData.moveTable.Waza[l.move].power)).ToList();
                if (attacks.Count > 0)
                {
                    var target = rng.RandomElement(attacks);
                    (target.move, firstMove.move) = (firstMove.move, target.move);
                }
                else
                    while (!IsWithin(Boundary.Power, gameData.moveTable.Waza[firstMove.move].power))
                        firstMove.move = (ushort)rng.RandomElement(gameData.moveTable.Waza).wazaNo;

                levelUpMoves.Sort((m1, m2) => m1.level - m2.level);

                if (sortByPower)
                {
                    var attackMoves = new List<PokemonDataTable.PokemonData.SheetWazaOboe.LearnedMove>();
                    var attackMoveLevels = new List<ushort>();
                    for (int i = 0; i < levelUpMoves.Count; i++)
                    {
                        if (levelUpMoves[i].level == 0 || !IsWithin(Boundary.Power, gameData.moveTable.Waza[levelUpMoves[i].move].power))
                            continue;
                        attackMoves.Add(levelUpMoves[i]);
                        attackMoveLevels.Add(levelUpMoves[i].level);
                    }
                    attackMoves.Sort((m1, m2) => gameData.moveTable.Waza[m1.move].power - gameData.moveTable.Waza[m2.move].power);
                    for (int i = 0; i < attackMoves.Count; i++)
                    {
                        attackMoves[i].level = attackMoveLevels[i];
                    }
                    levelUpMoves.Sort((m1, m2) => m1.level - m2.level);
                }
            }

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        private void RandomizeEggMoves(GameDataSet gameData, IDistribution moveDistribution, double typeBiasP, bool randomMoveCount, IDistribution moveCountDistribution)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
            {
                var eggMoves = pokemon.eggMoves.wazaNo;

                if (randomMoveCount)
                {
                    int moveCount = eggMoves.Count;
                    if (IsWithin(Boundary.EggMoveCount, moveCount))
                        moveCount = Conform(Boundary.EggMoveCount, moveCountDistribution.Next(eggMoves.Count));
                    while (eggMoves.Count < moveCount)
                    {
                        if (eggMoves.Count == 0)
                        {
                            eggMoves.Add((ushort)moveDistribution.Next(1));
                            continue;
                        }
                        eggMoves.Add(rng.RandomElement(eggMoves));
                    }
                    while (eggMoves.Count > moveCount)
                        eggMoves.RemoveAt(rng.Next(eggMoves.Count));
                }

                for (int i = 0; i < eggMoves.Count; i++)
                {
                    int moveID = moveDistribution.Next(eggMoves[i]);
                    if (rng.Percent(typeBiasP))
                        while (!pokemon.personal.Types.Contains(gameData.moveTable.Waza[moveID].type))
                            moveID = moveDistribution.Next(eggMoves[i]);
                    eggMoves[i] = (ushort)moveID;
                }
            }

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        private void RandomizeExpYields(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
                if (IsWithin(Boundary.ExpYield, pokemon.personal.give_exp))
                    pokemon.personal.give_exp = (ushort)Conform(Boundary.ExpYield, distribution.Next(pokemon.personal.give_exp));

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        private void RandomizeEvYields(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
            {
                byte[] evYield = new byte[6];
                int evYieldTotal = pokemon.personal.EVYieldTotal;
                if (IsWithin(Boundary.EvYieldTotal, evYieldTotal))
                    evYieldTotal = Conform(Boundary.EvYieldTotal,  distribution.Next(evYieldTotal));

                while (evYieldTotal > 0)
                {
                    int index = rng.Next(evYield.Length);
                    int room = (int)GetBoundaries(Boundary.EvYield)[2] - evYield[index];
                    int add = rng.Next(Math.Min(room, 1), Math.Min(room, evYieldTotal));
                    evYield[index] += (byte)add;
                    evYieldTotal -= add;
                }

                pokemon.personal.EVYields = evYield;
            }

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        private void RandomizeInitialFriendship(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
                if (IsWithin(Boundary.InitialFriendship, pokemon.personal.initial_friendship))
                    pokemon.personal.initial_friendship = (byte)Conform(Boundary.InitialFriendship, distribution.Next(pokemon.personal.initial_friendship));

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        private void RandomizeCatchRates(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
                if (IsWithin(Boundary.CatchRate, pokemon.personal.get_rate))
                    pokemon.personal.get_rate = (byte)Conform(Boundary.CatchRate, distribution.Next(pokemon.personal.get_rate));

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        private void RandomizePersonalAbilites(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
            {
                pokemon.personal.tokusei1 = (ushort)distribution.Next(pokemon.personal.tokusei1);
                pokemon.personal.tokusei2 = (ushort)distribution.Next(pokemon.personal.tokusei2);
                pokemon.personal.tokusei3 = (ushort)distribution.Next(pokemon.personal.tokusei3);
            }

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        private void RandomizeGrowthRates(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
            {
                if (pokemon.pastPokemon.Count > 0)
                    continue;

                var forms = gameData.pokemonDataTable.GetAllFormsForPokemon(pokemon.personal.monsno);
                int grow = distribution.Next(rng.RandomElement(forms).personal.grow);

                PropagateGrowthRate(gameData, pokemon, grow);
            }

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        /// <summary>
        /// Copies growth rate to the rest of family.
        /// </summary>
        private void PropagateGrowthRate(GameDataSet gameData, PokemonDataTable.PokemonData pokemon, int grow)
        {
            var forms = gameData.pokemonDataTable.GetAllFormsForPokemon(pokemon.personal.monsno);

            foreach (var form in forms)
                form.personal.grow = (byte)grow;

            foreach (var nextPokemon in pokemon.nextPokemon)
                PropagateGrowthRate(gameData, nextPokemon, grow);
        }

        private void RandomizeWildHeldItems(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
            {
                pokemon.personal.item1 = (ushort)distribution.Next(pokemon.personal.item1);
                pokemon.personal.item2 = (ushort)distribution.Next(pokemon.personal.item2);
                pokemon.personal.item3 = (ushort)distribution.Next(pokemon.personal.item3);
            }

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        private void RandomizeTMCompatibility(GameDataSet gameData, double compatibilityP, double typeBiasP, bool evolveLogic)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
            {
                if (pokemon.pastPokemon.Count > 0 && evolveLogic)
                    continue;

                bool[] tmCompatibility = pokemon.personal.TMFlags;
                for (int i = 0; i < tmCompatibility.Length; i++)
                {
                    if (i >= gameData.itemTable.WazaMachine.Count)
                    {
                        tmCompatibility[i] = false;
                        continue;
                    }

                    var tm = gameData.itemTable.WazaMachine[i];
                    if (!gameData.itemTable.Item[tm.itemNo].Enabled)
                    {
                        tmCompatibility[i] = false;
                        continue;
                    }

                    if (rng.Percent(typeBiasP))
                    {
                        tmCompatibility[i] = pokemon.personal.Types.Contains(gameData.moveTable.Waza[tm.wazaNo].type);
                        continue;
                    }

                    tmCompatibility[i] = rng.Percent(compatibilityP);
                }
                pokemon.personal.TMFlags = tmCompatibility;

                if (evolveLogic)
                    foreach (var next in pokemon.nextPokemon)
                        PropagateTMCompatibility(next, tmCompatibility);
            }

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        /// <summary>
        /// Copies TM compatibility to the rest of family.
        /// </summary>
        private void PropagateTMCompatibility(PokemonDataTable.PokemonData pokemon, bool[] tmCompatibility)
        {
            pokemon.personal.TMFlags = tmCompatibility;

            foreach (var next in pokemon.nextPokemon)
                PropagateTMCompatibility(next, tmCompatibility);
        }

        private void RandomizePokemonTyping(GameDataSet gameData, IDistribution distribution, bool evolveLogic, double doubleTypingP, IDistribution typingCorrelationDistribution)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
            {
                if (pokemon.pastPokemon.Count > 0 && evolveLogic)
                    continue;

                var oldTyping = pokemon.personal.Types;
                List<byte> newTyping = new()
                {
                    (byte)distribution.Next(oldTyping.First())
                };
                if (rng.Percent(doubleTypingP))
                    newTyping.Add((byte)distribution.Next(oldTyping.Last()));
                pokemon.personal.Types = newTyping.ToArray();

                if (evolveLogic)
                    foreach (var next in pokemon.nextPokemon)
                        RandomizeEvolutionTyping(next, pokemon.personal.Types, distribution, doubleTypingP, typingCorrelationDistribution);
            }

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        /// <summary>
        /// Randomizes typings of pokemon based on their previous stages' typing.
        /// </summary>
        private void RandomizeEvolutionTyping(PokemonDataTable.PokemonData pokemon, byte[] pastTyping, IDistribution distribution, double doubleTypingP, IDistribution typingCorrelationDistribution)
        {
            var oldTyping = pokemon.personal.Types;
            List<byte> newTyping = new();
            Analyzer.TypingCorrelation tc = (Analyzer.TypingCorrelation)Enum.ToObject(typeof(Analyzer.TypingCorrelation), typingCorrelationDistribution.Next(0));
            switch (tc)
            {
                case Analyzer.TypingCorrelation.Identical:
                    newTyping.AddRange(pastTyping);
                    break;

                case Analyzer.TypingCorrelation.Addition:
                    newTyping.AddRange(pastTyping);
                    if (newTyping.Count < 2)
                    {
                        int t = newTyping[0];
                        while (t == newTyping[0])
                            t = distribution.Next(oldTyping.Last());
                        newTyping.Add((byte)t);
                    }
                    else
                        newTyping.RemoveAt(rng.Next(newTyping.Count));
                    break;

                case Analyzer.TypingCorrelation.Swap:
                    newTyping.AddRange(pastTyping);
                    var newType = rng.RandomElement(newTyping);
                    while (newTyping.Contains(newType))
                        newType = (byte)distribution.Next(rng.RandomElement(oldTyping));
                    newTyping[rng.Next(newTyping.Count)] = newType;
                    break;

                case Analyzer.TypingCorrelation.NoCorrelation:
                    newTyping.Add((byte)distribution.Next(oldTyping.First()));
                    if (rng.Percent(doubleTypingP))
                        newTyping.Add((byte)distribution.Next(oldTyping.Last()));
                    break;
            }

            pokemon.personal.Types = newTyping.ToArray();

            foreach (var next in pokemon.nextPokemon)
                RandomizeEvolutionTyping(next, pokemon.personal.Types, distribution, doubleTypingP, typingCorrelationDistribution);
        }

        private void RandomizeStats(GameDataSet gameData, IDistribution distribution, bool shuffle, bool randomizeIndividually, bool bstLogic)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
            {
                if (shuffle)
                {
                    var list = pokemon.personal.BaseStats;
                    rng.Shuffle(list);
                    pokemon.personal.BaseStats = list.ToArray();
                }

                if (randomizeIndividually)
                {
                    var list = pokemon.personal.BaseStats;
                    for (int i=0; i<list.Length; i++)
                        if (IsWithin(Boundary.BaseStat, list[i]))
                            list[i] = (byte)Conform(Boundary.BaseStat, distribution.Next(list[i]));

                    pokemon.personal.BaseStats = list.ToArray();
                }
            }

            if (!bstLogic)
                return;

            foreach (var pokemon in gameData.pokemonDataTable.Data)
            {
                EnsureBSTLogic(pokemon);
            }

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        /// <summary>
        /// Adjusts BSTs so that evolutions and surperior forms have a higher BST.
        /// </summary>
        private void EnsureBSTLogic(PokemonDataTable.PokemonData pokemon)
        {
            // TODO: Remove other forms?
            List<PokemonDataTable.PokemonData> next = new();
            List<PokemonDataTable.PokemonData> past = new();
            next.AddRange(pokemon.nextPokemon);
            next.AddRange(pokemon.superiorForms);
            past.AddRange(pokemon.pastPokemon);
            past.AddRange(pokemon.inferiorForms);

            foreach (var nextPokemon in next)
            {
                if (pokemon.personal.BST > nextPokemon.personal.BST)
                {
                    FixBST(pokemon, nextPokemon);
                    EnsureBSTLogic(nextPokemon);
                }
            }
            foreach (var pastPokemon in past)
            {
                if (pokemon.personal.BST < pastPokemon.personal.BST)
                {
                    FixBST(pastPokemon, pokemon);
                    EnsureBSTLogic(pastPokemon);
                }
            }
        }

        /// <summary>
        /// Swaps stats until past's BST is lower or equal to next's.
        /// </summary>
        private void FixBST (PokemonDataTable.PokemonData past, PokemonDataTable.PokemonData next)
        {
            var pastStats = past.personal.BaseStats;
            var nextStats = next.personal.BaseStats;

            while (pastStats.Sum(b => (int)b) > nextStats.Sum(b => (int)b))
            {
                int index = -1;
                int maxDelta = 0;
                for (int i = 0; i < pastStats.Length; i++)
                {
                    int delta = pastStats[i] - nextStats[i];
                    if (delta > maxDelta)
                    {
                        index = i;
                        maxDelta = delta;
                    }
                }
                (nextStats[index], pastStats[index]) = (pastStats[index], nextStats[index]);
            }

            past.personal.BaseStats = pastStats;
            next.personal.BaseStats = nextStats;
        }

        private void RandomizeEvolutionLevels(GameDataSet gameData, IDistribution distribution)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
            {
                foreach (var evolution in pokemon.evolutionPaths.paths)
                    if (evolution.toMonsno != pokemon.personal.monsno && IsWithin(Boundary.Level, evolution.level))
                        evolution.level = (ushort)Conform(Boundary.Level, distribution.Next(evolution.level));

                pokemon.pastEvoLvs = (0, 0);
                pokemon.nextEvoLvs = (ushort.MaxValue, ushort.MaxValue);
                pokemon.pastPokemon = new();
                pokemon.nextPokemon = new();
                pokemon.inferiorForms = new();
                pokemon.superiorForms = new();
            }

            // TODO: Set Families
            //DataParser.SetFamilies();

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }

        private void RandomizeEvolutionDestinations(GameDataSet gameData, IDistribution distribution, bool bstLogic)
        {
            foreach (var pokemon in gameData.pokemonDataTable.Data)
            {
                foreach (var evolution in pokemon.evolutionPaths.paths)
                {
                    if (pokemon.personal.monsno == evolution.toMonsno)
                        continue;
                    PokemonDataTable.PokemonData dest;
                    do
                    {
                        dest = gameData.pokemonDataTable.Data[distribution.Next(gameData.pokemonDataTable.GetDataForPokemon(evolution.toMonsno, evolution.toFormno).personal.id)];
                    }
                    while (!dest.personal.Valid || bstLogic && dest.personal.BST <= pokemon.personal.BST);

                    evolution.toMonsno = dest.personal.monsno;
                    evolution.toFormno = dest.formID;
                }
                pokemon.pastEvoLvs = (0, 0);
                pokemon.nextEvoLvs = (ushort.MaxValue, ushort.MaxValue);
                pokemon.pastPokemon = new();
                pokemon.nextPokemon = new();
                pokemon.inferiorForms = new();
                pokemon.superiorForms = new();
            }

            // TODO: Set Families
            //DataParser.SetFamilies();

            gameData.SetModified(gameData.pokemonDataTable.GetType());
        }
    }
}
