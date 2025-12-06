using AssetsTools.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ImpostersOrdeal.GlobalData;
using static ImpostersOrdeal.GameDataTypes;
using static ImpostersOrdeal.ExternalJsonStructs;
using static ImpostersOrdeal.Wwise;
using AssetsTools.NET.Extra;
using SmartPoint.AssetAssistant;

namespace ImpostersOrdeal
{
    /// <summary>
    ///  Responsible for converting AssetsTools.NET objects into easier to work with objects and back.
    /// </summary>
    static public class DataParser
    {
        static Dictionary<PathEnum, Task<List<AssetTypeValueField>>> monoBehaviourCollection;

        static AssetTypeTemplateField tagDataTemplate = null;
        static AssetTypeTemplateField attributeValueTemplate = null;
        static AssetTypeTemplateField tagWordTemplate = null;
        static AssetTypeTemplateField wordDataTemplate = null;

        /// <summary>
        ///  Parses all files necessary for analysis and configuration.
        /// </summary>
        public static void PrepareAnalysis()
        {
            LoadMonoBehaviours();
            List<Task> tasks = new()
            {
                Task.Run(() => ParseNatures()),
                Task.Run(() => ParseEvScripts()),
                Task.Run(() => ParseAllMessageFiles()),
                Task.Run(() => ParseGrowthRates()),
                Task.Run(() => ParseItems()),
                Task.Run(() => ParsePickupItems()),
                Task.Run(() => ParseShopTables()),
                Task.Run(() => ParseMoves()),
                Task.Run(() => ParseTMs()),
                Task.Run(() => ParsePokemon()),
                Task.Run(() => ParseEncounterTables()),
                Task.Run(() => ParseTrainers()),
                Task.Run(() => ParseBattleTowerTrainers()),
                Task.Run(() => ParseUgTables()),
                Task.Run(() => ParseAbilities()),
                Task.Run(() => ParseTypings()),
                Task.Run(() => ParseTrainerTypes()),
                Task.Run(() => ParseBattleMasterDatas()),
                Task.Run(() => ParseMasterDatas()),
                Task.Run(() => ParsePersonalMasterDatas()),
                Task.Run(() => ParseUIMasterDatas()),
                Task.Run(() => ParseContestMasterDatas()),
                Task.Run(() => TryParseExternalStarters()),
                Task.Run(() => TryParseExternalHoneyTrees())
            };
            ParseDamagaCategories();
            ParseGlobalMetadata();
            ParseDprBin();
            ParseAudioData();
            TryParseModArgs();
            Task.WaitAll(tasks.ToArray());
            //Hot damn! 4GB?? This has got to go.
            monoBehaviourCollection = null;
            GC.Collect();
        }

        private static void TryParseModArgs()
        {
            gameData.modArgs = fileManager.TryGetModArgs();
        }

        private static void TryParseExternalHoneyTrees()
        {
            gameData.externalHoneyTrees = null;
            List<(string name, HoneyTreeZone obj)> files = fileManager.TryGetExternalJsons<HoneyTreeZone>($"Encounters\\HoneyTrees");
            if (files.Count == 0) return;
            gameData.externalHoneyTrees = files;
        }

        private static void TryParseExternalStarters()
        {
            gameData.externalStarters = null;
            List<(string name, Starter obj)> files = fileManager.TryGetExternalJsons<Starter>($"Encounters\\Starter");
            if (files.Count == 0) return;
            gameData.externalStarters = files;
        }

        private static async Task ParseContestMasterDatas()
        {
            gameData.contestResultMotion = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.ContestMasterdatas]).Find(m => m["m_Name"].AsString == "ContestConfigDatas");

            var resultMotionSheet = monoBehaviour["ResultMotion.Array"];
            foreach (var resultMotion in resultMotionSheet)
            {
                ResultMotion rm = new()
                {
                    validFlag = resultMotionSheet["valid_flag"].AsByte,
                    id = resultMotionSheet["id"].AsUShort,
                    monsNo = resultMotionSheet["monsNo"].AsInt,
                    winAnim = resultMotionSheet["winAnim"].AsUInt,
                    loseAnim = resultMotionSheet["loseAnim"].AsUInt,
                    waitAnim = resultMotionSheet["waitAnim"].AsUInt,
                    duration = resultMotionSheet["duration"].AsFloat
                };
                gameData.contestResultMotion.Add(rm);
            }
        }

        /// <summary>
        ///  Loads all monobehaviours asyncronously into monoBehaviourCollection.
        /// </summary>
        private static void LoadMonoBehaviours()
        {
            monoBehaviourCollection = new();
            foreach (PathEnum pe in Enum.GetValues(typeof(PathEnum)))
                monoBehaviourCollection[pe] = Task.Run(() => fileManager.GetMonoBehaviours(pe));
        }

        /// <summary>
        ///  Gets the appropriate path for a specific language.
        /// </summary>
        private static PathEnum GetMessageBundlePathForLanguage(Language language, bool isKanji = false)
        {
            return language switch
            {
                Language.Japanese => isKanji ? PathEnum.JpnKanji : PathEnum.Jpn,
                Language.English => PathEnum.English,
                Language.French => PathEnum.French,
                Language.Italian => PathEnum.Italian,
                Language.German => PathEnum.German,
                Language.Spanish => PathEnum.Spanish,
                Language.Korean => PathEnum.Korean,
                Language.SimpChinese => PathEnum.SimpChinese,
                Language.TradChinese => PathEnum.TradChinese,
                _ => PathEnum.CommonMsbt,
            };
        }

        /// <summary>
        ///  Gets the appropriate message file prefix for a specific language.
        /// </summary>
        private static string GetMessageFilePrefixForLanguage(Language language, bool isKanji = false)
        {
            return language switch
            {
                Language.Japanese => isKanji ? "jpn_kanji" : "jpn",
                Language.English => "english",
                Language.French => "french",
                Language.Italian => "italian",
                Language.German => "german",
                Language.Spanish => "spanish",
                Language.Korean => "korean",
                Language.SimpChinese => "simp_chinese",
                Language.TradChinese => "trad_chinese",
                _ => "",
            };
        }

        /// <summary>
        ///  Formats the message file name to have the proper language prefix.
        /// </summary>
        private static string FormatMessageFileNameForLanguage(string fileName, Language language, bool isKanji = false)
        {
            return string.Join("_", GetMessageFilePrefixForLanguage(language, isKanji), fileName);
        }

        /// <summary>
        ///  Gets all the labels of a message file in a specific language.
        /// </summary>
        private static async Task<AssetTypeValueField[]> FindLabelArrayOfMessageFileAsync(string fileName, Language language, bool isKanji = false)
        {
            string fullFileName = FormatMessageFileNameForLanguage(fileName, language, isKanji);
            PathEnum pathForLanguage = GetMessageBundlePathForLanguage(language, isKanji);

            var baseField = (await monoBehaviourCollection[PathEnum.CommonMsbt]).Find(m => m["m_Name"].AsString == fullFileName) ??
                            (await monoBehaviourCollection[pathForLanguage]).Find(m => m["m_Name"].AsString == fullFileName);
            return baseField?["labelDataArray.Array"].Children.ToArray() ?? Array.Empty<AssetTypeValueField>();
        }

        /// <summary>
        ///  Gets all the labels of a message file in a specific language.
        /// </summary>
        private static AssetTypeValueField[] FindLabelArrayOfMessageFile(string fileName, Language language, bool isKanji = false)
        {
            string fullFileName = FormatMessageFileNameForLanguage(fileName, language, isKanji);
            PathEnum pathForLanguage = GetMessageBundlePathForLanguage(language, isKanji);
            
            var baseField = fileManager.GetMonoBehaviours(PathEnum.CommonMsbt).Find(m => m["m_Name"].AsString == fullFileName) ??
                            fileManager.GetMonoBehaviours(pathForLanguage).Find(m => m["m_Name"].AsString == fullFileName);
            return baseField?["labelDataArray.Array"].Children.ToArray() ?? Array.Empty<AssetTypeValueField>();
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed TrainerTypes.
        /// </summary>
        private static async Task ParseTrainerTypes()
        {
            gameData.trainerTypes = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => m["m_Name"].AsString == "TrainerTable");

            AssetTypeValueField[] nameFields = await FindLabelArrayOfMessageFileAsync("dp_trainers_type", Language.English);
            Dictionary<string, string> trainerTypeNames = new();
            foreach (AssetTypeValueField label in nameFields)
                if (label["wordDataArray.Array"].Children.Count > 0)
                    trainerTypeNames[label["labelName"].AsString] = label["wordDataArray.Array"][0]["str"].AsString;

            var trainerTypeFields = monoBehaviour["TrainerType.Array"].Children;
            for (int trainerTypeIdx = 0; trainerTypeIdx < trainerTypeFields.Count; trainerTypeIdx++)
            {
                if (trainerTypeFields[trainerTypeIdx]["TrainerID"].AsInt == -1)
                    continue;

                TrainerType trainerType = new();
                trainerType.trainerTypeID = trainerTypeIdx;
                trainerType.label = trainerTypeFields[trainerTypeIdx]["LabelTrType"].AsString;

                trainerType.name = !trainerTypeNames.ContainsKey(trainerType.label) ? "" : trainerTypeNames[trainerType.label];

                gameData.trainerTypes.Add(trainerType);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed Natures.
        /// </summary>
        private static async Task ParseNatures()
        {
            gameData.natures = new();
            AssetTypeValueField[] natureFields = await FindLabelArrayOfMessageFileAsync("ss_seikaku", Language.English);

            for (int natureID = 0; natureID < natureFields.Length; natureID++)
            {
                Nature nature = new();
                nature.natureID = natureID;
                nature.name = natureFields[natureID]["wordDataArray.Array"][0]["str"].AsString;

                gameData.natures.Add(nature);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed DamageCategorie.
        /// </summary>
        private static void ParseDamagaCategories()
        {
            gameData.damageCategories = new();
            for (int i = 0; i < 3; i++)
            {
                DamageCategory damageCategory = new();
                damageCategory.damageCategoryID = i;
                gameData.damageCategories.Add(damageCategory);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed Typings.
        /// </summary>
        private static async Task ParseTypings()
        {
            gameData.typings = new();
            AssetTypeValueField[] typingFields = await FindLabelArrayOfMessageFileAsync("ss_typename", Language.English);

            for (int typingID = 0; typingID < typingFields.Length; typingID++)
            {
                Typing typing = new();
                typing.typingID = typingID;
                typing.name = typingFields[typingID]["wordDataArray.Array"][0]["str"].AsString;

                gameData.typings.Add(typing);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed Abilities.
        /// </summary>
        private static async Task ParseAbilities()
        {
            gameData.abilities = new();
            AssetTypeValueField[] abilityFields = await FindLabelArrayOfMessageFileAsync("ss_tokusei", Language.English);

            for (int abilityID = 0; abilityID < abilityFields.Length; abilityID++)
            {
                Ability ability = new();
                ability.abilityID = abilityID;
                ability.name = abilityFields[abilityID]["wordDataArray.Array"][0]["str"].AsString;

                gameData.abilities.Add(ability);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed underground data.
        /// </summary>
        private static async Task ParseUgTables()
        {
            gameData.ugAreas = new();
            gameData.ugEncounterFiles = new();
            gameData.ugEncounterLevelSets = new();
            gameData.ugSpecialEncounters = new();
            gameData.ugPokemonData = new();
            List<AssetTypeValueField> monoBehaviours = await monoBehaviourCollection[PathEnum.Ugdata];

            var ugAreaFields = monoBehaviours.Find(m => m["m_Name"].AsString == "UgRandMark")["table.Array"].Children;
            for (int ugAreaIdx = 0; ugAreaIdx < ugAreaFields.Count; ugAreaIdx++)
            {
                UgArea ugArea = new();
                ugArea.id = ugAreaFields[ugAreaIdx]["id"].AsInt;
                ugArea.fileName = ugAreaFields[ugAreaIdx]["FileName"].AsString;

                gameData.ugAreas.Add(ugArea);
            }

            List<AssetTypeValueField> ugEncounterFiles = monoBehaviours.Where(m => m["m_Name"].AsString.StartsWith("UgEncount_")).ToList();
            for (int ugEncounterFileIdx = 0; ugEncounterFileIdx < ugEncounterFiles.Count; ugEncounterFileIdx++)
            {
                UgEncounterFile ugEncounterFile = new();
                ugEncounterFile.mName = ugEncounterFiles[ugEncounterFileIdx]["m_Name"].AsString;

                ugEncounterFile.ugEncounters = new();
                var ugMonFields = ugEncounterFiles[ugEncounterFileIdx]["table.Array"].Children;
                for (int ugMonIdx = 0; ugMonIdx < ugMonFields.Count; ugMonIdx++)
                {
                    UgEncounter ugEncounter = new();
                    ugEncounter.dexID = ugMonFields[ugMonIdx]["monsno"].AsInt;
                    ugEncounter.version = ugMonFields[ugMonIdx]["version"].AsInt;
                    ugEncounter.zukanFlag = ugMonFields[ugMonIdx]["zukanflag"].AsInt;

                    ugEncounterFile.ugEncounters.Add(ugEncounter);
                }

                gameData.ugEncounterFiles.Add(ugEncounterFile);
            }

            var ugEncounterLevelFields = monoBehaviours.Find(m => m["m_Name"].AsString == "UgEncountLevel")["Data.Array"].Children;
            for (int ugEncouterLevelIdx = 0; ugEncouterLevelIdx < ugEncounterLevelFields.Count; ugEncouterLevelIdx++)
            {
                UgEncounterLevelSet ugLevels = new();
                ugLevels.minLv = ugEncounterLevelFields[ugEncouterLevelIdx]["MinLv"].AsInt;
                ugLevels.maxLv = ugEncounterLevelFields[ugEncouterLevelIdx]["MaxLv"].AsInt;

                gameData.ugEncounterLevelSets.Add(ugLevels);
            }

            var ugSpecialEncounterFields = monoBehaviours.Find(m => m["m_Name"].AsString == "UgSpecialPokemon")["Sheet1.Array"].Children;
            for (int ugSpecialEncounterIdx = 0; ugSpecialEncounterIdx < ugSpecialEncounterFields.Count; ugSpecialEncounterIdx++)
            {
                UgSpecialEncounter ugSpecialEncounter = new();
                ugSpecialEncounter.id = ugSpecialEncounterFields[ugSpecialEncounterIdx]["id"].AsInt;
                ugSpecialEncounter.dexID = ugSpecialEncounterFields[ugSpecialEncounterIdx]["monsno"].AsInt;
                ugSpecialEncounter.version = ugSpecialEncounterFields[ugSpecialEncounterIdx]["version"].AsInt;
                ugSpecialEncounter.dRate = ugSpecialEncounterFields[ugSpecialEncounterIdx]["Dspecialrate"].AsInt;
                ugSpecialEncounter.pRate = ugSpecialEncounterFields[ugSpecialEncounterIdx]["Pspecialrate"].AsInt;

                gameData.ugSpecialEncounters.Add(ugSpecialEncounter);
            }

            var ugPokemonDataFields = monoBehaviours.Find(m => m["m_Name"].AsString == "UgPokemonData")["table.Array"].Children;
            for (int ugPokemonDataIdx = 0; ugPokemonDataIdx < ugPokemonDataFields.Count; ugPokemonDataIdx++)
            {
                UgPokemonData ugPokemonData = new();
                ugPokemonData.monsno = ugPokemonDataFields[ugPokemonDataIdx]["monsno"].AsInt;
                ugPokemonData.type1ID = ugPokemonDataFields[ugPokemonDataIdx]["type1ID"].AsInt;
                ugPokemonData.type2ID = ugPokemonDataFields[ugPokemonDataIdx]["type2ID"].AsInt;
                ugPokemonData.size = ugPokemonDataFields[ugPokemonDataIdx]["size"].AsInt;
                ugPokemonData.movetype = ugPokemonDataFields[ugPokemonDataIdx]["movetype"].AsInt;
                ugPokemonData.reactioncode = new int[2];
                for (int i = 0; i < ugPokemonData.reactioncode.Length; i++)
                    ugPokemonData.reactioncode[i] = ugPokemonDataFields[ugPokemonDataIdx]["reactioncode.Array"][i].AsInt;
                ugPokemonData.moveRate = new int[2];
                for (int i = 0; i < ugPokemonData.moveRate.Length; i++)
                    ugPokemonData.moveRate[i] = ugPokemonDataFields[ugPokemonDataIdx]["move_rate.Array"][i].AsInt;
                ugPokemonData.submoveRate = new int[5];
                for (int i = 0; i < ugPokemonData.submoveRate.Length; i++)
                    ugPokemonData.submoveRate[i] = ugPokemonDataFields[ugPokemonDataIdx]["submove_rate.Array"][i].AsInt;
                ugPokemonData.reaction = new int[5];
                for (int i = 0; i < ugPokemonData.reaction.Length; i++)
                    ugPokemonData.reaction[i] = ugPokemonDataFields[ugPokemonDataIdx]["reaction.Array"][i].AsInt;
                ugPokemonData.flagrate = new int[6];
                for (int i = 0; i < ugPokemonData.flagrate.Length; i++)
                    ugPokemonData.flagrate[i] = ugPokemonDataFields[ugPokemonDataIdx]["flagrate.Array"][i].AsInt;
                ugPokemonData.rateup = ugPokemonDataFields[ugPokemonDataIdx]["rateup"].AsInt;

                gameData.ugPokemonData.Add(ugPokemonData);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with a parsed Trainer table.
        /// </summary>
        private static async Task ParseTrainers()
        {
            gameData.trainers = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => m["m_Name"].AsString == "TrainerTable");

            AssetTypeValueField[] nameFields = await FindLabelArrayOfMessageFileAsync("dp_trainers_name", Language.English);
            Dictionary<string, string> trainerNames = new();
            gameData.trainerNames = trainerNames;
            foreach (AssetTypeValueField label in nameFields)
                if (label["wordDataArray.Array"].Children.Count > 0)
                    trainerNames[label["labelName"].AsString] = label["wordDataArray.Array"][0]["str"].AsString;

            var trainerFields = monoBehaviour["TrainerData.Array"].Children;
            var trainerPokemonFields = monoBehaviour["TrainerPoke.Array"].Children;
            for (int trainerIdx = 0; trainerIdx < Math.Min(trainerFields.Count, trainerPokemonFields.Count); trainerIdx++)
            {
                Trainer trainer = new();
                trainer.trainerTypeID = trainerFields[trainerIdx]["TypeID"].AsInt;
                trainer.colorID = trainerFields[trainerIdx]["ColorID"].AsByte;
                trainer.fightType = trainerFields[trainerIdx]["FightType"].AsByte;
                trainer.arenaID = trainerFields[trainerIdx]["ArenaID"].AsInt;
                trainer.effectID = trainerFields[trainerIdx]["EffectID"].AsInt;
                trainer.gold = trainerFields[trainerIdx]["Gold"].AsByte;
                trainer.useItem1 = trainerFields[trainerIdx]["UseItem1"].AsUShort;
                trainer.useItem2 = trainerFields[trainerIdx]["UseItem2"].AsUShort;
                trainer.useItem3 = trainerFields[trainerIdx]["UseItem3"].AsUShort;
                trainer.useItem4 = trainerFields[trainerIdx]["UseItem4"].AsUShort;
                trainer.hpRecoverFlag = trainerFields[trainerIdx]["HpRecoverFlag"].AsByte;
                trainer.giftItem = trainerFields[trainerIdx]["GiftItem"].AsUShort;
                trainer.nameLabel = trainerFields[trainerIdx]["NameLabel"].AsString;
                trainer.aiBit = trainerFields[trainerIdx]["AIBit"].AsUInt;
                trainer.trainerID = trainerIdx;
                trainer.name = trainerNames[trainer.nameLabel];

                //Parse trainer pokemon
                trainer.trainerPokemon = new();
                var pokemonFields = trainerPokemonFields[trainerIdx];

                if (pokemonFields["P1MonsNo"].AsUShort != 0)
                {
                    TrainerPokemon pokemon = new();
                    pokemon.dexID = pokemonFields["P1MonsNo"].AsUShort;
                    pokemon.formID = pokemonFields["P1FormNo"].AsUShort;
                    pokemon.isRare = pokemonFields["P1IsRare"].AsByte;
                    pokemon.level = pokemonFields["P1Level"].AsByte;
                    pokemon.sex = pokemonFields["P1Sex"].AsByte;
                    pokemon.natureID = pokemonFields["P1Seikaku"].AsByte;
                    pokemon.abilityID = pokemonFields["P1Tokusei"].AsUShort;
                    pokemon.moveID1 = pokemonFields["P1Waza1"].AsUShort;
                    pokemon.moveID2 = pokemonFields["P1Waza2"].AsUShort;
                    pokemon.moveID3 = pokemonFields["P1Waza3"].AsUShort;
                    pokemon.moveID4 = pokemonFields["P1Waza4"].AsUShort;
                    pokemon.itemID = pokemonFields["P1Item"].AsUShort;
                    pokemon.ballID = pokemonFields["P1Ball"].AsByte;
                    pokemon.seal = pokemonFields["P1Seal"].AsInt;
                    pokemon.hpIV = pokemonFields["P1TalentHp"].AsByte;
                    pokemon.atkIV = pokemonFields["P1TalentAtk"].AsByte;
                    pokemon.defIV = pokemonFields["P1TalentDef"].AsByte;
                    pokemon.spAtkIV = pokemonFields["P1TalentSpAtk"].AsByte;
                    pokemon.spDefIV = pokemonFields["P1TalentSpDef"].AsByte;
                    pokemon.spdIV = pokemonFields["P1TalentAgi"].AsByte;
                    pokemon.hpEV = pokemonFields["P1EffortHp"].AsByte;
                    pokemon.atkEV = pokemonFields["P1EffortAtk"].AsByte;
                    pokemon.defEV = pokemonFields["P1EffortDef"].AsByte;
                    pokemon.spAtkEV = pokemonFields["P1EffortSpAtk"].AsByte;
                    pokemon.spDefEV = pokemonFields["P1EffortSpDef"].AsByte;
                    pokemon.spdEV = pokemonFields["P1EffortAgi"].AsByte;

                    trainer.trainerPokemon.Add(pokemon);
                }

                if (pokemonFields["P2MonsNo"].AsUShort != 0)
                {
                    TrainerPokemon pokemon = new();
                    pokemon.dexID = pokemonFields["P2MonsNo"].AsUShort;
                    pokemon.formID = pokemonFields["P2FormNo"].AsUShort;
                    pokemon.isRare = pokemonFields["P2IsRare"].AsByte;
                    pokemon.level = pokemonFields["P2Level"].AsByte;
                    pokemon.sex = pokemonFields["P2Sex"].AsByte;
                    pokemon.natureID = pokemonFields["P2Seikaku"].AsByte;
                    pokemon.abilityID = pokemonFields["P2Tokusei"].AsUShort;
                    pokemon.moveID1 = pokemonFields["P2Waza1"].AsUShort;
                    pokemon.moveID2 = pokemonFields["P2Waza2"].AsUShort;
                    pokemon.moveID3 = pokemonFields["P2Waza3"].AsUShort;
                    pokemon.moveID4 = pokemonFields["P2Waza4"].AsUShort;
                    pokemon.itemID = pokemonFields["P2Item"].AsUShort;
                    pokemon.ballID = pokemonFields["P2Ball"].AsByte;
                    pokemon.seal = pokemonFields["P2Seal"].AsInt;
                    pokemon.hpIV = pokemonFields["P2TalentHp"].AsByte;
                    pokemon.atkIV = pokemonFields["P2TalentAtk"].AsByte;
                    pokemon.defIV = pokemonFields["P2TalentDef"].AsByte;
                    pokemon.spAtkIV = pokemonFields["P2TalentSpAtk"].AsByte;
                    pokemon.spDefIV = pokemonFields["P2TalentSpDef"].AsByte;
                    pokemon.spdIV = pokemonFields["P2TalentAgi"].AsByte;
                    pokemon.hpEV = pokemonFields["P2EffortHp"].AsByte;
                    pokemon.atkEV = pokemonFields["P2EffortAtk"].AsByte;
                    pokemon.defEV = pokemonFields["P2EffortDef"].AsByte;
                    pokemon.spAtkEV = pokemonFields["P2EffortSpAtk"].AsByte;
                    pokemon.spDefEV = pokemonFields["P2EffortSpDef"].AsByte;
                    pokemon.spdEV = pokemonFields["P2EffortAgi"].AsByte;

                    trainer.trainerPokemon.Add(pokemon);
                }

                if (pokemonFields["P3MonsNo"].AsUShort != 0)
                {
                    TrainerPokemon pokemon = new();
                    pokemon.dexID = pokemonFields["P3MonsNo"].AsUShort;
                    pokemon.formID = pokemonFields["P3FormNo"].AsUShort;
                    pokemon.isRare = pokemonFields["P3IsRare"].AsByte;
                    pokemon.level = pokemonFields["P3Level"].AsByte;
                    pokemon.sex = pokemonFields["P3Sex"].AsByte;
                    pokemon.natureID = pokemonFields["P3Seikaku"].AsByte;
                    pokemon.abilityID = pokemonFields["P3Tokusei"].AsUShort;
                    pokemon.moveID1 = pokemonFields["P3Waza1"].AsUShort;
                    pokemon.moveID2 = pokemonFields["P3Waza2"].AsUShort;
                    pokemon.moveID3 = pokemonFields["P3Waza3"].AsUShort;
                    pokemon.moveID4 = pokemonFields["P3Waza4"].AsUShort;
                    pokemon.itemID = pokemonFields["P3Item"].AsUShort;
                    pokemon.ballID = pokemonFields["P3Ball"].AsByte;
                    pokemon.seal = pokemonFields["P3Seal"].AsInt;
                    pokemon.hpIV = pokemonFields["P3TalentHp"].AsByte;
                    pokemon.atkIV = pokemonFields["P3TalentAtk"].AsByte;
                    pokemon.defIV = pokemonFields["P3TalentDef"].AsByte;
                    pokemon.spAtkIV = pokemonFields["P3TalentSpAtk"].AsByte;
                    pokemon.spDefIV = pokemonFields["P3TalentSpDef"].AsByte;
                    pokemon.spdIV = pokemonFields["P3TalentAgi"].AsByte;
                    pokemon.hpEV = pokemonFields["P3EffortHp"].AsByte;
                    pokemon.atkEV = pokemonFields["P3EffortAtk"].AsByte;
                    pokemon.defEV = pokemonFields["P3EffortDef"].AsByte;
                    pokemon.spAtkEV = pokemonFields["P3EffortSpAtk"].AsByte;
                    pokemon.spDefEV = pokemonFields["P3EffortSpDef"].AsByte;
                    pokemon.spdEV = pokemonFields["P3EffortAgi"].AsByte;

                    trainer.trainerPokemon.Add(pokemon);
                }

                if (pokemonFields["P4MonsNo"].AsUShort != 0)
                {
                    TrainerPokemon pokemon = new();
                    pokemon.dexID = pokemonFields["P4MonsNo"].AsUShort;
                    pokemon.formID = pokemonFields["P4FormNo"].AsUShort;
                    pokemon.isRare = pokemonFields["P4IsRare"].AsByte;
                    pokemon.level = pokemonFields["P4Level"].AsByte;
                    pokemon.sex = pokemonFields["P4Sex"].AsByte;
                    pokemon.natureID = pokemonFields["P4Seikaku"].AsByte;
                    pokemon.abilityID = pokemonFields["P4Tokusei"].AsUShort;
                    pokemon.moveID1 = pokemonFields["P4Waza1"].AsUShort;
                    pokemon.moveID2 = pokemonFields["P4Waza2"].AsUShort;
                    pokemon.moveID3 = pokemonFields["P4Waza3"].AsUShort;
                    pokemon.moveID4 = pokemonFields["P4Waza4"].AsUShort;
                    pokemon.itemID = pokemonFields["P4Item"].AsUShort;
                    pokemon.ballID = pokemonFields["P4Ball"].AsByte;
                    pokemon.seal = pokemonFields["P4Seal"].AsInt;
                    pokemon.hpIV = pokemonFields["P4TalentHp"].AsByte;
                    pokemon.atkIV = pokemonFields["P4TalentAtk"].AsByte;
                    pokemon.defIV = pokemonFields["P4TalentDef"].AsByte;
                    pokemon.spAtkIV = pokemonFields["P4TalentSpAtk"].AsByte;
                    pokemon.spDefIV = pokemonFields["P4TalentSpDef"].AsByte;
                    pokemon.spdIV = pokemonFields["P4TalentAgi"].AsByte;
                    pokemon.hpEV = pokemonFields["P4EffortHp"].AsByte;
                    pokemon.atkEV = pokemonFields["P4EffortAtk"].AsByte;
                    pokemon.defEV = pokemonFields["P4EffortDef"].AsByte;
                    pokemon.spAtkEV = pokemonFields["P4EffortSpAtk"].AsByte;
                    pokemon.spDefEV = pokemonFields["P4EffortSpDef"].AsByte;
                    pokemon.spdEV = pokemonFields["P4EffortAgi"].AsByte;

                    trainer.trainerPokemon.Add(pokemon);
                }

                if (pokemonFields["P5MonsNo"].AsUShort != 0)
                {
                    TrainerPokemon pokemon = new();
                    pokemon.dexID = pokemonFields["P5MonsNo"].AsUShort;
                    pokemon.formID = pokemonFields["P5FormNo"].AsUShort;
                    pokemon.isRare = pokemonFields["P5IsRare"].AsByte;
                    pokemon.level = pokemonFields["P5Level"].AsByte;
                    pokemon.sex = pokemonFields["P5Sex"].AsByte;
                    pokemon.natureID = pokemonFields["P5Seikaku"].AsByte;
                    pokemon.abilityID = pokemonFields["P5Tokusei"].AsUShort;
                    pokemon.moveID1 = pokemonFields["P5Waza1"].AsUShort;
                    pokemon.moveID2 = pokemonFields["P5Waza2"].AsUShort;
                    pokemon.moveID3 = pokemonFields["P5Waza3"].AsUShort;
                    pokemon.moveID4 = pokemonFields["P5Waza4"].AsUShort;
                    pokemon.itemID = pokemonFields["P5Item"].AsUShort;
                    pokemon.ballID = pokemonFields["P5Ball"].AsByte;
                    pokemon.seal = pokemonFields["P5Seal"].AsInt;
                    pokemon.hpIV = pokemonFields["P5TalentHp"].AsByte;
                    pokemon.atkIV = pokemonFields["P5TalentAtk"].AsByte;
                    pokemon.defIV = pokemonFields["P5TalentDef"].AsByte;
                    pokemon.spAtkIV = pokemonFields["P5TalentSpAtk"].AsByte;
                    pokemon.spDefIV = pokemonFields["P5TalentSpDef"].AsByte;
                    pokemon.spdIV = pokemonFields["P5TalentAgi"].AsByte;
                    pokemon.hpEV = pokemonFields["P5EffortHp"].AsByte;
                    pokemon.atkEV = pokemonFields["P5EffortAtk"].AsByte;
                    pokemon.defEV = pokemonFields["P5EffortDef"].AsByte;
                    pokemon.spAtkEV = pokemonFields["P5EffortSpAtk"].AsByte;
                    pokemon.spDefEV = pokemonFields["P5EffortSpDef"].AsByte;
                    pokemon.spdEV = pokemonFields["P5EffortAgi"].AsByte;

                    trainer.trainerPokemon.Add(pokemon);
                }

                if (pokemonFields["P6MonsNo"].AsUShort != 0)
                {
                    TrainerPokemon pokemon = new();
                    pokemon.dexID = pokemonFields["P6MonsNo"].AsUShort;
                    pokemon.formID = pokemonFields["P6FormNo"].AsUShort;
                    pokemon.isRare = pokemonFields["P6IsRare"].AsByte;
                    pokemon.level = pokemonFields["P6Level"].AsByte;
                    pokemon.sex = pokemonFields["P6Sex"].AsByte;
                    pokemon.natureID = pokemonFields["P6Seikaku"].AsByte;
                    pokemon.abilityID = pokemonFields["P6Tokusei"].AsUShort;
                    pokemon.moveID1 = pokemonFields["P6Waza1"].AsUShort;
                    pokemon.moveID2 = pokemonFields["P6Waza2"].AsUShort;
                    pokemon.moveID3 = pokemonFields["P6Waza3"].AsUShort;
                    pokemon.moveID4 = pokemonFields["P6Waza4"].AsUShort;
                    pokemon.itemID = pokemonFields["P6Item"].AsUShort;
                    pokemon.ballID = pokemonFields["P6Ball"].AsByte;
                    pokemon.seal = pokemonFields["P6Seal"].AsInt;
                    pokemon.hpIV = pokemonFields["P6TalentHp"].AsByte;
                    pokemon.atkIV = pokemonFields["P6TalentAtk"].AsByte;
                    pokemon.defIV = pokemonFields["P6TalentDef"].AsByte;
                    pokemon.spAtkIV = pokemonFields["P6TalentSpAtk"].AsByte;
                    pokemon.spDefIV = pokemonFields["P6TalentSpDef"].AsByte;
                    pokemon.spdIV = pokemonFields["P6TalentAgi"].AsByte;
                    pokemon.hpEV = pokemonFields["P6EffortHp"].AsByte;
                    pokemon.atkEV = pokemonFields["P6EffortAtk"].AsByte;
                    pokemon.defEV = pokemonFields["P6EffortDef"].AsByte;
                    pokemon.spAtkEV = pokemonFields["P6EffortSpAtk"].AsByte;
                    pokemon.spDefEV = pokemonFields["P6EffortSpDef"].AsByte;
                    pokemon.spdEV = pokemonFields["P6EffortAgi"].AsByte;

                    trainer.trainerPokemon.Add(pokemon);
                }

                gameData.trainers.Add(trainer);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with a parsed Battle Tower Trainer table .
        /// </summary>
        private static async Task ParseBattleTowerTrainers()
        {
            gameData.battleTowerTrainers = new();
            gameData.battleTowerTrainersDouble = new();
            gameData.battleTowerTrainerPokemons = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => m["m_Name"].AsString == "TowerTrainerTable");
            AssetTypeValueField monoBehaviour2 = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => m["m_Name"].AsString == "TowerSingleStockTable");
            AssetTypeValueField monoBehaviour3 = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => m["m_Name"].AsString == "TowerDoubleStockTable");

            var nameFields = await FindLabelArrayOfMessageFileAsync("dp_trainers_name", Language.English);
            Dictionary<string, string> trainerNames = new();
            gameData.trainerNames = trainerNames;
            foreach (AssetTypeValueField label in nameFields)
                if (label["wordDataArray.Array"].Children.Count > 0)
                    trainerNames[label["labelName"].AsString] = label["wordDataArray.Array"][0]["str"].AsString;

            var trainerFields = monoBehaviour2["TowerSingleStock.Array"].Children;
            var trainerFieldsDouble = monoBehaviour3["TowerDoubleStock.Array"].Children;
            var pokemonFields = monoBehaviour["TrainerPoke.Array"].Children;
            var nameFieldsTower = monoBehaviour["TrainerData.Array"].Children;

            //Single battle parser
            for (int trainerIdx = 0; trainerIdx < trainerFields.Count; trainerIdx++)
            {
                BattleTowerTrainer trainer = new();
                trainer.trainerID2 = trainerFields[trainerIdx]["ID"].AsUInt;
                trainer.trainerTypeID = trainerFields[trainerIdx]["TrainerID"].AsInt;
                trainer.trainerTypeID2 = -1;
                trainer.battleTowerPokemonID1 = trainerFields[trainerIdx]["PokeID.Array"][0].AsUInt;
                trainer.battleTowerPokemonID2 = trainerFields[trainerIdx]["PokeID.Array"][1].AsUInt;
                trainer.battleTowerPokemonID3 = trainerFields[trainerIdx]["PokeID.Array"][2].AsUInt;
                trainer.battleTowerPokemonID4 = 0;
                trainer.battleBGM = trainerFields[trainerIdx]["BattleBGM"].AsString;
                trainer.winBGM = trainerFields[trainerIdx]["WinBGM"].AsString;
                trainer.nameLabel = nameFieldsTower[trainer.trainerTypeID]["NameLabel"].AsString;
                trainer.name = trainerNames[trainer.nameLabel];
                trainer.nameLabel2 = null;
                trainer.name2 = null;
                trainer.isDouble = false;
                gameData.battleTowerTrainers.Add(trainer);
            }

            //Double battle parser
            for (int trainerIdx = 0; trainerIdx < trainerFieldsDouble.Count; trainerIdx++)
            {
                BattleTowerTrainer trainer = new();
                trainer.trainerID2 = trainerFieldsDouble[trainerIdx]["ID"].AsUInt;
                trainer.trainerTypeID = trainerFieldsDouble[trainerIdx]["TrainerID.Array"][0].AsInt;
                trainer.trainerTypeID2 = trainerFieldsDouble[trainerIdx]["TrainerID.Array"][1].AsInt;
                trainer.battleTowerPokemonID1 = trainerFieldsDouble[trainerIdx]["PokeID.Array"][0].AsUInt;
                trainer.battleTowerPokemonID2 = trainerFieldsDouble[trainerIdx]["PokeID.Array"][1].AsUInt;
                trainer.battleTowerPokemonID3 = trainerFieldsDouble[trainerIdx]["PokeID.Array"][2].AsUInt;
                trainer.battleTowerPokemonID4 = trainerFieldsDouble[trainerIdx]["PokeID.Array"][3].AsUInt;
                trainer.battleBGM = trainerFieldsDouble[trainerIdx]["BattleBGM"].AsString;
                trainer.winBGM = trainerFieldsDouble[trainerIdx]["WinBGM"].AsString;
                trainer.nameLabel = nameFieldsTower[trainer.trainerTypeID]["NameLabel"].AsString;
                trainer.name = trainerNames[trainer.nameLabel];
                if (trainer.trainerTypeID2 != -1)
                {
                    trainer.nameLabel2 = nameFieldsTower[trainer.trainerTypeID2]["NameLabel"].AsString;
                    trainer.name2 = trainerNames[trainer.nameLabel2];
                }
                else
                {
                    trainer.nameLabel2 = null;
                    trainer.name2 = null;
                }
                trainer.isDouble = true;
                gameData.battleTowerTrainersDouble.Add(trainer);
            }

            //Parse battle tower trainer pokemon
            for (int pokemonIdx = 0; pokemonIdx < pokemonFields.Count; pokemonIdx++)
            {
                BattleTowerTrainerPokemon pokemon = new();
                pokemon.pokemonID = pokemonFields[pokemonIdx]["ID"].AsUInt;
                pokemon.dexID = pokemonFields[pokemonIdx]["MonsNo"].AsInt;
                pokemon.formID = pokemonFields[pokemonIdx]["FormNo"].AsUShort;
                pokemon.isRare = pokemonFields[pokemonIdx]["IsRare"].AsByte;
                pokemon.level = pokemonFields[pokemonIdx]["Level"].AsByte;
                pokemon.sex = pokemonFields[pokemonIdx]["Sex"].AsByte;
                pokemon.natureID = pokemonFields[pokemonIdx]["Seikaku"].AsInt;
                pokemon.abilityID = pokemonFields[pokemonIdx]["Tokusei"].AsInt;
                pokemon.moveID1 = pokemonFields[pokemonIdx]["Waza1"].AsInt;
                pokemon.moveID2 = pokemonFields[pokemonIdx]["Waza2"].AsInt;
                pokemon.moveID3 = pokemonFields[pokemonIdx]["Waza3"].AsInt;
                pokemon.moveID4 = pokemonFields[pokemonIdx]["Waza4"].AsInt;
                pokemon.itemID = pokemonFields[pokemonIdx]["Item"].AsUShort;
                pokemon.ballID = pokemonFields[pokemonIdx]["Ball"].AsByte;
                pokemon.seal = pokemonFields[pokemonIdx]["Seal"].AsInt;
                pokemon.hpIV = pokemonFields[pokemonIdx]["TalentHp"].AsByte;
                pokemon.atkIV = pokemonFields[pokemonIdx]["TalentAtk"].AsByte;
                pokemon.defIV = pokemonFields[pokemonIdx]["TalentDef"].AsByte;
                pokemon.spAtkIV = pokemonFields[pokemonIdx]["TalentSpAtk"].AsByte;
                pokemon.spDefIV = pokemonFields[pokemonIdx]["TalentSpDef"].AsByte;
                pokemon.spdIV = pokemonFields[pokemonIdx]["TalentAgi"].AsByte;
                pokemon.hpEV = pokemonFields[pokemonIdx]["EffortHp"].AsByte;
                pokemon.atkEV = pokemonFields[pokemonIdx]["EffortAtk"].AsByte;
                pokemon.defEV = pokemonFields[pokemonIdx]["EffortDef"].AsByte;
                pokemon.spAtkEV = pokemonFields[pokemonIdx]["EffortSpAtk"].AsByte;
                pokemon.spDefEV = pokemonFields[pokemonIdx]["EffortSpDef"].AsByte;
                pokemon.spdEV = pokemonFields[pokemonIdx]["EffortAgi"].AsByte;
                gameData.battleTowerTrainerPokemons.Add(pokemon);
            }

        }

        /// <summary>
        ///  Overwrites GlobalData with parsed EncounterTables.
        /// </summary>
        private static async Task ParseEncounterTables()
        {
            gameData.encounterTableFiles = new EncounterTableFile[2];

            List<AssetTypeValueField> monoBehaviours = await monoBehaviourCollection[PathEnum.Gamesettings];
            AssetTypeValueField[] encounterTableMonoBehaviours = new AssetTypeValueField[2];
            encounterTableMonoBehaviours[0] = monoBehaviours.Find(m => m["m_Name"].AsString == "FieldEncountTable_d");
            encounterTableMonoBehaviours[1] = monoBehaviours.Find(m => m["m_Name"].AsString == "FieldEncountTable_p");
            for (int encounterTableFileIdx = 0; encounterTableFileIdx < encounterTableMonoBehaviours.Length; encounterTableFileIdx++)
            {
                EncounterTableFile encounterTableFile = new();
                encounterTableFile.mName = encounterTableMonoBehaviours[encounterTableFileIdx]["m_Name"].AsString;

                //Parse wild encounter tables
                encounterTableFile.encounterTables = new();
                var encounterTableFields = encounterTableMonoBehaviours[encounterTableFileIdx]["table.Array"].Children;
                for (int encounterTableIdx = 0; encounterTableIdx < encounterTableFields.Count; encounterTableIdx++)
                {
                    EncounterTable encounterTable = new();
                    encounterTable.zoneID = (ZoneID)encounterTableFields[encounterTableIdx]["zoneID"].AsInt;
                    encounterTable.encRateGround = encounterTableFields[encounterTableIdx]["encRate_gr"].AsInt;
                    encounterTable.formProb = encounterTableFields[encounterTableIdx]["FormProb.Array"][0].AsInt;
                    encounterTable.unownTable = encounterTableFields[encounterTableIdx]["AnnoonTable.Array"][1].AsInt;
                    encounterTable.encRateWater = encounterTableFields[encounterTableIdx]["encRate_wat"].AsInt;
                    encounterTable.encRateOldRod = encounterTableFields[encounterTableIdx]["encRate_turi_boro"].AsInt;
                    encounterTable.encRateGoodRod = encounterTableFields[encounterTableIdx]["encRate_turi_ii"].AsInt;
                    encounterTable.encRateSuperRod = encounterTableFields[encounterTableIdx]["encRate_sugoi"].AsInt;

                    //Parse ground tables
                    encounterTable.groundMons = GetParsedEncounters(encounterTableFields[encounterTableIdx]["ground_mons.Array"].Children);

                    //Parse morning tables
                    encounterTable.tairyo = GetParsedEncounters(encounterTableFields[encounterTableIdx]["tairyo.Array"].Children);

                    //Parse day tables
                    encounterTable.day = GetParsedEncounters(encounterTableFields[encounterTableIdx]["day.Array"].Children);

                    //Parse night tables
                    encounterTable.night = GetParsedEncounters(encounterTableFields[encounterTableIdx]["night.Array"].Children);

                    //Parse pokefinder tables
                    encounterTable.swayGrass = GetParsedEncounters(encounterTableFields[encounterTableIdx]["swayGrass.Array"].Children);

                    //Parse ruby tables
                    encounterTable.gbaRuby = GetParsedEncounters(encounterTableFields[encounterTableIdx]["gbaRuby.Array"].Children);

                    //Parse sapphire tables
                    encounterTable.gbaSapphire = GetParsedEncounters(encounterTableFields[encounterTableIdx]["gbaSapp.Array"].Children);

                    //Parse emerald tables
                    encounterTable.gbaEmerald = GetParsedEncounters(encounterTableFields[encounterTableIdx]["gbaEme.Array"].Children);

                    //Parse fire tables
                    encounterTable.gbaFire = GetParsedEncounters(encounterTableFields[encounterTableIdx]["gbaFire.Array"].Children);

                    //Parse leaf tables
                    encounterTable.gbaLeaf = GetParsedEncounters(encounterTableFields[encounterTableIdx]["gbaLeaf.Array"].Children);

                    //Parse surfing tables
                    encounterTable.waterMons = GetParsedEncounters(encounterTableFields[encounterTableIdx]["water_mons.Array"].Children);

                    //Parse old rod tables
                    encounterTable.oldRodMons = GetParsedEncounters(encounterTableFields[encounterTableIdx]["boro_mons.Array"].Children);

                    //Parse good rod tables
                    encounterTable.goodRodMons = GetParsedEncounters(encounterTableFields[encounterTableIdx]["ii_mons.Array"].Children);

                    //Parse super rod tables
                    encounterTable.superRodMons = GetParsedEncounters(encounterTableFields[encounterTableIdx]["sugoi_mons.Array"].Children);

                    encounterTableFile.encounterTables.Add(encounterTable);
                }

                //Parse trophy garden table
                encounterTableFile.trophyGardenMons = new();
                var trophyGardenMonFields = encounterTableMonoBehaviours[encounterTableFileIdx]["urayama.Array"].Children;
                for (int trophyGardenMonIdx = 0; trophyGardenMonIdx < trophyGardenMonFields.Count; trophyGardenMonIdx++)
                    encounterTableFile.trophyGardenMons.Add(trophyGardenMonFields[trophyGardenMonIdx]["monsNo"].AsInt);

                //Parse honey tree tables
                encounterTableFile.honeyTreeEnconters = new();
                var honeyTreeEncounterFields = encounterTableMonoBehaviours[encounterTableFileIdx]["mistu.Array"].Children;
                for (int honeyTreeEncounterIdx = 0; honeyTreeEncounterIdx < honeyTreeEncounterFields.Count; honeyTreeEncounterIdx++)
                {
                    HoneyTreeEncounter honeyTreeEncounter = new();
                    honeyTreeEncounter.rate = honeyTreeEncounterFields[honeyTreeEncounterIdx]["Rate"].AsInt;
                    honeyTreeEncounter.normalDexID = honeyTreeEncounterFields[honeyTreeEncounterIdx]["Normal"].AsInt;
                    honeyTreeEncounter.rareDexID = honeyTreeEncounterFields[honeyTreeEncounterIdx]["Rare"].AsInt;
                    honeyTreeEncounter.superRareDexID = honeyTreeEncounterFields[honeyTreeEncounterIdx]["SuperRare"].AsInt;

                    encounterTableFile.honeyTreeEnconters.Add(honeyTreeEncounter);
                }

                //Parse safari table
                encounterTableFile.safariMons = new();
                var safariMonFields = encounterTableMonoBehaviours[encounterTableFileIdx]["safari.Array"].Children;
                for (int safariMonIdx = 0; safariMonIdx < safariMonFields.Count; safariMonIdx++)
                    encounterTableFile.safariMons.Add(safariMonFields[safariMonIdx]["MonsNo"].AsInt);

                gameData.encounterTableFiles[encounterTableFileIdx] = encounterTableFile;
            }
        }

        /// <summary>
        ///  Parses an array of encounters in a monobehaviour into a list of Encounters.
        /// </summary>
        private static List<Encounter> GetParsedEncounters(List<AssetTypeValueField> encounterFields)
        {
            List<Encounter> encounters = new();
            for (int encounterIdx = 0; encounterIdx < encounterFields.Count; encounterIdx++)
            {
                Encounter encounter = new();
                encounter.maxLv = encounterFields[encounterIdx]["maxlv"].AsInt;
                encounter.minLv = encounterFields[encounterIdx]["minlv"].AsInt;
                encounter.dexID = encounterFields[encounterIdx]["monsNo"].AsInt;

                encounters.Add(encounter);
            }
            return encounters;
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed DexEntries and PersonalEntries.
        /// </summary>
        private static async Task ParsePokemon()
        {
            gameData.dexEntries = new();
            gameData.personalEntries = new();
            List<AssetTypeValueField> monoBehaviours = (await monoBehaviourCollection[PathEnum.PersonalMasterdatas]);

            var levelUpMoveFields = monoBehaviours.Find(m => m["m_Name"].AsString == "WazaOboeTable")["WazaOboe.Array"].Children;
            var eggMoveFields = monoBehaviours.Find(m => m["m_Name"].AsString == "TamagoWazaTable")["Data.Array"].Children;
            var evolveFields = monoBehaviours.Find(m => m["m_Name"].AsString == "EvolveTable")["Evolve.Array"].Children;
            var personalFields = monoBehaviours.Find(m => m["m_Name"].AsString == "PersonalTable")["Personal.Array"].Children;
            var textFields = await FindLabelArrayOfMessageFileAsync("ss_monsname", Language.English);

            if (levelUpMoveFields.Count < personalFields.Count)
                MainForm.ShowParserError("Oh my, this WazaOboeTable is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "PersonalTable entries: " + personalFields.Count + "\n" +
                    "WazaOboeTable entries: " + levelUpMoveFields.Count + "??");
            if (eggMoveFields.Count < personalFields.Count)
                MainForm.ShowParserError("Oh my, this TamagoWazaTable is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "PersonalTable entries: " + personalFields.Count + "\n" +
                    "TamagoWazaTable entries: " + eggMoveFields.Count + "??");
            if (evolveFields.Count < personalFields.Count)
                MainForm.ShowParserError("Oh my, this EvolveTable is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "PersonalTable entries: " + personalFields.Count + "\n" +
                    "EvolveTable entries: " + evolveFields.Count + "??");

            for (int personalID = 0; personalID < personalFields.Count; personalID++)
            {
                Pokemon pokemon = new();
                pokemon.validFlag = personalFields[personalID]["valid_flag"].AsByte;
                pokemon.personalID = personalFields[personalID]["id"].AsUShort;
                pokemon.dexID = personalFields[personalID]["monsno"].AsUShort; 
                pokemon.formIndex = personalFields[personalID]["form_index"].AsUShort;
                pokemon.formMax = personalFields[personalID]["form_max"].AsByte;
                pokemon.color = personalFields[personalID]["color"].AsByte;
                pokemon.graNo = personalFields[personalID]["gra_no"].AsUShort;
                pokemon.basicHp = personalFields[personalID]["basic_hp"].AsByte;
                pokemon.basicAtk = personalFields[personalID]["basic_atk"].AsByte;
                pokemon.basicDef = personalFields[personalID]["basic_def"].AsByte;
                pokemon.basicSpd = personalFields[personalID]["basic_agi"].AsByte;
                pokemon.basicSpAtk = personalFields[personalID]["basic_spatk"].AsByte;
                pokemon.basicSpDef = personalFields[personalID]["basic_spdef"].AsByte;
                pokemon.typingID1 = personalFields[personalID]["type1"].AsByte;
                pokemon.typingID2 = personalFields[personalID]["type2"].AsByte;
                pokemon.getRate = personalFields[personalID]["get_rate"].AsByte;
                pokemon.rank = personalFields[personalID]["rank"].AsByte;
                pokemon.expValue = personalFields[personalID]["exp_value"].AsUShort;
                pokemon.item1 = personalFields[personalID]["item1"].AsUShort;
                pokemon.item2 = personalFields[personalID]["item2"].AsUShort;
                pokemon.item3 = personalFields[personalID]["item3"].AsUShort;
                pokemon.sex = personalFields[personalID]["sex"].AsByte;
                pokemon.eggBirth = personalFields[personalID]["egg_birth"].AsByte;
                pokemon.initialFriendship = personalFields[personalID]["initial_friendship"].AsByte;
                pokemon.eggGroup1 = personalFields[personalID]["egg_group1"].AsByte;
                pokemon.eggGroup2 = personalFields[personalID]["egg_group2"].AsByte;
                pokemon.grow = personalFields[personalID]["grow"].AsByte;
                pokemon.abilityID1 = personalFields[personalID]["tokusei1"].AsUShort;
                pokemon.abilityID2 = personalFields[personalID]["tokusei2"].AsUShort;
                pokemon.abilityID3 = personalFields[personalID]["tokusei3"].AsUShort;
                pokemon.giveExp = personalFields[personalID]["give_exp"].AsUShort;
                pokemon.height = personalFields[personalID]["height"].AsUShort;
                pokemon.weight = personalFields[personalID]["weight"].AsUShort;
                pokemon.chihouZukanNo = personalFields[personalID]["chihou_zukan_no"].AsUShort;
                pokemon.machine1 = personalFields[personalID]["machine1"].AsUInt;
                pokemon.machine2 = personalFields[personalID]["machine2"].AsUInt;
                pokemon.machine3 = personalFields[personalID]["machine3"].AsUInt;
                pokemon.machine4 = personalFields[personalID]["machine4"].AsUInt;
                pokemon.hiddenMachine = personalFields[personalID]["hiden_machine"].AsUInt;
                pokemon.eggMonsno = personalFields[personalID]["egg_monsno"].AsUShort;
                pokemon.eggFormno = personalFields[personalID]["egg_formno"].AsUShort;
                pokemon.eggFormnoKawarazunoishi = personalFields[personalID]["egg_formno_kawarazunoishi"].AsUShort;
                pokemon.eggFormInheritKawarazunoishi = personalFields[personalID]["egg_form_inherit_kawarazunoishi"].AsByte;

                pokemon.formID = 0;
                if (pokemon.personalID != pokemon.dexID)
                    pokemon.formID = pokemon.personalID - pokemon.formIndex + 1;
                pokemon.name = "";
                if (textFields[pokemon.dexID]["wordDataArray.Array"].Children.Count > 0)
                    pokemon.name = textFields[pokemon.dexID]["wordDataArray.Array"][0]["str"].AsString;
                pokemon.nextEvoLvs = (ushort.MaxValue, ushort.MaxValue); //(wildLevel, trainerLevel)
                pokemon.pastPokemon = new();
                pokemon.nextPokemon = new();
                pokemon.inferiorForms = new();
                pokemon.superiorForms = new();

                //Parse level up moves
                pokemon.levelUpMoves = new();
                for (int levelUpMoveIdx = 0; levelUpMoveIdx < levelUpMoveFields[personalID]["ar.Array"].Children.Count; levelUpMoveIdx += 2)
                {
                    LevelUpMove levelUpMove = new();
                    levelUpMove.level = levelUpMoveFields[personalID]["ar.Array"][levelUpMoveIdx].AsUShort;
                    levelUpMove.moveID = levelUpMoveFields[personalID]["ar.Array"][levelUpMoveIdx + 1].AsUShort;

                    pokemon.levelUpMoves.Add(levelUpMove);
                }

                //Parse egg moves
                pokemon.eggMoves = new();
                for (int eggMoveIdx = 0; eggMoveIdx < eggMoveFields[personalID]["wazaNo.Array"].Children.Count; eggMoveIdx++)
                    pokemon.eggMoves.Add(eggMoveFields[personalID]["wazaNo.Array"][eggMoveIdx].AsUShort);

                //Parse evolutions
                pokemon.evolutionPaths = new();
                for (int evolutionIdx = 0; evolutionIdx < evolveFields[personalID]["ar.Array"].Children.Count; evolutionIdx += 5)
                {
                    EvolutionPath evolution = new();
                    evolution.method = evolveFields[personalID]["ar.Array"][evolutionIdx].AsUShort;
                    evolution.parameter = evolveFields[personalID]["ar.Array"][evolutionIdx + 1].AsUShort;
                    evolution.destDexID = evolveFields[personalID]["ar.Array"][evolutionIdx + 2].AsUShort;
                    evolution.destFormID = evolveFields[personalID]["ar.Array"][evolutionIdx + 3].AsUShort;
                    evolution.level = evolveFields[personalID]["ar.Array"][evolutionIdx + 4].AsUShort;

                    pokemon.evolutionPaths.Add(evolution);
                }

                pokemon.externalTMLearnset = fileManager.TryGetExternalJson<TMLearnset>(
                    $"MonData\\TMLearnset\\monsno_{pokemon.dexID}_formno_{pokemon.formID}.json");

                gameData.personalEntries.Add(pokemon);

                if (gameData.dexEntries.Count == pokemon.dexID)
                {
                    gameData.dexEntries.Add(new());
                    gameData.dexEntries[pokemon.dexID].dexID = pokemon.dexID;
                    gameData.dexEntries[pokemon.dexID].forms = new();
                    gameData.dexEntries[pokemon.dexID].name = pokemon.name;
                }

                gameData.dexEntries[pokemon.dexID].forms.Add(pokemon);
            }

            SetFamilies();
            SetLegendaries();
        }

        private static async void SetLegendaries()
        {
            var legendFields = (await monoBehaviourCollection[PathEnum.Gamesettings]).Find(m => m["m_Name"].AsString == "FieldEncountTable_d")["legendpoke.Array"].Children;
            for (int legendEntryIdx = 0; legendEntryIdx < legendFields.Count; legendEntryIdx++)
            {
                List<Pokemon> forms = gameData.dexEntries[legendFields[legendEntryIdx]["monsNo"].AsInt].forms;
                for (int formID = 0; formID < forms.Count; formID++)
                    forms[formID].legendary = true;
            }
        }

        /// <summary>
        ///  Overwrites and updates all pokemons' evolution info for easier BST logic.
        /// </summary>
        public static void SetFamilies()
        {
            for (int dexID = 0; dexID < gameData.dexEntries.Count; dexID++)
            {
                for (int formID = 0; formID < gameData.dexEntries[dexID].forms.Count; formID++)
                {
                    Pokemon pokemon = gameData.dexEntries[dexID].forms[formID];
                    for (int evolutionIdx = 0; evolutionIdx < gameData.dexEntries[dexID].forms[formID].evolutionPaths.Count; evolutionIdx++)
                    {
                        EvolutionPath evo = pokemon.evolutionPaths[evolutionIdx];
                        Pokemon next = gameData.dexEntries[evo.destDexID].forms[evo.destFormID];

                        if (pokemon.dexID == next.dexID)
                            continue;

                        pokemon.nextPokemon.Add(next);
                        next.pastPokemon.Add(pokemon);
                    }

                    for (int formID2 = 0; formID2 < gameData.dexEntries[dexID].forms.Count; formID2++)
                    {
                        Pokemon pokemon2 = gameData.dexEntries[dexID].forms[formID2];
                        if (pokemon2.GetBST() - pokemon.GetBST() >= 30)
                        {
                            pokemon2.inferiorForms.Add(pokemon);
                            pokemon.superiorForms.Add(pokemon2);
                        }
                    }
                }
            }

            for (int dexID = 0; dexID < gameData.dexEntries.Count; dexID++)
            {
                for (int formID = 0; formID < gameData.dexEntries[dexID].forms.Count; formID++)
                {
                    Pokemon pokemon = gameData.dexEntries[dexID].forms[formID];
                    (ushort, ushort) evoLvs = GetEvoLvs(pokemon);
                    if (evoLvs == (0, 0))
                        continue;

                    pokemon.nextEvoLvs = evoLvs;

                    for (int evolutionIdx = 0; evolutionIdx < gameData.dexEntries[dexID].forms[formID].evolutionPaths.Count; evolutionIdx++)
                    {
                        EvolutionPath evo = pokemon.evolutionPaths[evolutionIdx];
                        Pokemon next = gameData.dexEntries[evo.destDexID].forms[evo.destFormID];

                        if (pokemon.dexID == next.dexID)
                            continue;

                        next.pastEvoLvs.Item1 = Math.Max(next.pastEvoLvs.Item1, evoLvs.Item1);
                        next.pastEvoLvs.Item2 = Math.Max(next.pastEvoLvs.Item2, evoLvs.Item2);
                    }
                }
            }
        }

        /// <summary>
        ///  Finds the levels the specified pokemon is likely to evolve: (wildLevel, trainerLevel).
        /// </summary>
        private static (ushort, ushort) GetEvoLvs(Pokemon pokemon)
        {
            (ushort, ushort) evoLvs = (0, 0);
            for (int evolutionIdx = 0; evolutionIdx < pokemon.evolutionPaths.Count; evolutionIdx++)
            {
                EvolutionPath evo = pokemon.evolutionPaths[evolutionIdx];
                if (pokemon.dexID == evo.destDexID)
                    continue;

                switch (evo.method)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 21:
                    case 29:
                    case 43:
                        for (int pastEvos = 0; pastEvos < pokemon.pastPokemon.Count; pastEvos++)
                        {
                            (ushort, ushort) pastPokemonEvoLvs = GetEvoLvs(pokemon.pastPokemon[pastEvos]);
                            evoLvs.Item1 = Math.Max(evoLvs.Item1, pastPokemonEvoLvs.Item1);
                            evoLvs.Item2 = Math.Max(evoLvs.Item2, pastPokemonEvoLvs.Item2);
                        }
                        if (evoLvs == (0, 0))
                            evoLvs = (1, 1);
                        evoLvs.Item1 += 16;
                        evoLvs.Item2 += 16;
                        break;
                    case 4:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 23:
                    case 24:
                    case 28:
                    case 32:
                    case 33:
                    case 34:
                    case 36:
                    case 37:
                    case 38:
                    case 40:
                    case 41:
                    case 46:
                    case 47:
                        evoLvs.Item1 = evo.level;
                        evoLvs.Item2 = evo.level;
                        break;
                    case 5:
                    case 8:
                    case 17:
                    case 18:
                    case 22:
                    case 25:
                    case 26:
                    case 27:
                    case 39:
                    case 42:
                    case 44:
                    case 45:
                        for (int pastEvos = 0; pastEvos < pokemon.pastPokemon.Count; pastEvos++)
                        {
                            (ushort, ushort) pastPokemonEvoLvs = GetEvoLvs(pokemon.pastPokemon[pastEvos]);
                            evoLvs.Item1 = Math.Max(evoLvs.Item1, pastPokemonEvoLvs.Item1);
                            evoLvs.Item2 = Math.Max(evoLvs.Item2, pastPokemonEvoLvs.Item2);
                        }
                        if (evoLvs == (0, 0))
                            evoLvs = (1, 1);
                        evoLvs.Item1 += 32;
                        evoLvs.Item2 += 16;
                        break;
                    case 6:
                    case 7:
                        for (int pastEvos = 0; pastEvos < pokemon.pastPokemon.Count; pastEvos++)
                        {
                            (ushort, ushort) pastPokemonEvoLvs = GetEvoLvs(pokemon.pastPokemon[pastEvos]);
                            evoLvs.Item1 = Math.Max(evoLvs.Item1, pastPokemonEvoLvs.Item1);
                            evoLvs.Item2 = Math.Max(evoLvs.Item2, pastPokemonEvoLvs.Item2);
                        }
                        if (evoLvs == (0, 0))
                            evoLvs = (1, 1);
                        evoLvs.Item1 += 48;
                        evoLvs.Item2 += 16;
                        break;
                    case 16:
                        for (int pastEvos = 0; pastEvos < pokemon.pastPokemon.Count; pastEvos++)
                        {
                            (ushort, ushort) pastPokemonEvoLvs = GetEvoLvs(pokemon.pastPokemon[pastEvos]);
                            evoLvs.Item1 = Math.Max(evoLvs.Item1, pastPokemonEvoLvs.Item1);
                            evoLvs.Item2 = Math.Max(evoLvs.Item2, pastPokemonEvoLvs.Item2);
                        }
                        if (evoLvs == (0, 0))
                            evoLvs = (1, 1);
                        evoLvs.Item1 += 48;
                        evoLvs.Item2 += 32;
                        break;
                    case 19:
                    case 20:
                    case 30:
                    case 31:
                        evoLvs.Item1 = (ushort)(evo.level + 16);
                        evoLvs.Item2 = evo.level;
                        break;
                }
            }
            return evoLvs;
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed TMs.
        /// </summary>
        private static async Task ParseTMs()
        {
            gameData.tms = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.PersonalMasterdatas]).Find(m => m["m_Name"].AsString == "ItemTable");

            var tmFields = monoBehaviour["WazaMachine.Array"].Children;
            var textFields = await FindLabelArrayOfMessageFileAsync("ss_itemname", Language.English);
            for (int tmID = 0; tmID < tmFields.Count; tmID++)
            {
                TM tm = new();
                tm.itemID = tmFields[tmID]["itemNo"].AsInt;
                tm.machineNo = tmFields[tmID]["machineNo"].AsInt;
                tm.moveID = tmFields[tmID]["wazaNo"].AsInt;

                tm.tmID = tmID;
                tm.name = "";
                if (textFields[tm.itemID]["wordDataArray.Array"].Children.Count > 0)
                    tm.name = textFields[tm.itemID]["wordDataArray.Array"][0]["str"].AsString;

                gameData.tms.Add(tm);
            }
        }
        private static async Task ParsePersonalMasterDatas()
        {
            gameData.addPersonalTables = new();
            AssetTypeValueField addPersonalTable = (await monoBehaviourCollection[PathEnum.PersonalMasterdatas]).Find(m => m["m_Name"].AsString == "AddPersonalTable");
            var addPersonalTableArray = addPersonalTable["AddPersonal.Array"].Children;
            for (int i = 0; i < addPersonalTableArray.Count; i++)
            {
                PersonalMasterdatas.AddPersonalTable addPersonal = new();
                addPersonal.valid_flag = addPersonalTableArray[i]["valid_flag"].AsByte == 1;
                addPersonal.monsno = addPersonalTableArray[i]["monsno"].AsUShort;
                addPersonal.formno = addPersonalTableArray[i]["formno"].AsUShort;
                addPersonal.isEnableSynchronize = addPersonalTableArray[i]["isEnableSynchronize"].AsByte == 1;
                addPersonal.escape = addPersonalTableArray[i]["escape"].AsByte;
                addPersonal.isDisableReverce = addPersonalTableArray[i]["isDisableReverce"].AsByte == 1;
                gameData.addPersonalTables.Add(addPersonal);
            }
        }

        private static async Task ParseUIMasterDatas()
        {
            gameData.uiPokemonIcon = new();
            gameData.uiAshiatoIcon = new();
            gameData.uiPokemonVoice = new();
            gameData.uiZukanDisplay = new();
            gameData.uiZukanCompareHeights = new();
            gameData.uiSearchPokeIconSex = new();
            gameData.uiDistributionTable = new();
            AssetTypeValueField uiDatabase = (await monoBehaviourCollection[PathEnum.UIMasterdatas]).Find(m => m["m_Name"].AsString == "UIDatabase");
            AssetTypeValueField distributionTable = (await monoBehaviourCollection[PathEnum.UIMasterdatas]).Find(m => m["m_Name"].AsString == "DistributionTable");

            var pokemonIcons = uiDatabase["PokemonIcon.Array"].Children;
            for (int i = 0; i < pokemonIcons.Count; i++)
            {
                UIMasterdatas.PokemonIcon pokemonIcon = new();
                pokemonIcon.uniqueID = pokemonIcons[i]["UniqueID"].AsInt;
                pokemonIcon.assetBundleName = pokemonIcons[i]["AssetBundleName"].AsString;
                pokemonIcon.assetName = pokemonIcons[i]["AssetName"].AsString;
                pokemonIcon.assetBundleNameLarge = pokemonIcons[i]["AssetBundleNameLarge"].AsString;
                pokemonIcon.assetNameLarge = pokemonIcons[i]["AssetNameLarge"].AsString;
                pokemonIcon.assetBundleNameDP = pokemonIcons[i]["AssetBundleNameDP"].AsString;
                pokemonIcon.assetNameDP = pokemonIcons[i]["AssetNameDP"].AsString;
                pokemonIcon.hallofFameOffset = new();
                pokemonIcon.hallofFameOffset.X = pokemonIcons[i]["HallofFameOffset.x"].AsFloat;
                pokemonIcon.hallofFameOffset.Y = pokemonIcons[i]["HallofFameOffset.y"].AsFloat;

                gameData.uiPokemonIcon.Add(pokemonIcon);
            }

            var ashiatoIcons = uiDatabase["AshiatoIcon.Array"].Children;
            for (int i = 0; i < ashiatoIcons.Count; i++)
            {
                UIMasterdatas.AshiatoIcon ashiatoIcon = new();
                ashiatoIcon.uniqueID = ashiatoIcons[i]["UniqueID"].AsInt;
                ashiatoIcon.sideIconAssetName = ashiatoIcons[i]["SideIconAssetName"].AsString;
                ashiatoIcon.bothIconAssetName = ashiatoIcons[i]["BothIconAssetName"].AsString;

                gameData.uiAshiatoIcon.Add(ashiatoIcon);
            }

            var pokemonVoices = uiDatabase["PokemonVoice.Array"].Children;
            for (int i = 0; i < pokemonVoices.Count; i++)
            {
                UIMasterdatas.PokemonVoice pokemonVoice = new();
                pokemonVoice.uniqueID = pokemonVoices[i]["UniqueID"].AsInt;
                pokemonVoice.wwiseEvent = pokemonVoices[i]["WwiseEvent"].AsString;
                pokemonVoice.stopEventId = pokemonVoices[i]["stopEventId"].AsString;
                pokemonVoice.centerPointOffset = new();
                pokemonVoice.centerPointOffset.X = pokemonVoices[i]["CenterPointOffset.x"].AsFloat;
                pokemonVoice.centerPointOffset.Y = pokemonVoices[i]["CenterPointOffset.y"].AsFloat;
                pokemonVoice.centerPointOffset.Z = pokemonVoices[i]["CenterPointOffset.z"].AsFloat;
                pokemonVoice.rotationLimits = pokemonVoices[i]["RotationLimits"].AsByte == 1;
                pokemonVoice.rotationLimitAngle = new();
                pokemonVoice.rotationLimitAngle.X = pokemonVoices[i]["RotationLimitAngle.x"].AsFloat;
                pokemonVoice.rotationLimitAngle.Y = pokemonVoices[i]["RotationLimitAngle.y"].AsFloat;

                gameData.uiPokemonVoice.Add(pokemonVoice);
            }

            var zukanDisplays = uiDatabase["ZukanDisplay.Array"].Children;
            for (int i = 0; i < zukanDisplays.Count; i++)
            {
                UIMasterdatas.ZukanDisplay zukanDisplay = new();
                zukanDisplay.uniqueID = zukanDisplays[i]["UniqueID"].AsInt;

                zukanDisplay.moveLimit = new();
                zukanDisplay.moveLimit.X = zukanDisplays[i]["MoveLimit.x"].AsFloat;
                zukanDisplay.moveLimit.Y = zukanDisplays[i]["MoveLimit.y"].AsFloat;
                zukanDisplay.moveLimit.Z = zukanDisplays[i]["MoveLimit.z"].AsFloat;

                zukanDisplay.modelOffset = new();
                zukanDisplay.modelOffset.X = zukanDisplays[i]["ModelOffset.x"].AsFloat;
                zukanDisplay.modelOffset.Y = zukanDisplays[i]["ModelOffset.y"].AsFloat;
                zukanDisplay.modelOffset.Z = zukanDisplays[i]["ModelOffset.z"].AsFloat;

                zukanDisplay.modelRotationAngle = new();
                zukanDisplay.modelRotationAngle.X = zukanDisplays[i]["ModelRotationAngle.x"].AsFloat;
                zukanDisplay.modelRotationAngle.Y = zukanDisplays[i]["ModelRotationAngle.y"].AsFloat;

                gameData.uiZukanDisplay.Add(zukanDisplay);
            }

            var zukanCompareHeights = uiDatabase["ZukanCompareHeight.Array"].Children;
            for (int i = 0; i < zukanCompareHeights.Count; i++)
            {
                UIMasterdatas.ZukanCompareHeight zukanCompareHeight = new();
                zukanCompareHeight.uniqueID = zukanCompareHeights[i]["UniqueID"].AsInt;

                zukanCompareHeight.playerScaleFactor = zukanCompareHeights[i]["PlayerScaleFactor"].AsFloat;
                zukanCompareHeight.playerOffset = new();
                zukanCompareHeight.playerOffset.X = zukanCompareHeights[i]["PlayerOffset.x"].AsFloat;
                zukanCompareHeight.playerOffset.Y = zukanCompareHeights[i]["PlayerOffset.y"].AsFloat;
                zukanCompareHeight.playerOffset.Z = zukanCompareHeights[i]["PlayerOffset.z"].AsFloat;

                zukanCompareHeight.playerRotationAngle = new();
                zukanCompareHeight.playerRotationAngle.X = zukanCompareHeights[i]["PlayerRotationAngle.x"].AsFloat;
                zukanCompareHeight.playerRotationAngle.Y = zukanCompareHeights[i]["PlayerRotationAngle.y"].AsFloat;

                gameData.uiZukanCompareHeights.Add(zukanCompareHeight);
            }

            var searchPokeIconSexes = uiDatabase["SearchPokeIconSex.Array"].Children;
            for (int i = 0; i < searchPokeIconSexes.Count; i++)
            {
                UIMasterdatas.SearchPokeIconSex searchPokeIconSex = new();
                searchPokeIconSex.monsNo = searchPokeIconSexes[i]["MonsNo"].AsInt;
                searchPokeIconSex.sex = searchPokeIconSexes[i]["Sex"].AsInt;

                gameData.uiSearchPokeIconSex.Add(searchPokeIconSex);
            }

            gameData.uiDistributionTable.diamondFieldTable = ParseDistributionSheet(distributionTable["Diamond_FieldTable.Array"].Children);
            gameData.uiDistributionTable.diamondDungeonTable = ParseDistributionSheet(distributionTable["Diamond_DungeonTable.Array"].Children);
            gameData.uiDistributionTable.pearlFieldTable = ParseDistributionSheet(distributionTable["Pearl_FieldTable.Array"].Children);
            gameData.uiDistributionTable.pearlDungeonTable = ParseDistributionSheet(distributionTable["Pearl_DungeonTable.Array"].Children);
        }

        private static List<UIMasterdatas.DistributionEntry> ParseDistributionSheet(List<AssetTypeValueField> sheetATVF)
        {
            List<UIMasterdatas.DistributionEntry> sheet = new();
            for (int i = 0; i < sheetATVF.Count; i++)
            {
                UIMasterdatas.DistributionEntry entry = new()
                {
                    beforeMorning = ParseDistributionCoord(sheetATVF[i]["BeforeMorning"]),
                    beforeDaytime = ParseDistributionCoord(sheetATVF[i]["BeforeDaytime"]),
                    beforeNight = ParseDistributionCoord(sheetATVF[i]["BeforeNight"]),
                    afterMorning = ParseDistributionCoord(sheetATVF[i]["AfterMorning"]),
                    afterDaytime = ParseDistributionCoord(sheetATVF[i]["AfterDaytime"]),
                    afterNight = ParseDistributionCoord(sheetATVF[i]["AfterNight"]),
                    fishing = ParseDistributionCoord(sheetATVF[i]["Fishing"]),
                    pokemonTraser = ParseDistributionCoord(sheetATVF[i]["PokemonTraser"]),
                    honeyTree = ParseDistributionCoord(sheetATVF[i]["HoneyTree"])
                };
                sheet.Add(entry);
            }
            return sheet;
        }

        private static int[] ParseDistributionCoord(AssetTypeValueField posATVF) => posATVF["Array"].Children.Select(a => a.AsInt).ToArray();

        private static async Task ParseMasterDatas()
        {
            gameData.pokemonInfos = new();
            AssetTypeValueField pokemonInfo = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => m["m_Name"].AsString == "PokemonInfo");
            var catalogArray = pokemonInfo["Catalog.Array"].Children;
            List<AssetTypeValueField> trearukiArray = null;
            if (!pokemonInfo["Trearuki"].IsDummy)
                trearukiArray = pokemonInfo["Trearuki.Array"].Children;
            //pre-1.3.0 versions don't have this field

            for (int i = 0; i < catalogArray.Count; i++)
            {
                Masterdatas.PokemonInfoCatalog catalog = new();
                catalog.UniqueID = catalogArray[i]["UniqueID"].AsInt;
                catalog.No = catalogArray[i]["No"].AsInt;
                catalog.SinnohNo = catalogArray[i]["SinnohNo"].AsInt;
                catalog.MonsNo = catalogArray[i]["MonsNo"].AsInt;
                catalog.FormNo = catalogArray[i]["FormNo"].AsInt;
                catalog.Sex = catalogArray[i]["Sex"].AsByte;
                catalog.Rare = catalogArray[i]["Rare"].AsByte == 1;
                catalog.AssetBundleName = catalogArray[i]["AssetBundleName"].AsString;
                catalog.BattleScale = catalogArray[i]["BattleScale"].AsFloat;
                catalog.ContestScale = catalogArray[i]["ContestScale"].AsFloat;
                catalog.ContestSize = (Masterdatas.Size)catalogArray[i]["ContestSize"].AsInt;
                catalog.FieldScale = catalogArray[i]["FieldScale"].AsFloat;
                catalog.FieldChikaScale = catalogArray[i]["FieldChikaScale"].AsFloat;
                catalog.StatueScale = catalogArray[i]["StatueScale"].AsFloat;
                catalog.FieldWalkingScale = catalogArray[i]["FieldWalkingScale"].AsFloat;
                catalog.FieldFureaiScale = catalogArray[i]["FieldFureaiScale"].AsFloat;
                catalog.MenuScale = catalogArray[i]["MenuScale"].AsFloat;
                catalog.ModelMotion = catalogArray[i]["ModelMotion"].AsString;

                catalog.ModelOffset = new();
                catalog.ModelOffset.X = catalogArray[i]["ModelOffset.x"].AsFloat;
                catalog.ModelOffset.Y = catalogArray[i]["ModelOffset.y"].AsFloat;
                catalog.ModelOffset.Z = catalogArray[i]["ModelOffset.z"].AsFloat;

                catalog.ModelRotationAngle = new();
                catalog.ModelRotationAngle.X = catalogArray[i]["ModelRotationAngle.x"].AsFloat;
                catalog.ModelRotationAngle.Y = catalogArray[i]["ModelRotationAngle.y"].AsFloat;
                catalog.ModelRotationAngle.Z = catalogArray[i]["ModelRotationAngle.z"].AsFloat;

                catalog.DistributionScale = catalogArray[i]["DistributionScale"].AsFloat;
                catalog.DistributionModelMotion = catalogArray[i]["DistributionModelMotion"].AsString;

                catalog.DistributionModelOffset = new();
                catalog.DistributionModelOffset.X = catalogArray[i]["DistributionModelOffset.x"].AsFloat;
                catalog.DistributionModelOffset.Y = catalogArray[i]["DistributionModelOffset.y"].AsFloat;
                catalog.DistributionModelOffset.Z = catalogArray[i]["DistributionModelOffset.z"].AsFloat;

                catalog.DistributionModelRotationAngle = new();
                catalog.DistributionModelRotationAngle.X = catalogArray[i]["DistributionModelRotationAngle.x"].AsFloat;
                catalog.DistributionModelRotationAngle.Y = catalogArray[i]["DistributionModelRotationAngle.y"].AsFloat;
                catalog.DistributionModelRotationAngle.Z = catalogArray[i]["DistributionModelRotationAngle.z"].AsFloat;

                catalog.VoiceScale = catalogArray[i]["VoiceScale"].AsFloat;
                catalog.VoiceModelMotion = catalogArray[i]["VoiceModelMotion"].AsString;

                catalog.VoiceModelOffset = new();
                catalog.VoiceModelOffset.X = catalogArray[i]["VoiceModelOffset.x"].AsFloat;
                catalog.VoiceModelOffset.Y = catalogArray[i]["VoiceModelOffset.y"].AsFloat;
                catalog.VoiceModelOffset.Z = catalogArray[i]["VoiceModelOffset.z"].AsFloat;

                catalog.VoiceModelRotationAngle = new();
                catalog.VoiceModelRotationAngle.X = catalogArray[i]["VoiceModelRotationAngle.x"].AsFloat;
                catalog.VoiceModelRotationAngle.Y = catalogArray[i]["VoiceModelRotationAngle.y"].AsFloat;
                catalog.VoiceModelRotationAngle.Z = catalogArray[i]["VoiceModelRotationAngle.z"].AsFloat;

                catalog.CenterPointOffset = new();
                catalog.CenterPointOffset.X = catalogArray[i]["CenterPointOffset.x"].AsFloat;
                catalog.CenterPointOffset.Y = catalogArray[i]["CenterPointOffset.y"].AsFloat;
                catalog.CenterPointOffset.Z = catalogArray[i]["CenterPointOffset.z"].AsFloat;

                catalog.RotationLimitAngle = new();
                catalog.RotationLimitAngle.X = catalogArray[i]["RotationLimitAngle.x"].AsFloat;
                catalog.RotationLimitAngle.Y = catalogArray[i]["RotationLimitAngle.y"].AsFloat;

                catalog.StatusScale = catalogArray[i]["StatusScale"].AsFloat;
                catalog.StatusModelMotion = catalogArray[i]["StatusModelMotion"].AsString;

                catalog.StatusModelOffset = new();
                catalog.StatusModelOffset.X = catalogArray[i]["StatusModelOffset.x"].AsFloat;
                catalog.StatusModelOffset.Y = catalogArray[i]["StatusModelOffset.y"].AsFloat;
                catalog.StatusModelOffset.Z = catalogArray[i]["StatusModelOffset.z"].AsFloat;

                catalog.StatusModelRotationAngle = new();
                catalog.StatusModelRotationAngle.X = catalogArray[i]["StatusModelRotationAngle.x"].AsFloat;
                catalog.StatusModelRotationAngle.Y = catalogArray[i]["StatusModelRotationAngle.y"].AsFloat;
                catalog.StatusModelRotationAngle.Z = catalogArray[i]["StatusModelRotationAngle.z"].AsFloat;

                catalog.BoxScale = catalogArray[i]["BoxScale"].AsFloat;
                catalog.BoxModelMotion = catalogArray[i]["BoxModelMotion"].AsString;

                catalog.BoxModelOffset = new();
                catalog.BoxModelOffset.X = catalogArray[i]["BoxModelOffset.x"].AsFloat;
                catalog.BoxModelOffset.Y = catalogArray[i]["BoxModelOffset.y"].AsFloat;
                catalog.BoxModelOffset.Z = catalogArray[i]["BoxModelOffset.z"].AsFloat;

                catalog.BoxModelRotationAngle = new();
                catalog.BoxModelRotationAngle.X = catalogArray[i]["BoxModelRotationAngle.x"].AsFloat;
                catalog.BoxModelRotationAngle.Y = catalogArray[i]["BoxModelRotationAngle.y"].AsFloat;
                catalog.BoxModelRotationAngle.Z = catalogArray[i]["BoxModelRotationAngle.z"].AsFloat;

                catalog.CompareScale = catalogArray[i]["CompareScale"].AsFloat;
                catalog.CompareModelMotion = catalogArray[i]["CompareModelMotion"].AsString;

                catalog.CompareModelOffset = new();
                catalog.CompareModelOffset.X = catalogArray[i]["CompareModelOffset.x"].AsFloat;
                catalog.CompareModelOffset.Y = catalogArray[i]["CompareModelOffset.y"].AsFloat;
                catalog.CompareModelOffset.Z = catalogArray[i]["CompareModelOffset.z"].AsFloat;

                catalog.CompareModelRotationAngle = new();
                catalog.CompareModelRotationAngle.X = catalogArray[i]["CompareModelRotationAngle.x"].AsFloat;
                catalog.CompareModelRotationAngle.Y = catalogArray[i]["CompareModelRotationAngle.y"].AsFloat;
                catalog.CompareModelRotationAngle.Z = catalogArray[i]["CompareModelRotationAngle.z"].AsFloat;

                catalog.BrakeStart = catalogArray[i]["BrakeStart"].AsFloat;
                catalog.BrakeEnd = catalogArray[i]["BrakeEnd"].AsFloat;
                catalog.WalkSpeed = catalogArray[i]["WalkSpeed"].AsFloat;
                catalog.RunSpeed = catalogArray[i]["RunSpeed"].AsFloat;
                catalog.WalkStart = catalogArray[i]["WalkStart"].AsFloat;
                catalog.RunStart = catalogArray[i]["RunStart"].AsFloat;
                catalog.BodySize = catalogArray[i]["BodySize"].AsFloat;
                catalog.AppearLimit = catalogArray[i]["AppearLimit"].AsFloat;
                catalog.MoveType = (Masterdatas.MoveType)catalogArray[i]["MoveType"].AsInt;

                catalog.GroundEffect = catalogArray[i]["GroundEffect"].AsByte != 0;
                catalog.Waitmoving = catalogArray[i]["Waitmoving"].AsByte != 0;
                catalog.BattleAjustHeight = catalogArray[i]["BattleAjustHeight"].AsInt;

                if (trearukiArray != null)
                {
                    Masterdatas.Trearuki t = new()
                    {
                        enable = trearukiArray[i]["Enable"].AsByte != 0,
                        animeIndex = new(),
                        animeDuration = new()
                    };
                    catalog.trearuki = t;

                    var animeIndexATVFS = trearukiArray[i]["AnimeIndex.Array"].Children;
                    foreach (AssetTypeValueField atvf in animeIndexATVFS)
                        t.animeIndex.Add(atvf.AsInt);

                    var animeDurationATVFS = trearukiArray[i]["AnimeDuration.Array"].Children;
                    foreach (AssetTypeValueField atvf in animeDurationATVFS)
                        t.animeDuration.Add(atvf.AsFloat);
                }

                gameData.pokemonInfos.Add(catalog);
            }
        }
        private static async Task ParseBattleMasterDatas()
        {
            gameData.motionTimingData = new();
            AssetTypeValueField battleDataTable = (await monoBehaviourCollection[PathEnum.BattleMasterdatas]).Find(m => m["m_Name"].AsString == "BattleDataTable");
            var motionTimingDataArray = battleDataTable["MotionTimingData.Array"].Children;

            for (int i = 0; i < motionTimingDataArray.Count; i++)
            {
                BattleMasterdatas.MotionTimingData motionTimingData = new();
                motionTimingData.MonsNo = motionTimingDataArray[i]["MonsNo"].AsInt;
                motionTimingData.FormNo = motionTimingDataArray[i]["FormNo"].AsInt;
                motionTimingData.Sex = motionTimingDataArray[i]["Sex"].AsInt;
                motionTimingData.Buturi01 = motionTimingDataArray[i]["Buturi01"].AsInt;
                motionTimingData.Buturi02 = motionTimingDataArray[i]["Buturi02"].AsInt;
                motionTimingData.Buturi03 = motionTimingDataArray[i]["Buturi03"].AsInt;
                motionTimingData.Tokusyu01 = motionTimingDataArray[i]["Tokusyu01"].AsInt;
                motionTimingData.Tokusyu02 = motionTimingDataArray[i]["Tokusyu02"].AsInt;
                motionTimingData.Tokusyu03 = motionTimingDataArray[i]["Tokusyu03"].AsInt;
                motionTimingData.BodyBlow = motionTimingDataArray[i]["BodyBlow"].AsInt;
                motionTimingData.Punch = motionTimingDataArray[i]["Punch"].AsInt;
                motionTimingData.Kick = motionTimingDataArray[i]["Kick"].AsInt;
                motionTimingData.Tail = motionTimingDataArray[i]["Tail"].AsInt;
                motionTimingData.Bite = motionTimingDataArray[i]["Bite"].AsInt;
                motionTimingData.Peck = motionTimingDataArray[i]["Peck"].AsInt;
                motionTimingData.Radial = motionTimingDataArray[i]["Radial"].AsInt;
                motionTimingData.Cry = motionTimingDataArray[i]["Cry"].AsInt;
                motionTimingData.Dust = motionTimingDataArray[i]["Dust"].AsInt;
                motionTimingData.Shot = motionTimingDataArray[i]["Shot"].AsInt;
                motionTimingData.Guard = motionTimingDataArray[i]["Guard"].AsInt;
                motionTimingData.LandingFall = motionTimingDataArray[i]["LandingFall"].AsInt;
                motionTimingData.LandingFallEase = motionTimingDataArray[i]["LandingFallEase"].AsInt;

                gameData.motionTimingData.Add(motionTimingData);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed Moves.
        /// </summary>
        private static async Task ParseMoves()
        {
            gameData.moves = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.PersonalMasterdatas]).Find(m => m["m_Name"].AsString == "WazaTable");
            AssetTypeValueField animationData = (await monoBehaviourCollection[PathEnum.BattleMasterdatas]).Find(m => m["m_Name"].AsString == "BattleDataTable");

            var moveFields = monoBehaviour["Waza.Array"].Children;
            var animationFields = animationData["BattleWazaData.Array"].Children;
            var textFields = await FindLabelArrayOfMessageFileAsync("ss_wazaname", Language.English);

            if (animationFields.Count < moveFields.Count)
                MainForm.ShowParserError("Oh my, this BattleDataTable is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "WazaTable entries: " + moveFields.Count + "\n" +
                    "BattleDataTable entries: " + animationFields.Count + "??");

            for (int moveID = 0; moveID < moveFields.Count; moveID++)
            {
                Move move = new();
                move.moveID = moveFields[moveID]["wazaNo"].AsInt;
                move.isValid = moveFields[moveID]["isValid"].AsByte;
                move.typingID = moveFields[moveID]["type"].AsByte;
                move.category = moveFields[moveID]["category"].AsByte;
                move.damageCategoryID = moveFields[moveID]["damageType"].AsByte;
                move.power = moveFields[moveID]["power"].AsByte;
                move.hitPer = moveFields[moveID]["hitPer"].AsByte;
                move.basePP = moveFields[moveID]["basePP"].AsByte;
                move.priority = moveFields[moveID]["priority"].AsSByte;
                move.hitCountMax = moveFields[moveID]["hitCountMax"].AsByte;
                move.hitCountMin = moveFields[moveID]["hitCountMin"].AsByte;
                move.sickID = moveFields[moveID]["sickID"].AsUShort;
                move.sickPer = moveFields[moveID]["sickPer"].AsByte;
                move.sickCont = moveFields[moveID]["sickCont"].AsByte;
                move.sickTurnMin = moveFields[moveID]["sickTurnMin"].AsByte;
                move.sickTurnMax = moveFields[moveID]["sickTurnMax"].AsByte;
                move.criticalRank = moveFields[moveID]["criticalRank"].AsByte;
                move.shrinkPer = moveFields[moveID]["shrinkPer"].AsByte;
                move.aiSeqNo = moveFields[moveID]["aiSeqNo"].AsUShort;
                move.damageRecoverRatio = moveFields[moveID]["damageRecoverRatio"].AsSByte;
                move.hpRecoverRatio = moveFields[moveID]["hpRecoverRatio"].AsSByte;
                move.target = moveFields[moveID]["target"].AsByte;
                move.rankEffType1 = moveFields[moveID]["rankEffType1"].AsByte;
                move.rankEffType2 = moveFields[moveID]["rankEffType2"].AsByte;
                move.rankEffType3 = moveFields[moveID]["rankEffType3"].AsByte;
                move.rankEffValue1 = moveFields[moveID]["rankEffValue1"].AsSByte;
                move.rankEffValue2 = moveFields[moveID]["rankEffValue2"].AsSByte;
                move.rankEffValue3 = moveFields[moveID]["rankEffValue3"].AsSByte;
                move.rankEffPer1 = moveFields[moveID]["rankEffPer1"].AsByte;
                move.rankEffPer2 = moveFields[moveID]["rankEffPer2"].AsByte;
                move.rankEffPer3 = moveFields[moveID]["rankEffPer3"].AsByte;
                move.flags = moveFields[moveID]["flags"].AsUInt;
                move.contestWazaNo = moveFields[moveID]["contestWazaNo"].AsUInt;

                move.cmdSeqName = animationFields[moveID]["CmdSeqName"].AsString;
                move.cmdSeqNameLegend = animationFields[moveID]["CmdSeqNameLegend"].AsString;
                move.notShortenTurnType0 = animationFields[moveID]["NotShortenTurnType0"].AsString;
                move.notShortenTurnType1 = animationFields[moveID]["NotShortenTurnType1"].AsString;
                move.turnType1 = animationFields[moveID]["TurnType1"].AsString;
                move.turnType2 = animationFields[moveID]["TurnType2"].AsString;
                move.turnType3 = animationFields[moveID]["TurnType3"].AsString;
                move.turnType4 = animationFields[moveID]["TurnType4"].AsString;

                move.name = "";
                if (textFields[moveID]["wordDataArray.Array"].Children.Count > 0)
                    move.name = textFields[moveID]["wordDataArray.Array"][0]["str"].AsString;

                gameData.moves.Add(move);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed ShopTables.
        /// </summary>
        private static async Task ParseShopTables()
        {
            gameData.shopTables = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => m["m_Name"].AsString == "ShopTable");

            gameData.shopTables.martItems = new();
            var martItemFields = monoBehaviour["FS.Array"].Children;
            for (int martItemIdx = 0; martItemIdx < martItemFields.Count; martItemIdx++)
            {
                MartItem martItem = new();
                martItem.itemID = martItemFields[martItemIdx]["ItemNo"].AsUShort;
                martItem.badgeNum = martItemFields[martItemIdx]["BadgeNum"].AsInt;
                martItem.zoneID = martItemFields[martItemIdx]["ZoneID"].AsInt;

                gameData.shopTables.martItems.Add(martItem);
            }

            gameData.shopTables.fixedShopItems = new();
            var fixedShopItemFields = monoBehaviour["FixedShop.Array"].Children;
            for (int fixedShopItemIdx = 0; fixedShopItemIdx < fixedShopItemFields.Count; fixedShopItemIdx++)
            {
                FixedShopItem fixedShopItem = new();
                fixedShopItem.itemID = fixedShopItemFields[fixedShopItemIdx]["ItemNo"].AsUShort;
                fixedShopItem.shopID = fixedShopItemFields[fixedShopItemIdx]["ShopID"].AsInt;

                gameData.shopTables.fixedShopItems.Add(fixedShopItem);
            }

            gameData.shopTables.bpShopItems = new();
            var bpShopItemFields = monoBehaviour["BPShop.Array"].Children;
            for (int bpShopItemIdx = 0; bpShopItemIdx < bpShopItemFields.Count; bpShopItemIdx++)
            {
                BpShopItem bpShopItem = new();
                bpShopItem.itemID = bpShopItemFields[bpShopItemIdx]["ItemNo"].AsUShort;

                if (!bpShopItemFields[bpShopItemIdx]["NPCID"].IsDummy)
                    bpShopItem.npcID = bpShopItemFields[bpShopItemIdx]["NPCID"].AsInt;
                else
                {
                    MainForm.ShowParserError("Oh my, this dump might be a bit outdated...\n" +
                        "Please input at least the v1.1.3 version of BDSP.\n" +
                        "I don't feel so good...");
                    throw new Exception("Outdated Dump");
                }

                gameData.shopTables.bpShopItems.Add(bpShopItem);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed PickupItems.
        /// </summary>
        private static async Task ParsePickupItems()
        {
            gameData.pickupItems = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.DprMasterdatas]).Find(m => m["m_Name"].AsString == "MonohiroiTable");

            var pickupItemFields = monoBehaviour["MonoHiroi.Array"].Children;
            for (int pickupItemIdx = 0; pickupItemIdx < pickupItemFields.Count; pickupItemIdx++)
            {
                PickupItem pickupItem = new();
                pickupItem.itemID = pickupItemFields[pickupItemIdx]["ID"].AsUShort;

                //Parse item probabilities
                pickupItem.ratios = new();
                for (int ratio = 0; ratio < pickupItemFields[pickupItemIdx]["Ratios.Array"].Children.Count; ratio++)
                    pickupItem.ratios.Add(pickupItemFields[pickupItemIdx]["Ratios.Array"][ratio].AsByte);

                gameData.pickupItems.Add(pickupItem);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed Items.
        /// </summary>
        private static async Task ParseItems()
        {
            gameData.items = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.PersonalMasterdatas]).Find(m => m["m_Name"].AsString == "ItemTable");

            var itemFields = monoBehaviour["Item.Array"].Children;
            var textFields = await FindLabelArrayOfMessageFileAsync("ss_itemname", Language.English);

            if (textFields.Length < itemFields.Count)
                MainForm.ShowParserError("Oh my, this " + FormatMessageFileNameForLanguage("ss_itemname", Language.English) + " is missing some stuff...\n" +
                    "I don't feel so good...\n" +
                    "ItemTable entries: " + itemFields.Count + "\n" +
                    FormatMessageFileNameForLanguage("ss_itemname", Language.English) + " entries: " + textFields.Length + "??");

            for (int itemIdx = 0; itemIdx < itemFields.Count; itemIdx++)
            {
                Item item = new();
                item.itemID = itemFields[itemIdx]["no"].AsShort;
                item.type = itemFields[itemIdx]["type"].AsByte;
                item.iconID = itemFields[itemIdx]["iconid"].AsInt;
                item.price = itemFields[itemIdx]["price"].AsInt;
                item.bpPrice = itemFields[itemIdx]["bp_price"].AsInt;
                item.nageAtc = itemFields[itemIdx]["nage_atc"].AsByte;
                item.sizenAtc = itemFields[itemIdx]["sizen_atc"].AsByte;
                item.sizenType = itemFields[itemIdx]["sizen_type"].AsByte;
                item.tuibamuEff = itemFields[itemIdx]["tuibamu_eff"].AsByte;
                item.sort = itemFields[itemIdx]["sort"].AsByte;
                item.group = itemFields[itemIdx]["group"].AsByte;
                item.groupID = itemFields[itemIdx]["group_id"].AsByte;
                item.fldPocket = itemFields[itemIdx]["fld_pocket"].AsByte;
                item.fieldFunc = itemFields[itemIdx]["field_func"].AsByte;
                item.battleFunc = itemFields[itemIdx]["battle_func"].AsByte;
                item.criticalRanks = itemFields[itemIdx]["wk_critical_up"].AsByte;
                item.atkStages = itemFields[itemIdx]["wk_atc_up"].AsByte;
                item.defStages = itemFields[itemIdx]["wk_def_up"].AsByte;
                item.spdStages = itemFields[itemIdx]["wk_agi_up"].AsByte;
                item.accStages = itemFields[itemIdx]["wk_hit_up"].AsByte;
                item.spAtkStages = itemFields[itemIdx]["wk_spa_up"].AsByte;
                item.spDefStages = itemFields[itemIdx]["wk_spd_up"].AsByte;
                item.ppRestoreAmount = itemFields[itemIdx]["wk_prm_pp_rcv"].AsByte;
                item.hpEvIncrease = itemFields[itemIdx]["wk_prm_hp_exp"].AsSByte;
                item.atkEvIncrease = itemFields[itemIdx]["wk_prm_pow_exp"].AsSByte;
                item.defEvIncrease = itemFields[itemIdx]["wk_prm_def_exp"].AsSByte;
                item.spdEvIncrease = itemFields[itemIdx]["wk_prm_agi_exp"].AsSByte;
                item.spAtkEvIncrease = itemFields[itemIdx]["wk_prm_spa_exp"].AsSByte;
                item.spDefEvIncrease = itemFields[itemIdx]["wk_prm_spd_exp"].AsSByte;
                item.friendshipIncrease1 = itemFields[itemIdx]["wk_friend1"].AsSByte;
                item.friendshipIncrease2 = itemFields[itemIdx]["wk_friend2"].AsSByte;
                item.friendshipIncrease3 = itemFields[itemIdx]["wk_friend3"].AsSByte;
                item.hpRestoreAmount = itemFields[itemIdx]["wk_prm_hp_rcv"].AsByte;
                item.flags0 = itemFields[itemIdx]["flags0"].AsUInt;

                item.name = "";
                if (textFields[itemIdx]["wordDataArray.Array"].Children.Count > 0)
                    item.name = textFields[itemIdx]["wordDataArray.Array"][0]["str"].AsString;

                gameData.items.Add(item);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed growth rates.
        /// </summary>
        private static async Task ParseGrowthRates()
        {
            gameData.growthRates = new();
            AssetTypeValueField monoBehaviour = (await monoBehaviourCollection[PathEnum.PersonalMasterdatas]).Find(m => m["m_Name"].AsString == "GrowTable");

            var growthRateFields = monoBehaviour["Data.Array"].Children;
            for (int growthRateIdx = 0; growthRateIdx < growthRateFields.Count; growthRateIdx++)
            {
                GrowthRate growthRate = new();
                growthRate.growthID = growthRateIdx;

                //Parse exp requirement
                growthRate.expRequirements = new();
                for (int level = 0; level < growthRateFields[growthRateIdx]["exps.Array"].Children.Count; level++)
                    growthRate.expRequirements.Add(growthRateFields[growthRateIdx]["exps.Array"][level].AsUInt);

                gameData.growthRates.Add(growthRate);
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed MessageFiles.
        /// </summary>
        public static async Task ParseAllMessageFiles()
        {
            gameData.messageFileSets = new MessageFileSet[10];
            for (int i = 0; i < gameData.messageFileSets.Length; i++)
            {
                gameData.messageFileSets[i] = new();
                gameData.messageFileSets[i].messageFiles = new();
            }
            gameData.messageFileSets[0].langID = Language.Japanese;
            gameData.messageFileSets[1].langID = Language.Japanese;
            gameData.messageFileSets[2].langID = Language.English;
            gameData.messageFileSets[3].langID = Language.French;
            gameData.messageFileSets[4].langID = Language.Italian;
            gameData.messageFileSets[5].langID = Language.German;
            gameData.messageFileSets[6].langID = Language.Spanish;
            gameData.messageFileSets[7].langID = Language.Korean;
            gameData.messageFileSets[8].langID = Language.SimpChinese;
            gameData.messageFileSets[9].langID = Language.TradChinese;

            List<AssetTypeValueField> monoBehaviours = await monoBehaviourCollection[PathEnum.CommonMsbt];
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.English]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.French]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.German]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.Italian]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.Jpn]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.JpnKanji]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.Korean]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.SimpChinese]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.Spanish]);
            monoBehaviours.AddRange(await monoBehaviourCollection[PathEnum.TradChinese]);

            for (int mIdx = 0; mIdx < monoBehaviours.Count; mIdx++)
            {
                MessageFile messageFile = new();
                messageFile.mName = monoBehaviours[mIdx]["m_Name"].AsString;
                messageFile.langID = (Language)monoBehaviours[mIdx]["langID"].AsInt;
                messageFile.isKanji = monoBehaviours[mIdx]["isKanji"].AsByte;

                //Parse LabelData
                messageFile.labelDatas = new();
                var labelDataFields = monoBehaviours[mIdx]["labelDataArray.Array"].Children;
                for (int labelDataIdx = 0; labelDataIdx < labelDataFields.Count; labelDataIdx++)
                {
                    LabelData labelData = new();
                    labelData.labelIndex = labelDataFields[labelDataIdx]["labelIndex"].AsInt;
                    labelData.arrayIndex = labelDataFields[labelDataIdx]["arrayIndex"].AsInt;
                    labelData.labelName = labelDataFields[labelDataIdx]["labelName"].AsString;
                    labelData.styleIndex = labelDataFields[labelDataIdx]["styleInfo.styleIndex"].AsInt;
                    labelData.colorIndex = labelDataFields[labelDataIdx]["styleInfo.colorIndex"].AsInt;
                    labelData.fontSize = labelDataFields[labelDataIdx]["styleInfo.fontSize"].AsInt;
                    labelData.maxWidth = labelDataFields[labelDataIdx]["styleInfo.maxWidth"].AsInt;
                    labelData.controlID = labelDataFields[labelDataIdx]["styleInfo.controlID"].AsInt;

                    // Parse Attribute Array
                    var attrArray = labelDataFields[labelDataIdx]["attributeValueArray.Array"].Children;
                    labelData.attributeValues = new();
                    for (int attrIdx = 0; attrIdx < attrArray.Count; attrIdx++)
                    {
                        labelData.attributeValues.Add(attrArray[attrIdx].AsInt);
                    }

                    // Parse TagData
                    var tagDataFields = labelDataFields[labelDataIdx]["tagDataArray.Array"].Children;
                    labelData.tagDatas = new();
                    for (int tagDataIdx = 0; tagDataIdx < tagDataFields.Count; tagDataIdx++)
                    {
                        TagData tagData = new();
                        tagData.tagIndex = tagDataFields[tagDataIdx]["tagIndex"].AsInt;
                        tagData.groupID = tagDataFields[tagDataIdx]["groupID"].AsInt;
                        tagData.tagID = tagDataFields[tagDataIdx]["tagID"].AsInt;
                        tagData.tagPatternID = tagDataFields[tagDataIdx]["tagPatternID"].AsInt;
                        tagData.forceArticle = tagDataFields[tagDataIdx]["forceArticle"].AsInt;
                        tagData.tagParameter = tagDataFields[tagDataIdx]["tagParameter"].AsInt;
                        tagData.tagWordArray = new();
                        foreach (AssetTypeValueField tagWordField in tagDataFields[tagDataIdx]["tagWordArray.Array"].Children)
                        {
                            tagData.tagWordArray.Add(tagWordField.AsString);
                        }

                        tagData.forceGrmID = tagDataFields[tagDataIdx]["forceGrmID"].AsInt;

                        labelData.tagDatas.Add(tagData);
                    }

                    //Parse WordData
                    labelData.wordDatas = new();
                    var wordDataFields = labelDataFields[labelDataIdx]["wordDataArray.Array"].Children;
                    for (int wordDataIdx = 0; wordDataIdx < wordDataFields.Count; wordDataIdx++)
                    {
                        WordData wordData = new();
                        wordData.patternID = wordDataFields[wordDataIdx]["patternID"].AsInt;
                        wordData.eventID = wordDataFields[wordDataIdx]["eventID"].AsInt;
                        wordData.tagIndex = wordDataFields[wordDataIdx]["tagIndex"].AsInt;
                        wordData.tagValue = wordDataFields[wordDataIdx]["tagValue"].AsFloat;
                        wordData.str = wordDataFields[wordDataIdx]["str"].AsString;
                        wordData.strWidth = wordDataFields[wordDataIdx]["strWidth"].AsFloat;

                        labelData.wordDatas.Add(wordData);
                    }

                    messageFile.labelDatas.Add(labelData);
                }

                switch (messageFile.langID)
                {
                    case Language.Japanese:
                        if (messageFile.isKanji == 0)
                            gameData.messageFileSets[0].messageFiles.Add(messageFile);
                        else
                            gameData.messageFileSets[1].messageFiles.Add(messageFile);
                        break;
                    case Language.English:
                        gameData.messageFileSets[2].messageFiles.Add(messageFile);
                        break;
                    case Language.French:
                        gameData.messageFileSets[3].messageFiles.Add(messageFile);
                        break;
                    case Language.Italian:
                        gameData.messageFileSets[4].messageFiles.Add(messageFile);
                        break;
                    case Language.German:
                        gameData.messageFileSets[5].messageFiles.Add(messageFile);
                        break;
                    case Language.Spanish:
                        gameData.messageFileSets[6].messageFiles.Add(messageFile);
                        break;
                    case Language.Korean:
                        gameData.messageFileSets[7].messageFiles.Add(messageFile);
                        break;
                    case Language.SimpChinese:
                        gameData.messageFileSets[8].messageFiles.Add(messageFile);
                        break;
                    case Language.TradChinese:
                        gameData.messageFileSets[9].messageFiles.Add(messageFile);
                        break;
                }
            }
        }

        /// <summary>
        ///  Overwrites GlobalData with parsed EvScripts.
        /// </summary>
        private static async Task ParseEvScripts()
        {
            gameData.evScripts = new();
            List<AssetTypeValueField> monoBehaviours = (await monoBehaviourCollection[PathEnum.EvScript]).Where(m => !m["Scripts"].IsDummy && !m["StrList"].IsDummy).ToList();

            for (int mIdx = 0; mIdx < monoBehaviours.Count; mIdx++)
            {
                EvScript evScript = new();
                evScript.mName = monoBehaviours[mIdx]["m_Name"].AsString;

                //Parse Scripts
                evScript.scripts = new();
                var scriptFields = monoBehaviours[mIdx]["Scripts.Array"].Children;
                for (int scriptIdx = 0; scriptIdx < scriptFields.Count; scriptIdx++)
                {
                    Script script = new();
                    script.evLabel = scriptFields[scriptIdx]["Label"].AsString;

                    //Parse Commands
                    script.commands = new();
                    var commandFields = scriptFields[scriptIdx]["Commands.Array"].Children;
                    for (int commandIdx = 0; commandIdx < commandFields.Count; commandIdx++)
                    {
                        Command command = new();

                        //Check for commands without data, because those exist for some reason.
                        if (commandFields[commandIdx]["Arg.Array"].Children.Count == 0)
                        {
                            command.cmdType = -1;
                            script.commands.Add(command);
                            continue;
                        }
                        command.cmdType = commandFields[commandIdx]["Arg.Array"][0]["data"].AsInt;

                        //Parse Arguments
                        command.args = new();
                        var argumentFields = commandFields[commandIdx]["Arg.Array"].Children;
                        for (int argIdx = 1; argIdx < argumentFields.Count; argIdx++)
                        {
                            Argument arg = new();
                            arg.argType = argumentFields[argIdx]["argType"].AsInt;
                            arg.data = argumentFields[argIdx]["data"].AsInt;
                            if (arg.argType == 1)
                                arg.data = ConvertToFloat((int)arg.data);

                            command.args.Add(arg);
                        }

                        script.commands.Add(command);
                    }

                    evScript.scripts.Add(script);
                }

                //Parse StrLists
                evScript.strList = new();
                var stringFields = monoBehaviours[mIdx]["StrList.Array"].Children;
                for (int stringIdx = 0; stringIdx < stringFields.Count; stringIdx++)
                    evScript.strList.Add(stringFields[stringIdx].AsString);

                gameData.evScripts.Add(evScript);
            }
        }

        /// <summary>
        ///  Interprets bytes of an int32 as a float.
        /// </summary>
        private static float ConvertToFloat(int n)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(n));
        }

        /// <summary>
        ///  Overwrites GlobalData with a parsed WwiseData.
        /// </summary>
        private static void ParseAudioData()
        {
            gameData.audioData = new();
            gameData.audioData.Parse(fileManager.GetDelphisMainBuffer());
        }

        /// <summary>
        ///  Overwrites GlobalData with a parsed GlobalMetadata.
        /// </summary>
        private static void ParseGlobalMetadata()
        {
            gameData.globalMetadata = new();
            byte[] buffer = fileManager.GetGlobalMetadataBuffer();
            gameData.globalMetadata.buffer = buffer;

            gameData.globalMetadata.stringOffset = BitConverter.ToUInt32(buffer, 0x18);

            gameData.globalMetadata.defaultValuePtrOffset = BitConverter.ToUInt32(buffer, 0x40);
            gameData.globalMetadata.defaultValuePtrSecSize = BitConverter.ToUInt32(buffer, 0x44);
            uint defaultValuePtrSize = 0xC;
            uint defaultValuePtrCount = gameData.globalMetadata.defaultValuePtrSecSize / defaultValuePtrSize;

            gameData.globalMetadata.defaultValueOffset = BitConverter.ToUInt32(buffer, 0x48);
            gameData.globalMetadata.defaultValueSecSize = BitConverter.ToUInt32(buffer, 0x4C);

            gameData.globalMetadata.fieldOffset = BitConverter.ToUInt32(buffer, 0x60);
            uint fieldSize = 0xC;

            gameData.globalMetadata.typeOffset = BitConverter.ToUInt32(buffer, 0xA0);
            uint typeSize = 0x5C;

            gameData.globalMetadata.imageOffset = BitConverter.ToUInt32(buffer, 0xA8);
            gameData.globalMetadata.imageSecSize = BitConverter.ToUInt32(buffer, 0xAC);
            uint imageSize = 0x28;
            uint imageCount = gameData.globalMetadata.imageSecSize / imageSize;

            gameData.globalMetadata.defaultValueDic = new();
            uint defaultValuePtrOffset = gameData.globalMetadata.defaultValuePtrOffset;
            for (int defaultValuePtrIdx = 0; defaultValuePtrIdx < defaultValuePtrCount; defaultValuePtrIdx++)
            {
                FieldDefaultValue fdv = new();
                fdv.offset = gameData.globalMetadata.defaultValueOffset + BitConverter.ToUInt32(buffer, (int)defaultValuePtrOffset + 8);
                long nextOffset = gameData.globalMetadata.defaultValueOffset + gameData.globalMetadata.defaultValueSecSize;
                if (defaultValuePtrIdx < defaultValuePtrCount - 1)
                    nextOffset = gameData.globalMetadata.defaultValueOffset + BitConverter.ToUInt32(buffer, (int)defaultValuePtrOffset + 20);
                fdv.length = (int)(nextOffset - fdv.offset);
                uint fieldIdx = BitConverter.ToUInt32(buffer, (int)defaultValuePtrOffset + 0);

                gameData.globalMetadata.defaultValueDic[fieldIdx] = fdv;
                defaultValuePtrOffset += defaultValuePtrSize;
            }

            gameData.globalMetadata.images = new();
            uint imageOffset = gameData.globalMetadata.imageOffset;
            for (int imageIdx = 0; imageIdx < imageCount; imageIdx++)
            {
                ImageDefinition id = new();
                uint imageNameIdx = BitConverter.ToUInt32(buffer, (int)imageOffset + 0);
                id.name = ReadNullTerminatedString(buffer, gameData.globalMetadata.stringOffset + imageNameIdx);
                id.typeStart = BitConverter.ToUInt32(buffer, (int)imageOffset + 8);
                id.typeCount = BitConverter.ToUInt32(buffer, (int)imageOffset + 12);

                id.types = new();
                uint typeOffset = gameData.globalMetadata.typeOffset + id.typeStart * typeSize;
                for (uint typeIdx = id.typeStart; typeIdx < id.typeStart + id.typeCount; typeIdx++)
                {
                    TypeDefinition td = new();
                    uint typeNameIdx = BitConverter.ToUInt32(buffer, (int)typeOffset + 0);
                    uint namespaceNameIdx = BitConverter.ToUInt32(buffer, (int)typeOffset + 4);
                    td.name = ReadNullTerminatedString(buffer, gameData.globalMetadata.stringOffset + namespaceNameIdx);
                    td.name += td.name.Length > 0 ? "." : "";
                    td.name += ReadNullTerminatedString(buffer, gameData.globalMetadata.stringOffset + typeNameIdx);
                    td.fieldStart = BitConverter.ToInt32(buffer, (int)typeOffset + 36);
                    td.fieldCount = BitConverter.ToUInt16(buffer, (int)typeOffset + 72);

                    td.fields = new();
                    uint fieldOffset = (uint)(gameData.globalMetadata.fieldOffset + td.fieldStart * fieldSize);
                    for (uint fieldIdx = (uint)td.fieldStart; fieldIdx < td.fieldStart + td.fieldCount; fieldIdx++)
                    {
                        FieldDefinition fd = new();
                        uint fieldNameIdx = BitConverter.ToUInt32(buffer, (int)fieldOffset + 0);
                        fd.name = ReadNullTerminatedString(buffer, gameData.globalMetadata.stringOffset + fieldNameIdx);
                        if (gameData.globalMetadata.defaultValueDic.TryGetValue(fieldIdx, out FieldDefaultValue fdv))
                            fd.defautValue = fdv;

                        td.fields.Add(fd);
                        fieldOffset += fieldSize;
                    }

                    id.types.Add(td);
                    typeOffset += typeSize;
                }

                gameData.globalMetadata.images.Add(id);
                imageOffset += imageSize;
            }

            string[] typeArrayNames = new string[]
            {
"A3758C06C7FB42A47D220A11FBA532C6E8C62A77",
"4B289ECFF3C0F0970CFBB23E3106E05803CB0010",
"B9D3FD531E1A63CC167C4B98C0EC93F0249D9944",
"347E5A9763B5C5AD3094AEC4B91A98983001E87D",
"C089A0863406C198B5654996536BAC473C816234",
"BCEEC8610D8506C3EDAC1C28CED532E5E2D8AD32",
"A6F987666C679A4472D8CD64F600B501D2241486",
"ACBC28AD33161A13959E63783CBFC94EB7FB2D90",
"0459498E9764395D87F7F43BE89CCE657C669BFC",
"C4215116A59F8DBC29910FA47BFBC6A82702816F",
"AEDBD0B97A96E5BDD926058406DB246904438044",
"DF2387E4B816070AE396698F2BD7359657EADE81",
"64FFED43123BBC9517F387412947F1C700527EB4",
"B5D988D1CB442CF60C021541BF2DC2A008819FD4",
"D64329EA3A838F1B4186746A734070A5DFDA4983",
"37DF3221C4030AC4E0EB9DD64616D020BB628CC1",
"B2DD1970DDE852F750899708154090300541F4DE",
"F774719D6A36449B152496136177E900605C9778"
            };

            TypeDefinition privateImplementationDetails = gameData.globalMetadata.images
            .Where(i => i.name == "Assembly-CSharp.dll").SelectMany(i => i.types)
            .First(t => t.name == "<PrivateImplementationDetails>");

            gameData.globalMetadata.typeMatchupOffsets = typeArrayNames
            .Select(s => privateImplementationDetails.fields.First(f => f.name == s).defautValue.offset).ToArray();
        }

        /// <summary>
        ///  Returns the null terminated UTF8 string starting at the specified offset.
        /// </summary>
        private static string ReadNullTerminatedString(byte[] buffer, long offset)
        {
            long endOffset = offset;
            while (buffer[endOffset] != 0)
                endOffset++;
            return Encoding.UTF8.GetString(buffer, (int)offset, (int)(endOffset - offset));
        }

        /// <summary>
        ///  Overwrites GlobalData with a parsed AssetBundleDownloadManifest.
        /// </summary>
        private static void ParseDprBin()
        {
            AssetBundleDownloadManifest abdm = fileManager.GetDprBin();
            if (abdm != null)
                gameData.dprBin = abdm;
        }

        /// <summary>
        ///  Commits all modified files and prepares them for exporting.
        /// </summary>
        public static void CommitChanges()
        {
            if (gameData.IsModified(GameDataSet.DataField.EvScripts))
                CommitEvScripts();
            if (gameData.IsModified(GameDataSet.DataField.PickupItems))
                CommitPickupItems();
            if (gameData.IsModified(GameDataSet.DataField.ShopTables))
                CommitShopTables();
            if (gameData.IsModified(GameDataSet.DataField.Trainers))
                CommitTrainers();
            CommitBattleTowerPokemon();
            CommitBattleTowerTrainers();
            if (gameData.IsModified(GameDataSet.DataField.battleTowerTrainerPokemons))
                CommitBattleTowerPokemon();
            if (gameData.IsModified(GameDataSet.DataField.EncounterTableFiles))
                CommitEncounterTables();
            if (gameData.IsModified(GameDataSet.DataField.MessageFileSets))
                CommitMessageFileSets();
            if (gameData.IsModified(GameDataSet.DataField.UgAreas) ||
            gameData.IsModified(GameDataSet.DataField.UgEncounterFiles) ||
            gameData.IsModified(GameDataSet.DataField.UgEncounterLevelSets) ||
            gameData.IsModified(GameDataSet.DataField.UgSpecialEncounters) ||
            gameData.IsModified(GameDataSet.DataField.UgPokemonData))
                CommitUgTables();
            if (gameData.IsModified(GameDataSet.DataField.PersonalEntries))
                CommitPokemon();
            if (gameData.IsModified(GameDataSet.DataField.Items))
                CommitItems();
            if (gameData.IsModified(GameDataSet.DataField.TMs))
                CommitTMs();
            if (gameData.IsModified(GameDataSet.DataField.Moves))
                CommitMoves();
            if (gameData.IsModified(GameDataSet.DataField.AudioData))
                CommitAudio();
            if (gameData.IsModified(GameDataSet.DataField.GlobalMetadata))
                CommitGlobalMetadata();
            if (gameData.IsModified(GameDataSet.DataField.UIMasterdatas))
                CommitUIMasterdatas();
            if (gameData.IsModified(GameDataSet.DataField.AddPersonalTable))
                CommitAddPersonalTable();
            if (gameData.IsModified(GameDataSet.DataField.MotionTimingData))
                CommitMotionTimingData();
            if (gameData.IsModified(GameDataSet.DataField.PokemonInfo))
                CommitPokemonInfo();
            if (gameData.IsModified(GameDataSet.DataField.ContestResultMotion))
                CommitContestMasterDatas();
            if (gameData.IsModified(GameDataSet.DataField.DprBin))
                CommitDprBin();
            if (gameData.IsModified(GameDataSet.DataField.ExternalStarters))
                CommitExternalStarters();
            if (gameData.IsModified(GameDataSet.DataField.ExternalHoneyTrees))
                CommitExternalHoneyTrees();
        }

        private static void CommitExternalHoneyTrees()
        {
            foreach ((string name, Starter _) in gameData.externalStarters)
                fileManager.CommitExternalJson($"Encounters\\Starter\\{name}.json");
        }

        private static void CommitExternalStarters()
        {
            foreach ((string name, HoneyTreeZone _) in gameData.externalHoneyTrees)
                fileManager.CommitExternalJson($"Encounters\\HoneyTrees\\{name}.json");
        }

        private static void CommitContestMasterDatas()
        {
            gameData.contestResultMotion.Sort();
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.ContestMasterdatas).Find(m => m["m_Name"].AsString == "ContestConfigDatas");

            List<AssetTypeValueField> newResultMotionSheet = new();
            for (int i = 0; i < gameData.contestResultMotion.Count; i++)
            {
                ResultMotion rm = gameData.contestResultMotion[i];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["ResultMotion.Array"]);

                baseField["valid_flag"].AsBool = rm.validFlag != 0;
                baseField["id"].AsUShort = rm.id;
                baseField["monsNo"].AsInt = rm.monsNo;
                baseField["winAnim"].AsUInt = rm.winAnim;
                baseField["loseAnim"].AsUInt = rm.loseAnim;
                baseField["waitAnim"].AsUInt = rm.waitAnim;
                baseField["duration"].AsFloat = rm.duration;

                newResultMotionSheet.Add(baseField);
            }

            monoBehaviour["ResultMotion.Array"].Children = newResultMotionSheet;

            fileManager.WriteMonoBehaviour(PathEnum.ContestMasterdatas, monoBehaviour);
        }

        private static void CommitDprBin()
        {
            fileManager.CommitDprBin();
        }

        private static void CommitGlobalMetadata()
        {
            fileManager.CommitGlobalMetadata();
        }

        private static void CommitAudio()
        {
            gameData.audioSourceLog ??= fileManager.GetAudioSourceLog();
            gameData.audioSourceLog.Clear();
            Dictionary<uint, WwiseObject> lookup = gameData.audioData.objectsByID;

            foreach (Pokemon p in gameData.personalEntries)
            {
                IEnumerable<string> eventNames = PokemonInserter.GetWwiseEvents(p.dexID, p.formID).Take(5);
                foreach (string eventName in eventNames)
                {
                    uint eventID = FNV132(eventName);
                    if (!lookup.TryGetValue(eventID, out WwiseObject ewo)) continue;
                    Event e = (Event)ewo;
                    if (!lookup.TryGetValue(e.actionIDs.First(), out WwiseObject apwo)) continue;
                    ActionPlay ap = (ActionPlay)apwo;
                    if (!lookup.TryGetValue(ap.idExt, out WwiseObject swo)) continue;
                    if (swo is not Sound) continue;
                    Sound s = (Sound)swo;
                    gameData.audioSourceLog.Append(eventName + " → " + s.bankSourceData.mediaInformation.sourceID + "\n");
                }
            }

            fileManager.CommitAudio();
        }

        private static void CommitMoves()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.PersonalMasterdatas).Find(m => m["m_Name"].AsString == "WazaTable");
            AssetTypeValueField animationData = fileManager.GetMonoBehaviours(PathEnum.BattleMasterdatas).Find(m => m["m_Name"].AsString == "BattleDataTable");

            List<AssetTypeValueField> newMoveFields = new();
            List<AssetTypeValueField> newAnimationFields = new();
            for (int moveID = 0; moveID < gameData.moves.Count; moveID++)
            {
                Move move = gameData.moves[moveID];
                AssetTypeValueField baseMoveField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["Waza.Array"]);
                AssetTypeValueField baseAnimationField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["BattleWazaData.Array"]);

                baseMoveField["wazaNo"].AsInt = move.moveID;
                baseMoveField["isValid"].AsByte = move.isValid;
                baseMoveField["type"].AsByte = move.typingID;
                baseMoveField["category"].AsByte = move.category;
                baseMoveField["damageType"].AsByte = move.damageCategoryID;
                baseMoveField["power"].AsByte = move.power;
                baseMoveField["hitPer"].AsByte = move.hitPer;
                baseMoveField["basePP"].AsByte = move.basePP;
                baseMoveField["priority"].AsSByte = move.priority;
                baseMoveField["hitCountMax"].AsByte = move.hitCountMax;
                baseMoveField["hitCountMin"].AsByte = move.hitCountMin;
                baseMoveField["sickID"].AsUShort = move.sickID;
                baseMoveField["sickPer"].AsByte = move.sickPer;
                baseMoveField["sickCont"].AsByte = move.sickCont;
                baseMoveField["sickTurnMin"].AsByte = move.sickTurnMin;
                baseMoveField["sickTurnMax"].AsByte = move.sickTurnMax;
                baseMoveField["criticalRank"].AsByte = move.criticalRank;
                baseMoveField["shrinkPer"].AsByte = move.shrinkPer;
                baseMoveField["aiSeqNo"].AsUShort = move.aiSeqNo;
                baseMoveField["damageRecoverRatio"].AsSByte = move.damageRecoverRatio;
                baseMoveField["hpRecoverRatio"].AsSByte = move.hpRecoverRatio;
                baseMoveField["target"].AsByte = move.target;
                baseMoveField["rankEffType1"].AsByte = move.rankEffType1;
                baseMoveField["rankEffType2"].AsByte = move.rankEffType2;
                baseMoveField["rankEffType3"].AsByte = move.rankEffType3;
                baseMoveField["rankEffValue1"].AsSByte = move.rankEffValue1;
                baseMoveField["rankEffValue2"].AsSByte = move.rankEffValue2;
                baseMoveField["rankEffValue3"].AsSByte = move.rankEffValue3;
                baseMoveField["rankEffPer1"].AsByte = move.rankEffPer1;
                baseMoveField["rankEffPer2"].AsByte = move.rankEffPer2;
                baseMoveField["rankEffPer3"].AsByte = move.rankEffPer3;
                baseMoveField["flags"].AsUInt = move.flags;
                baseMoveField["contestWazaNo"].AsUInt = move.contestWazaNo;

                baseAnimationField["CmdSeqName"].AsString = move.cmdSeqName;
                baseAnimationField["CmdSeqNameLegend"].AsString = move.cmdSeqNameLegend;
                baseAnimationField["NotShortenTurnType0"].AsString = move.notShortenTurnType0;
                baseAnimationField["NotShortenTurnType1"].AsString = move.notShortenTurnType1;
                baseAnimationField["TurnType1"].AsString = move.turnType1;
                baseAnimationField["TurnType2"].AsString = move.turnType2;
                baseAnimationField["TurnType3"].AsString = move.turnType3;
                baseAnimationField["TurnType4"].AsString = move.turnType4;

                newMoveFields.Add(baseMoveField);
                newAnimationFields.Add(baseAnimationField);
            }

            monoBehaviour["Waza.Array"].Children = newMoveFields;
            monoBehaviour["BattleWazaData.Array"].Children = newAnimationFields;

            fileManager.WriteMonoBehaviour(PathEnum.PersonalMasterdatas, monoBehaviour);
            fileManager.WriteMonoBehaviour(PathEnum.BattleMasterdatas, animationData);
        }

        /// <summary>
        ///  Updates loaded bundle with TMs.
        /// </summary>
        private static void CommitTMs()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.PersonalMasterdatas).Find(m => m["m_Name"].AsString == "ItemTable");

            List<AssetTypeValueField> newTmFields = new();
            for (int tmID = 0; tmID < gameData.tms.Count; tmID++)
            {
                TM tm = gameData.tms[tmID];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["WazaMachine.Array"]);

                baseField["itemNo"].AsInt = tm.itemID;
                baseField["machineNo"].AsInt = tm.machineNo;
                baseField["wazaNo"].AsInt = tm.moveID;

                newTmFields.Add(baseField);
            }

            monoBehaviour["WazaMachine.Array"].Children = newTmFields;

            fileManager.WriteMonoBehaviour(PathEnum.PersonalMasterdatas, monoBehaviour);
        }

        /// <summary>
        ///  Updates loaded bundle with Items.
        /// </summary>
        private static void CommitItems()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.PersonalMasterdatas).Find(m => m["m_Name"].AsString == "ItemTable");

            List<AssetTypeValueField> newItemFields = new();
            for (int itemIdx = 0; itemIdx < gameData.items.Count; itemIdx++)
            {
                Item item = gameData.items[itemIdx];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["Item.Array"]);

                baseField["no"].AsShort = item.itemID;
                baseField["type"].AsByte = item.type;
                baseField["iconid"].AsInt = item.iconID;
                baseField["price"].AsInt = item.price;
                baseField["bp_price"].AsInt = item.bpPrice;
                baseField["nage_atc"].AsByte = item.nageAtc;
                baseField["sizen_atc"].AsByte = item.sizenAtc;
                baseField["sizen_type"].AsByte = item.sizenType;
                baseField["tuibamu_eff"].AsByte = item.tuibamuEff;
                baseField["sort"].AsByte = item.sort;
                baseField["group"].AsByte = item.group;
                baseField["group_id"].AsByte = item.groupID;
                baseField["fld_pocket"].AsByte = item.fldPocket;
                baseField["field_func"].AsByte = item.fieldFunc;
                baseField["battle_func"].AsByte = item.battleFunc;
                baseField["wk_critical_up"].AsByte = item.criticalRanks;
                baseField["wk_atc_up"].AsByte = item.atkStages;
                baseField["wk_def_up"].AsByte = item.defStages;
                baseField["wk_agi_up"].AsByte = item.spdStages;
                baseField["wk_hit_up"].AsByte = item.accStages;
                baseField["wk_spa_up"].AsByte = item.spAtkStages;
                baseField["wk_spd_up"].AsByte = item.spDefStages;
                baseField["wk_prm_pp_rcv"].AsByte = item.ppRestoreAmount;
                baseField["wk_prm_hp_exp"].AsSByte = item.hpEvIncrease;
                baseField["wk_prm_pow_exp"].AsSByte = item.atkEvIncrease;
                baseField["wk_prm_def_exp"].AsSByte = item.defEvIncrease;
                baseField["wk_prm_agi_exp"].AsSByte = item.spdEvIncrease;
                baseField["wk_prm_spa_exp"].AsSByte = item.spAtkEvIncrease;
                baseField["wk_prm_spd_exp"].AsSByte = item.spDefEvIncrease;
                baseField["wk_friend1"].AsSByte = item.friendshipIncrease1;
                baseField["wk_friend2"].AsSByte = item.friendshipIncrease2;
                baseField["wk_friend3"].AsSByte = item.friendshipIncrease3;
                baseField["wk_prm_hp_rcv"].AsByte = item.hpRestoreAmount;
                baseField["flags0"].AsUInt = item.flags0;

                newItemFields.Add(baseField);
            }

            monoBehaviour["Item.Array"].Children = newItemFields;

            fileManager.WriteMonoBehaviour(PathEnum.PersonalMasterdatas, monoBehaviour);
        }
        private static void CommitPokemonInfo()
        {
            gameData.pokemonInfos.Sort();

            AssetTypeValueField pokemonInfo = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas).Find(m => m["m_Name"].AsString == "PokemonInfo");

            List<AssetTypeValueField> newCatalogs = new();
            List<AssetTypeValueField> newTrearukis = new();
            foreach (Masterdatas.PokemonInfoCatalog pokemonInfoCatalog in gameData.pokemonInfos)
            {
                AssetTypeValueField catalogBaseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(pokemonInfo["Catalog.Array"]);
                AssetTypeValueField trearukiBaseField = null;
                if (!pokemonInfo["Trearuki"].IsDummy)
                    trearukiBaseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(pokemonInfo["Trearuki.Array"]);

                catalogBaseField["UniqueID"].AsInt = pokemonInfoCatalog.UniqueID;
                catalogBaseField["No"].AsInt = pokemonInfoCatalog.No;
                catalogBaseField["SinnohNo"].AsInt = pokemonInfoCatalog.SinnohNo;
                catalogBaseField["MonsNo"].AsInt = pokemonInfoCatalog.MonsNo;
                catalogBaseField["FormNo"].AsInt = pokemonInfoCatalog.FormNo;
                catalogBaseField["Sex"].AsByte = pokemonInfoCatalog.Sex;
                catalogBaseField["Rare"].AsBool = pokemonInfoCatalog.Rare;
                catalogBaseField["AssetBundleName"].AsString = pokemonInfoCatalog.AssetBundleName;
                catalogBaseField["BattleScale"].AsFloat = pokemonInfoCatalog.BattleScale;
                catalogBaseField["ContestScale"].AsFloat = pokemonInfoCatalog.ContestScale;
                catalogBaseField["ContestSize"].AsInt = (int)pokemonInfoCatalog.ContestSize;
                catalogBaseField["FieldScale"].AsFloat = pokemonInfoCatalog.FieldScale;
                catalogBaseField["FieldChikaScale"].AsFloat = pokemonInfoCatalog.FieldChikaScale;
                catalogBaseField["StatueScale"].AsFloat = pokemonInfoCatalog.StatueScale;
                catalogBaseField["FieldWalkingScale"].AsFloat = pokemonInfoCatalog.FieldWalkingScale;
                catalogBaseField["FieldFureaiScale"].AsFloat = pokemonInfoCatalog.FieldFureaiScale;
                catalogBaseField["MenuScale"].AsFloat = pokemonInfoCatalog.MenuScale;
                catalogBaseField["ModelMotion"].AsString = pokemonInfoCatalog.ModelMotion;
                catalogBaseField["ModelOffset.x"].AsFloat = pokemonInfoCatalog.ModelOffset.X;
                catalogBaseField["ModelOffset.y"].AsFloat = pokemonInfoCatalog.ModelOffset.Y;
                catalogBaseField["ModelOffset.z"].AsFloat = pokemonInfoCatalog.ModelOffset.Z;
                catalogBaseField["ModelRotationAngle.x"].AsFloat = pokemonInfoCatalog.ModelRotationAngle.X;
                catalogBaseField["ModelRotationAngle.y"].AsFloat = pokemonInfoCatalog.ModelRotationAngle.Y;
                catalogBaseField["ModelRotationAngle.z"].AsFloat = pokemonInfoCatalog.ModelRotationAngle.Z;
                catalogBaseField["DistributionScale"].AsFloat = pokemonInfoCatalog.DistributionScale;
                catalogBaseField["DistributionModelMotion"].AsString = pokemonInfoCatalog.DistributionModelMotion;
                catalogBaseField["DistributionModelOffset.x"].AsFloat = pokemonInfoCatalog.DistributionModelOffset.X;
                catalogBaseField["DistributionModelOffset.y"].AsFloat = pokemonInfoCatalog.DistributionModelOffset.Y;
                catalogBaseField["DistributionModelOffset.z"].AsFloat = pokemonInfoCatalog.DistributionModelOffset.Z;
                catalogBaseField["DistributionModelRotationAngle.x"].AsFloat = pokemonInfoCatalog.DistributionModelRotationAngle.X;
                catalogBaseField["DistributionModelRotationAngle.y"].AsFloat = pokemonInfoCatalog.DistributionModelRotationAngle.Y;
                catalogBaseField["DistributionModelRotationAngle.z"].AsFloat = pokemonInfoCatalog.DistributionModelRotationAngle.Z;
                catalogBaseField["VoiceScale"].AsFloat = pokemonInfoCatalog.VoiceScale;
                catalogBaseField["VoiceModelMotion"].AsString = pokemonInfoCatalog.VoiceModelMotion;
                catalogBaseField["VoiceModelOffset.x"].AsFloat = pokemonInfoCatalog.VoiceModelOffset.X;
                catalogBaseField["VoiceModelOffset.y"].AsFloat = pokemonInfoCatalog.VoiceModelOffset.Y;
                catalogBaseField["VoiceModelOffset.z"].AsFloat = pokemonInfoCatalog.VoiceModelOffset.Z;
                catalogBaseField["VoiceModelRotationAngle.x"].AsFloat = pokemonInfoCatalog.VoiceModelRotationAngle.X;
                catalogBaseField["VoiceModelRotationAngle.y"].AsFloat = pokemonInfoCatalog.VoiceModelRotationAngle.Y;
                catalogBaseField["VoiceModelRotationAngle.z"].AsFloat = pokemonInfoCatalog.VoiceModelRotationAngle.Z;
                catalogBaseField["CenterPointOffset.x"].AsFloat = pokemonInfoCatalog.CenterPointOffset.X;
                catalogBaseField["CenterPointOffset.y"].AsFloat = pokemonInfoCatalog.CenterPointOffset.Y;
                catalogBaseField["CenterPointOffset.z"].AsFloat = pokemonInfoCatalog.CenterPointOffset.Z;
                catalogBaseField["RotationLimitAngle.x"].AsFloat = pokemonInfoCatalog.RotationLimitAngle.X;
                catalogBaseField["RotationLimitAngle.y"].AsFloat = pokemonInfoCatalog.RotationLimitAngle.Y;
                catalogBaseField["StatusScale"].AsFloat = pokemonInfoCatalog.StatusScale;
                catalogBaseField["StatusModelMotion"].AsString = pokemonInfoCatalog.StatusModelMotion;
                catalogBaseField["StatusModelOffset.x"].AsFloat = pokemonInfoCatalog.StatusModelOffset.X;
                catalogBaseField["StatusModelOffset.y"].AsFloat = pokemonInfoCatalog.StatusModelOffset.Y;
                catalogBaseField["StatusModelOffset.z"].AsFloat = pokemonInfoCatalog.StatusModelOffset.Z;
                catalogBaseField["StatusModelRotationAngle.x"].AsFloat = pokemonInfoCatalog.StatusModelRotationAngle.X;
                catalogBaseField["StatusModelRotationAngle.y"].AsFloat = pokemonInfoCatalog.StatusModelRotationAngle.Y;
                catalogBaseField["StatusModelRotationAngle.z"].AsFloat = pokemonInfoCatalog.StatusModelRotationAngle.Z;
                catalogBaseField["BoxScale"].AsFloat = pokemonInfoCatalog.BoxScale;
                catalogBaseField["BoxModelMotion"].AsString = pokemonInfoCatalog.BoxModelMotion;
                catalogBaseField["BoxModelOffset.x"].AsFloat = pokemonInfoCatalog.BoxModelOffset.X;
                catalogBaseField["BoxModelOffset.y"].AsFloat = pokemonInfoCatalog.BoxModelOffset.Y;
                catalogBaseField["BoxModelOffset.z"].AsFloat = pokemonInfoCatalog.BoxModelOffset.Z;
                catalogBaseField["BoxModelRotationAngle.x"].AsFloat = pokemonInfoCatalog.BoxModelRotationAngle.X;
                catalogBaseField["BoxModelRotationAngle.y"].AsFloat = pokemonInfoCatalog.BoxModelRotationAngle.Y;
                catalogBaseField["BoxModelRotationAngle.z"].AsFloat = pokemonInfoCatalog.BoxModelRotationAngle.Z;
                catalogBaseField["CompareScale"].AsFloat = pokemonInfoCatalog.CompareScale;
                catalogBaseField["CompareModelMotion"].AsString = pokemonInfoCatalog.CompareModelMotion;
                catalogBaseField["CompareModelOffset.x"].AsFloat = pokemonInfoCatalog.CompareModelOffset.X;
                catalogBaseField["CompareModelOffset.y"].AsFloat = pokemonInfoCatalog.CompareModelOffset.Y;
                catalogBaseField["CompareModelOffset.z"].AsFloat = pokemonInfoCatalog.CompareModelOffset.Z;
                catalogBaseField["CompareModelRotationAngle.x"].AsFloat = pokemonInfoCatalog.CompareModelRotationAngle.X;
                catalogBaseField["CompareModelRotationAngle.y"].AsFloat = pokemonInfoCatalog.CompareModelRotationAngle.Y;
                catalogBaseField["CompareModelRotationAngle.z"].AsFloat = pokemonInfoCatalog.CompareModelRotationAngle.Z;
                catalogBaseField["BrakeStart"].AsFloat = pokemonInfoCatalog.BrakeStart;
                catalogBaseField["BrakeEnd"].AsFloat = pokemonInfoCatalog.BrakeEnd;
                catalogBaseField["WalkSpeed"].AsFloat = pokemonInfoCatalog.WalkSpeed;
                catalogBaseField["RunSpeed"].AsFloat = pokemonInfoCatalog.RunSpeed;
                catalogBaseField["WalkStart"].AsFloat = pokemonInfoCatalog.WalkStart;
                catalogBaseField["RunStart"].AsFloat = pokemonInfoCatalog.RunStart;
                catalogBaseField["BodySize"].AsFloat = pokemonInfoCatalog.BodySize;
                catalogBaseField["AppearLimit"].AsFloat = pokemonInfoCatalog.AppearLimit;
                catalogBaseField["MoveType"].AsInt = (int)pokemonInfoCatalog.MoveType;
                catalogBaseField["GroundEffect"].AsBool = pokemonInfoCatalog.GroundEffect;
                catalogBaseField["Waitmoving"].AsBool = pokemonInfoCatalog.Waitmoving;
                catalogBaseField["BattleAjustHeight"].AsInt = (int)pokemonInfoCatalog.BattleAjustHeight;

                newCatalogs.Add(catalogBaseField);

                if (trearukiBaseField != null)
                {
                    trearukiBaseField["Enable"].AsBool = pokemonInfoCatalog.trearuki.enable;

                    List<AssetTypeValueField> newAnimeIndices = new();
                    for (int i = 0; i < pokemonInfoCatalog.trearuki.animeIndex.Count; i++)
                    {
                        AssetTypeValueField animeIndexBaseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(trearukiBaseField["AnimeIndex.Array"]);
                        animeIndexBaseField.AsInt = pokemonInfoCatalog.trearuki.animeIndex[i];
                        newAnimeIndices.Add(animeIndexBaseField);
                    }
                    trearukiBaseField["AnimeIndex.Array"].Children = newAnimeIndices;

                    List<AssetTypeValueField> newAnimeDurations = new();
                    for (int i = 0; i < pokemonInfoCatalog.trearuki.animeDuration.Count; i++)
                    {
                        AssetTypeValueField animeDurationBaseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(trearukiBaseField["AnimeDuration.Array"]);
                        animeDurationBaseField.AsFloat = pokemonInfoCatalog.trearuki.animeDuration[i];
                        newAnimeDurations.Add(animeDurationBaseField);
                    }
                    trearukiBaseField["AnimeDuration.Array"].Children = newAnimeDurations;

                    newTrearukis.Add(trearukiBaseField);
                }
            }

            pokemonInfo["Catalog.Array"].Children = newCatalogs;
            if (!pokemonInfo["Trearuki"].IsDummy)
                pokemonInfo["Trearuki.Array"].Children = newTrearukis;

            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, pokemonInfo);
        }

        private static void CommitMotionTimingData()
        {
            gameData.motionTimingData.Sort();

            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.BattleMasterdatas);
            AssetTypeValueField BattleDataTable = monoBehaviours.Find(m => m["m_Name"].AsString == "BattleDataTable");

            List<AssetTypeValueField> newMotionTimingData = new();
            foreach (BattleMasterdatas.MotionTimingData motionTimingData in gameData.motionTimingData)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(BattleDataTable["MotionTimingData.Array"]);

                baseField["MonsNo"].AsInt = motionTimingData.MonsNo;
                baseField["FormNo"].AsInt = motionTimingData.FormNo;
                baseField["Sex"].AsInt = motionTimingData.Sex;
                baseField["Buturi01"].AsInt = motionTimingData.Buturi01;
                baseField["Buturi02"].AsInt = motionTimingData.Buturi02;
                baseField["Buturi03"].AsInt = motionTimingData.Buturi03;
                baseField["Tokusyu01"].AsInt = motionTimingData.Tokusyu01;
                baseField["Tokusyu02"].AsInt = motionTimingData.Tokusyu02;
                baseField["Tokusyu03"].AsInt = motionTimingData.Tokusyu03;
                baseField["BodyBlow"].AsInt = motionTimingData.BodyBlow;
                baseField["Punch"].AsInt = motionTimingData.Punch;
                baseField["Kick"].AsInt = motionTimingData.Kick;
                baseField["Tail"].AsInt = motionTimingData.Tail;
                baseField["Bite"].AsInt = motionTimingData.Bite;
                baseField["Peck"].AsInt = motionTimingData.Peck;
                baseField["Radial"].AsInt = motionTimingData.Radial;
                baseField["Cry"].AsInt = motionTimingData.Cry;
                baseField["Dust"].AsInt = motionTimingData.Dust;
                baseField["Shot"].AsInt = motionTimingData.Shot;
                baseField["Guard"].AsInt = motionTimingData.Guard;
                baseField["LandingFall"].AsInt = motionTimingData.LandingFall;
                baseField["LandingFallEase"].AsInt = motionTimingData.LandingFallEase;

                newMotionTimingData.Add(baseField);
            }

            BattleDataTable["MotionTimingData.Array"].Children = newMotionTimingData;

            fileManager.WriteMonoBehaviour(PathEnum.BattleMasterdatas, BattleDataTable);
        }
        private static void CommitAddPersonalTable()
        {
            gameData.addPersonalTables.Sort();

            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.PersonalMasterdatas);
            AssetTypeValueField AddPersonalTable = monoBehaviours.Find(m => m["m_Name"].AsString == "AddPersonalTable");

            List<AssetTypeValueField> newAddPersonals = new();
            foreach (PersonalMasterdatas.AddPersonalTable addPersonal in gameData.addPersonalTables)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(AddPersonalTable["AddPersonal.Array"]);

                baseField["valid_flag"].AsBool = addPersonal.valid_flag;
                baseField["monsno"].AsUShort = addPersonal.monsno;
                baseField["formno"].AsUShort = addPersonal.formno;
                baseField["isEnableSynchronize"].AsBool = addPersonal.isEnableSynchronize;
                baseField["escape"].AsByte = addPersonal.escape;
                baseField["isDisableReverce"].AsBool = addPersonal.isDisableReverce;

                newAddPersonals.Add(baseField);
            }

            AddPersonalTable["AddPersonal.Array"].Children = newAddPersonals;

            fileManager.WriteMonoBehaviour(PathEnum.PersonalMasterdatas, AddPersonalTable);
        }

        private static void CommitUIMasterdatas()
        {
            gameData.uiPokemonIcon.Sort();
            gameData.uiAshiatoIcon.Sort();
            gameData.uiPokemonVoice.Sort();
            gameData.uiZukanDisplay.Sort();
            gameData.uiZukanCompareHeights.Sort();
            gameData.uiSearchPokeIconSex.Sort();

            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.UIMasterdatas);

            AssetTypeValueField uiDatabase = monoBehaviours.Find(m => m["m_Name"].AsString == "UIDatabase");
            AssetTypeValueField distributionTable = monoBehaviours.Find(m => m["m_Name"].AsString == "DistributionTable");

            // Pokemon Icon
            List<AssetTypeValueField> newPokemonIcons = new();
            foreach (UIMasterdatas.PokemonIcon pokemonIcon in gameData.uiPokemonIcon)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(uiDatabase["PokemonIcon.Array"]);

                baseField["UniqueID"].AsInt = pokemonIcon.uniqueID;
                baseField["AssetBundleName"].AsString = pokemonIcon.assetBundleName;
                baseField["AssetName"].AsString = pokemonIcon.assetName;
                baseField["AssetBundleNameLarge"].AsString = pokemonIcon.assetBundleNameLarge;
                baseField["AssetNameLarge"].AsString = pokemonIcon.assetNameLarge;
                baseField["AssetBundleNameDP"].AsString = pokemonIcon.assetBundleNameDP;
                baseField["AssetNameDP"].AsString = pokemonIcon.assetNameDP;
                baseField["HallofFameOffset.x"].AsFloat = pokemonIcon.hallofFameOffset.X;
                baseField["HallofFameOffset.y"].AsFloat = pokemonIcon.hallofFameOffset.Y;

                newPokemonIcons.Add(baseField);
            }
            uiDatabase["PokemonIcon.Array"].Children = newPokemonIcons;

            // Ashiato Icon
            List<AssetTypeValueField> newAshiatoIcons = new();
            foreach (UIMasterdatas.AshiatoIcon ashiatoIcon in gameData.uiAshiatoIcon)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(uiDatabase["AshiatoIcon.Array"]);

                baseField["UniqueID"].AsInt = ashiatoIcon.uniqueID;
                baseField["SideIconAssetName"].AsString = ashiatoIcon.sideIconAssetName;
                baseField["BothIconAssetName"].AsString = ashiatoIcon.bothIconAssetName;

                newAshiatoIcons.Add(baseField);
            }
            uiDatabase["AshiatoIcon.Array"].Children = newAshiatoIcons;

            // Pokemon Voice
            List<AssetTypeValueField> newPokemonVoices = new();
            foreach (UIMasterdatas.PokemonVoice pokemonVoice in gameData.uiPokemonVoice)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(uiDatabase["PokemonVoice.Array"]);

                baseField["UniqueID"].AsInt = pokemonVoice.uniqueID;
                baseField["WwiseEvent"].AsString = pokemonVoice.wwiseEvent;
                baseField["stopEventId"].AsString = pokemonVoice.stopEventId;
                baseField["CenterPointOffset.x"].AsFloat = pokemonVoice.centerPointOffset.X;
                baseField["CenterPointOffset.y"].AsFloat = pokemonVoice.centerPointOffset.Y;
                baseField["CenterPointOffset.z"].AsFloat = pokemonVoice.centerPointOffset.Z;
                baseField["RotationLimits"].AsBool = pokemonVoice.rotationLimits;
                baseField["RotationLimitAngle.x"].AsFloat = pokemonVoice.rotationLimitAngle.X;
                baseField["RotationLimitAngle.y"].AsFloat = pokemonVoice.rotationLimitAngle.Y;

                newPokemonVoices.Add(baseField);
            }
            uiDatabase["PokemonVoice.Array"].Children = newPokemonVoices;

            // ZukanDisplay
            List<AssetTypeValueField> newZukanDisplays = new();
            foreach (UIMasterdatas.ZukanDisplay zukanDisplay in gameData.uiZukanDisplay)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(uiDatabase["ZukanDisplay.Array"]);

                baseField["UniqueID"].AsInt = zukanDisplay.uniqueID;
                baseField["MoveLimit.x"].AsFloat = zukanDisplay.moveLimit.X;
                baseField["MoveLimit.y"].AsFloat = zukanDisplay.moveLimit.Y;
                baseField["MoveLimit.z"].AsFloat = zukanDisplay.moveLimit.Z;
                baseField["ModelOffset.x"].AsFloat = zukanDisplay.modelOffset.X;
                baseField["ModelOffset.y"].AsFloat = zukanDisplay.modelOffset.Y;
                baseField["ModelOffset.z"].AsFloat = zukanDisplay.modelOffset.Z;
                baseField["ModelRotationAngle.x"].AsFloat = zukanDisplay.modelRotationAngle.X;
                baseField["ModelRotationAngle.y"].AsFloat = zukanDisplay.modelRotationAngle.Y;

                newZukanDisplays.Add(baseField);
            }
            uiDatabase["ZukanDisplay.Array"].Children = newZukanDisplays;

            // ZukanCompareHeight
            List<AssetTypeValueField> newZukanCompareHeights = new();
            foreach (UIMasterdatas.ZukanCompareHeight zukanCompareHeight in gameData.uiZukanCompareHeights)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(uiDatabase["ZukanCompareHeight.Array"]);

                baseField["UniqueID"].AsInt = zukanCompareHeight.uniqueID;
                baseField["PlayerScaleFactor"].AsFloat = zukanCompareHeight.playerScaleFactor;
                baseField["PlayerOffset.x"].AsFloat = zukanCompareHeight.playerOffset.X;
                baseField["PlayerOffset.y"].AsFloat = zukanCompareHeight.playerOffset.Y;
                baseField["PlayerOffset.z"].AsFloat = zukanCompareHeight.playerOffset.Z;
                baseField["PlayerRotationAngle.x"].AsFloat = zukanCompareHeight.playerRotationAngle.X;
                baseField["PlayerRotationAngle.y"].AsFloat = zukanCompareHeight.playerRotationAngle.Y;

                newZukanCompareHeights.Add(baseField);
            }
            uiDatabase["ZukanCompareHeight.Array"].Children = newZukanCompareHeights;

            // SearchPokeIconSex
            List<AssetTypeValueField> newSearchPokeIconSexes = new();
            foreach (UIMasterdatas.SearchPokeIconSex searchPokeIconSex in gameData.uiSearchPokeIconSex)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(uiDatabase["SearchPokeIconSex.Array"]);

                baseField["MonsNo"].AsInt = searchPokeIconSex.monsNo;
                baseField["Sex"].AsInt = searchPokeIconSex.sex;

                newSearchPokeIconSexes.Add(baseField);
            }
            uiDatabase["SearchPokeIconSex.Array"].Children = newSearchPokeIconSexes;

            // DistributionTable
            CommitDistributionSheet(distributionTable["Diamond_FieldTable.Array"], gameData.uiDistributionTable.diamondFieldTable);
            CommitDistributionSheet(distributionTable["Diamond_DungeonTable.Array"], gameData.uiDistributionTable.diamondDungeonTable);
            CommitDistributionSheet(distributionTable["Pearl_FieldTable.Array"], gameData.uiDistributionTable.pearlFieldTable);
            CommitDistributionSheet(distributionTable["Pearl_DungeonTable.Array"], gameData.uiDistributionTable.pearlDungeonTable);

            fileManager.WriteMonoBehaviours(PathEnum.UIMasterdatas, new AssetTypeValueField[] { uiDatabase, distributionTable });
        }

        private static void CommitDistributionSheet(AssetTypeValueField sheetATVF, List<UIMasterdatas.DistributionEntry> sheet)
        {
            List<AssetTypeValueField> newSheet = new();
            foreach (UIMasterdatas.DistributionEntry entry in sheet)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(sheetATVF);

                CommitIntArray(baseField["BeforeMorning.Array"], entry.beforeMorning);
                CommitIntArray(baseField["BeforeDaytime.Array"], entry.beforeDaytime);
                CommitIntArray(baseField["BeforeNight.Array"], entry.beforeNight);
                CommitIntArray(baseField["AfterMorning.Array"], entry.afterMorning);
                CommitIntArray(baseField["AfterDaytime.Array"], entry.afterDaytime);
                CommitIntArray(baseField["AfterNight.Array"], entry.afterNight);
                CommitIntArray(baseField["Fishing.Array"], entry.fishing);
                CommitIntArray(baseField["PokemonTraser.Array"], entry.pokemonTraser);
                CommitIntArray(baseField["HoneyTree.Array"], entry.honeyTree);

                newSheet.Add(baseField);
            }

            sheetATVF.Children = newSheet;
        }

        private static void CommitIntArray(AssetTypeValueField arrayATVF, int[] values)
        {
            List<AssetTypeValueField> newInts = new();
            for (int i = 0; i < values.Length; i++)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(arrayATVF);

                baseField.AsInt = values[i];

                newInts.Add(baseField);
            }
            arrayATVF.Children = newInts;
        }

        private static void CommitUIntArray(AssetTypeValueField arrayATVF, uint[] values)
        {
            List<AssetTypeValueField> newInts = new();
            for (int i = 0; i < values.Length; i++)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(arrayATVF);

                baseField.AsUInt = values[i];

                newInts.Add(baseField);
            }
            arrayATVF.Children = newInts;
        }

        private static void CommitByteArray(AssetTypeValueField arrayATVF, byte[] values)
        {
            List<AssetTypeValueField> newInts = new();
            for (int i = 0; i < values.Length; i++)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(arrayATVF);

                baseField.AsByte = values[i];

                newInts.Add(baseField);
            }
            arrayATVF.Children = newInts;
        }

        /// <summary>
        ///  Updates loaded bundle with Pokemon.
        /// </summary>
        private static void CommitPokemon()
        {
            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.PersonalMasterdatas);

            AssetTypeValueField wazaOboeTable = monoBehaviours.Find(m => m["m_Name"].AsString == "WazaOboeTable");
            AssetTypeValueField tamagoWazaTable = monoBehaviours.Find(m => m["m_Name"].AsString == "TamagoWazaTable");
            AssetTypeValueField evolveTable = monoBehaviours.Find(m => m["m_Name"].AsString == "EvolveTable");
            AssetTypeValueField personalTable = monoBehaviours.Find(m => m["m_Name"].AsString == "PersonalTable");

            List<AssetTypeValueField> newLevelUpMoveFields = new();
            List<AssetTypeValueField> newEggMoveFields = new();
            List<AssetTypeValueField> newEvolveFields = new();
            List<AssetTypeValueField> newPersonalFields = new();
            for (int personalID = 0; personalID < gameData.personalEntries.Count; personalID++)
            {
                Pokemon pokemon = gameData.personalEntries[personalID];
                AssetTypeValueField personalField = ValueBuilder.DefaultValueFieldFromArrayTemplate(personalTable["Personal.Array"]);

                personalField["valid_flag"].AsByte = pokemon.validFlag;
                personalField["id"].AsUShort = pokemon.personalID;
                personalField["monsno"].AsUShort = pokemon.dexID;
                personalField["form_index"].AsUShort = pokemon.formIndex;
                personalField["form_max"].AsByte = pokemon.formMax;
                personalField["color"].AsByte = pokemon.color;
                personalField["gra_no"].AsUShort = pokemon.graNo;
                personalField["basic_hp"].AsByte = pokemon.basicHp;
                personalField["basic_atk"].AsByte = pokemon.basicAtk;
                personalField["basic_def"].AsByte = pokemon.basicDef;
                personalField["basic_agi"].AsByte = pokemon.basicSpd;
                personalField["basic_spatk"].AsByte = pokemon.basicSpAtk;
                personalField["basic_spdef"].AsByte = pokemon.basicSpDef;
                personalField["type1"].AsByte = pokemon.typingID1;
                personalField["type2"].AsByte = pokemon.typingID2;
                personalField["get_rate"].AsByte = pokemon.getRate;
                personalField["rank"].AsByte = pokemon.rank;
                personalField["exp_value"].AsUShort = pokemon.expValue;
                personalField["item1"].AsUShort = pokemon.item1;
                personalField["item2"].AsUShort = pokemon.item2;
                personalField["item3"].AsUShort = pokemon.item3;
                personalField["sex"].AsByte = pokemon.sex;
                personalField["egg_birth"].AsByte = pokemon.eggBirth;
                personalField["initial_friendship"].AsByte = pokemon.initialFriendship;
                personalField["egg_group1"].AsByte = pokemon.eggGroup1;
                personalField["egg_group2"].AsByte = pokemon.eggGroup2;
                personalField["grow"].AsByte = pokemon.grow;
                personalField["tokusei1"].AsUShort = pokemon.abilityID1;
                personalField["tokusei2"].AsUShort = pokemon.abilityID2;
                personalField["tokusei3"].AsUShort = pokemon.abilityID3;
                personalField["give_exp"].AsUShort = pokemon.giveExp;
                personalField["height"].AsUShort = pokemon.height;
                personalField["weight"].AsUShort = pokemon.weight;
                personalField["chihou_zukan_no"].AsUShort = pokemon.chihouZukanNo;
                personalField["machine1"].AsUInt = pokemon.machine1;
                personalField["machine2"].AsUInt = pokemon.machine2;
                personalField["machine3"].AsUInt = pokemon.machine3;
                personalField["machine4"].AsUInt = pokemon.machine4;
                personalField["hiden_machine"].AsUInt = pokemon.hiddenMachine;
                personalField["egg_monsno"].AsUShort = pokemon.eggMonsno;
                personalField["egg_formno"].AsUShort = pokemon.eggFormno;
                personalField["egg_formno_kawarazunoishi"].AsUShort = pokemon.eggFormnoKawarazunoishi;
                personalField["egg_form_inherit_kawarazunoishi"].AsByte = pokemon.eggFormInheritKawarazunoishi;

                newPersonalFields.Add(personalField);

                // Level Up Moves
                AssetTypeValueField levelUpMoveField = ValueBuilder.DefaultValueFieldFromArrayTemplate(wazaOboeTable["WazaOboe.Array"]);
                levelUpMoveField["id"].AsInt = pokemon.personalID;

                List<AssetTypeValueField> levelUpMoveAr = new();
                foreach (LevelUpMove levelUpMove in pokemon.levelUpMoves)
                {
                    AssetTypeValueField levelField = ValueBuilder.DefaultValueFieldFromArrayTemplate(levelUpMoveField["ar.Array"]);
                    AssetTypeValueField moveIDField = ValueBuilder.DefaultValueFieldFromArrayTemplate(levelUpMoveField["ar.Array"]);

                    levelField.AsUShort = levelUpMove.level;
                    moveIDField.AsUShort = levelUpMove.moveID;

                    levelUpMoveAr.Add(levelField);
                    levelUpMoveAr.Add(moveIDField);
                }
                levelUpMoveField["ar.Array"].Children = levelUpMoveAr;

                newLevelUpMoveFields.Add(levelUpMoveField);

                // Evolution Paths
                AssetTypeValueField evolveField = ValueBuilder.DefaultValueFieldFromArrayTemplate(evolveTable["Evolve.Array"]);
                evolveField["id"].AsInt = pokemon.personalID;

                List<AssetTypeValueField> evolveAr = new();
                foreach (EvolutionPath evolutionPath in pokemon.evolutionPaths)
                {
                    AssetTypeValueField methodField = ValueBuilder.DefaultValueFieldFromArrayTemplate(evolveField["ar.Array"]);
                    AssetTypeValueField parameterField = ValueBuilder.DefaultValueFieldFromArrayTemplate(evolveField["ar.Array"]);
                    AssetTypeValueField destDexIDField = ValueBuilder.DefaultValueFieldFromArrayTemplate(evolveField["ar.Array"]);
                    AssetTypeValueField destFormIDField = ValueBuilder.DefaultValueFieldFromArrayTemplate(evolveField["ar.Array"]);
                    AssetTypeValueField levelField = ValueBuilder.DefaultValueFieldFromArrayTemplate(evolveField["ar.Array"]);

                    methodField.AsUShort = evolutionPath.method;
                    parameterField.AsUShort = evolutionPath.parameter;
                    destDexIDField.AsUShort = evolutionPath.destDexID;
                    destFormIDField.AsUShort = evolutionPath.destFormID;
                    levelField.AsUShort = evolutionPath.level;

                    evolveAr.Add(methodField);
                    evolveAr.Add(parameterField);
                    evolveAr.Add(destDexIDField);
                    evolveAr.Add(destFormIDField);
                    evolveAr.Add(levelField);
                }
                evolveField["ar.Array"].Children = evolveAr;

                newEvolveFields.Add(evolveField);

                // Egg Moves
                AssetTypeValueField eggMoveField = ValueBuilder.DefaultValueFieldFromArrayTemplate(tamagoWazaTable["Data.Array"]);
                eggMoveField["no"].AsUShort = pokemon.dexID;
                eggMoveField["formNo"].AsUShort = (ushort)pokemon.formID;

                List<AssetTypeValueField> wazaNos = new();
                foreach (ushort wazaNo in pokemon.eggMoves)
                {
                    AssetTypeValueField wazaNoField = ValueBuilder.DefaultValueFieldFromArrayTemplate(eggMoveField["wazaNo.Array"]);

                    wazaNoField.AsUShort = wazaNo;

                    wazaNos.Add(wazaNoField);
                }
                eggMoveField["wazaNo.Array"].Children = wazaNos;

                newEggMoveFields.Add(eggMoveField);

                // External TM Learnsets
                if (pokemon.externalTMLearnset != null)
                    fileManager.CommitExternalJson($"MonData\\TMLearnset\\monsno_{pokemon.dexID}_formno_{pokemon.formID}.json");
            }

            newPersonalFields.Sort((atvf1, atvf2) => atvf1["id"].AsUShort.CompareTo(atvf2["id"].AsUShort));
            newLevelUpMoveFields.Sort((atvf1, atvf2) => atvf1["id"].AsInt.CompareTo(atvf2["id"].AsInt));
            newEvolveFields.Sort((atvf1, atvf2) => atvf1["id"].AsInt.CompareTo(atvf2["id"].AsInt));
            newEggMoveFields.Sort((atvf1, atvf2) =>
            {
                if (atvf1["formNo"].AsUShort.CompareTo(atvf2["formNo"].AsUShort) != 0)
                {
                    if (atvf1["formNo"].AsUShort == 0)
                        return -1;
                    if (atvf2["formNo"].AsUShort == 0)
                        return 1;
                }

                int i = atvf1["no"].AsUShort.CompareTo(atvf2["no"].AsUShort);
                if (i == 0)
                    i = atvf1["formNo"].AsUShort.CompareTo(atvf2["formNo"].AsUShort);
                return i;
            });

            wazaOboeTable["WazaOboe.Array"].Children = newLevelUpMoveFields;
            tamagoWazaTable["Data.Array"].Children = newEggMoveFields;
            evolveTable["Evolve.Array"].Children = newEvolveFields;
            personalTable["Personal.Array"].Children = newPersonalFields;

            fileManager.WriteMonoBehaviours(PathEnum.PersonalMasterdatas, new AssetTypeValueField[] { wazaOboeTable, tamagoWazaTable, evolveTable, personalTable });
        }

        /// <summary>
        ///  Updates loaded bundle with UgEncounters.
        /// </summary>
        private static void CommitUgTables()
        {
            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.Ugdata);
            List<AssetTypeValueField> updatedMonoBehaviours = new();

            AssetTypeValueField ugRandMark = monoBehaviours.Find(m => m["m_Name"].AsString == "UgRandMark");
            List<AssetTypeValueField> newUgRandMarkFields = new();
            for (int ugAreaIdx = 0; ugAreaIdx < gameData.ugAreas.Count; ugAreaIdx++)
            {
                UgArea ugArea = gameData.ugAreas[ugAreaIdx];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(ugRandMark["table.Array"]);

                baseField["id"].AsInt = ugArea.id;
                baseField["FileName"].AsString = ugArea.fileName;

                newUgRandMarkFields.Add(baseField);
            }
            ugRandMark["table.Array"].Children = newUgRandMarkFields;
            updatedMonoBehaviours.Add(ugRandMark);

            List<AssetTypeValueField> ugEncounterFileMonobehaviours = monoBehaviours.Where(m => m["m_Name"].AsString.StartsWith("UgEncount_")).ToList();
            for (int ugEncounterFileIdx = 0; ugEncounterFileIdx < ugEncounterFileMonobehaviours.Count; ugEncounterFileIdx++)
            {
                var ugEncounterMono = ugEncounterFileMonobehaviours[ugEncounterFileIdx];
                UgEncounterFile ugEncounterFile = gameData.ugEncounterFiles.Find(f => f.mName == ugEncounterMono["m_Name"].AsString);

                List<AssetTypeValueField> newUgEncounters = new();
                foreach (UgEncounter ugEncounter in ugEncounterFile.ugEncounters)
                {
                    AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(ugEncounterMono["table.Array"]);

                    baseField["monsno"].AsInt = ugEncounter.dexID;
                    baseField["version"].AsInt = ugEncounter.version;
                    baseField["zukanflag"].AsInt = ugEncounter.zukanFlag;

                    newUgEncounters.Add(baseField);
                }
                ugEncounterMono["table.Array"].Children = newUgEncounters;
                updatedMonoBehaviours.Add(ugEncounterFileMonobehaviours[ugEncounterFileIdx]);
            }

            AssetTypeValueField ugEncounterLevelMonobehaviour = monoBehaviours.Find(m => m["m_Name"].AsString == "UgEncountLevel");
            List<AssetTypeValueField> newUgEncounterLevelFields = new();
            for (int ugEncouterLevelIdx = 0; ugEncouterLevelIdx < gameData.ugEncounterLevelSets.Count; ugEncouterLevelIdx++)
            {
                UgEncounterLevelSet ugLevels = gameData.ugEncounterLevelSets[ugEncouterLevelIdx];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(ugEncounterLevelMonobehaviour["Data.Array"]);

                baseField["MinLv"].AsInt = ugLevels.minLv;
                baseField["MaxLv"].AsInt = ugLevels.maxLv;

                newUgEncounterLevelFields.Add(baseField);
            }
            ugRandMark["Data.Array"].Children = newUgEncounterLevelFields;
            updatedMonoBehaviours.Add(ugEncounterLevelMonobehaviour);

            AssetTypeValueField ugSpecialPokemon = monoBehaviours.Find(m => m["m_Name"].AsString == "UgSpecialPokemon");
            List<AssetTypeValueField> newUgSpecialPokemon = new();
            foreach (UgSpecialEncounter ugSpecialEncounter in gameData.ugSpecialEncounters)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(ugSpecialPokemon["Sheet1.Array"]);

                baseField["id"].AsInt = ugSpecialEncounter.id;
                baseField["monsno"].AsInt = ugSpecialEncounter.dexID;
                baseField["version"].AsInt = ugSpecialEncounter.version;
                baseField["Dspecialrate"].AsInt = ugSpecialEncounter.dRate;
                baseField["Pspecialrate"].AsInt = ugSpecialEncounter.pRate;

                newUgSpecialPokemon.Add(baseField);
            }
            ugSpecialPokemon["Sheet1.Array"].Children = newUgSpecialPokemon;
            updatedMonoBehaviours.Add(ugSpecialPokemon);

            AssetTypeValueField ugPokemonDataMonobehaviour = monoBehaviours.Find(m => m["m_Name"].AsString == "UgPokemonData");
            List<AssetTypeValueField> newUgPokemonData = new();
            foreach (UgPokemonData ugPokemonData in gameData.ugPokemonData)
            {
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(ugPokemonDataMonobehaviour["table.Array"]);

                baseField["monsno"].AsInt = ugPokemonData.monsno;
                baseField["type1ID"].AsInt = ugPokemonData.type1ID;
                baseField["type2ID"].AsInt = ugPokemonData.type2ID;
                baseField["size"].AsInt = ugPokemonData.size;
                baseField["movetype"].AsInt = ugPokemonData.movetype;
                CommitIntArray(baseField["reactioncode.Array"], ugPokemonData.reactioncode);
                CommitIntArray(baseField["move_rate.Array"], ugPokemonData.moveRate);
                CommitIntArray(baseField["submove_rate.Array"], ugPokemonData.submoveRate);
                CommitIntArray(baseField["reaction.Array"], ugPokemonData.reaction);
                CommitIntArray(baseField["flagrate.Array"], ugPokemonData.flagrate);

                newUgPokemonData.Add(baseField);
            }
            ugSpecialPokemon["table.Array"].Children = newUgPokemonData;
            updatedMonoBehaviours.Add(ugPokemonDataMonobehaviour);

            fileManager.WriteMonoBehaviours(PathEnum.Ugdata, updatedMonoBehaviours.ToArray());
        }

        /// <summary>
        ///  Updates loaded bundle with MessageFileSets.
        /// </summary>
        private static void CommitMessageFileSets()
        {
            List<MessageFile> messageFiles = gameData.messageFileSets.SelectMany(mfs => mfs.messageFiles).ToList();

            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.CommonMsbt);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.CommonMsbt, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.English);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.English, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.French);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.French, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.German);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.German, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.Italian);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.Italian, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.Jpn);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.Jpn, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.JpnKanji);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.JpnKanji, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.Korean);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.Korean, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.SimpChinese);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.SimpChinese, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.Spanish);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.Spanish, monoBehaviours.ToArray());

            monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.TradChinese);
            CommitMessageFiles(monoBehaviours, messageFiles);
            fileManager.WriteMonoBehaviours(PathEnum.TradChinese, monoBehaviours.ToArray());
        }

        /// <summary>
        ///  Writes all data into monobehaviors from a superset of MessageFiles.
        /// </summary>
        private static void CommitMessageFiles(List<AssetTypeValueField> monoBehaviours, List<MessageFile> messageFiles)
        {
            foreach (AssetTypeValueField monoBehaviour in monoBehaviours)
            {
                MessageFile messageFile = messageFiles.Find(mf => mf.mName == monoBehaviour["m_Name"].AsString);

                List<AssetTypeValueField> newLabelDataArray = new();
                foreach (LabelData labelData in messageFile.labelDatas)
                {
                    AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["labelDataArray.Array"]);

                    baseField["labelIndex"].AsInt = labelData.labelIndex;
                    baseField["arrayIndex"].AsInt = labelData.arrayIndex;
                    baseField["labelName"].AsString = labelData.labelName;
                    baseField["styleInfo.styleIndex"].AsInt = labelData.styleIndex;
                    baseField["styleInfo.colorIndex"].AsInt = labelData.colorIndex;
                    baseField["styleInfo.fontSize"].AsInt = labelData.fontSize;
                    baseField["styleInfo.maxWidth"].AsInt = labelData.maxWidth;
                    baseField["styleInfo.controlID"].AsInt = labelData.controlID;

                    CommitIntArray(baseField["attributeValueArray.Array"], labelData.attributeValues.ToArray());

                    List<AssetTypeValueField> tagDataArray = new();
                    foreach (TagData tagData in labelData.tagDatas)
                    {
                        AssetTypeValueField tagDataField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["tagDataArray.Array"]);

                        tagDataField["tagIndex"].AsInt = tagData.tagIndex;
                        tagDataField["groupID"].AsInt = tagData.groupID;
                        tagDataField["tagID"].AsInt = tagData.tagID;
                        tagDataField["tagPatternID"].AsInt = tagData.tagPatternID;
                        tagDataField["forceArticle"].AsInt = tagData.forceArticle;
                        tagDataField["tagParameter"].AsInt = tagData.tagParameter;

                        List<AssetTypeValueField> tagWordArray = new();
                        foreach (string tagWord in tagData.tagWordArray)
                        {
                            AssetTypeValueField tagWordField = ValueBuilder.DefaultValueFieldFromArrayTemplate(tagDataField["tagWordArray.Array"]);

                            tagWordField.AsString = tagWord;

                            tagWordArray.Add(tagWordField);
                        }
                        tagDataField["tagWordArray.Array"].Children = tagWordArray;

                        tagDataField["forceGrmID"].AsInt = tagData.forceGrmID;

                        tagDataArray.Add(tagDataField);
                    }
                    baseField["tagDataArray.Array"].Children = tagDataArray;

                    List<AssetTypeValueField> wordDataArray = new();
                    foreach (WordData wordData in labelData.wordDatas)
                    {
                        AssetTypeValueField wordDataField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["wordDataArray.Array"]);

                        wordDataField["patternID"].AsInt = wordData.patternID;
                        wordDataField["eventID"].AsInt = wordData.eventID;
                        wordDataField["tagIndex"].AsInt = wordData.tagIndex;
                        wordDataField["tagValue"].AsFloat = wordData.tagValue;
                        wordDataField["str"].AsString = wordData.str;
                        wordDataField["strWidth"].AsFloat = wordData.strWidth;

                        wordDataArray.Add(wordDataField);
                    }
                    baseField["wordDataArray.Array"].Children = wordDataArray;

                    newLabelDataArray.Add(baseField);
                }
                monoBehaviour["labelDataArray.Array"].Children = newLabelDataArray;
            }
        }

        /// <summary>
        ///  Updates loaded bundle with EncounterTables.
        /// </summary>
        private static void CommitEncounterTables()
        {
            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.Gamesettings);
            AssetTypeValueField[] encounterTableMonoBehaviours = new AssetTypeValueField[] {
                monoBehaviours.Find(m => m["m_Name"].AsString == "FieldEncountTable_d"),
                monoBehaviours.Find(m => m["m_Name"].AsString == "FieldEncountTable_p"),
            };

            for (int encounterTableFileIdx = 0; encounterTableFileIdx < encounterTableMonoBehaviours.Length; encounterTableFileIdx++)
            {
                var encounterMono = encounterTableMonoBehaviours[encounterTableFileIdx];
                EncounterTableFile encounterTableFile = Array.Find(gameData.encounterTableFiles, f => f.mName == encounterMono["m_Name"].AsString);

                //Write wild encounter tables
                List<AssetTypeValueField> newEncounterTable = new();
                foreach (EncounterTable encounterTable in encounterTableFile.encounterTables)
                {
                    AssetTypeValueField encounterTableField = ValueBuilder.DefaultValueFieldFromArrayTemplate(encounterMono["table.Array"]);

                    encounterTableField["zoneID"].AsInt = (int)encounterTable.zoneID;
                    encounterTableField["encRate_gr"].AsInt = encounterTable.encRateGround;

                    CommitIntArray(encounterTableField["FormProb.Array"], new int[] { encounterTable.formProb, encounterTable.formProb });
                    CommitIntArray(encounterTableField["AnnoonTable.Array"], new int[] { 0, encounterTable.unownTable });

                    encounterTableField["encRate_wat"].AsInt = encounterTable.encRateWater;
                    encounterTableField["encRate_turi_boro"].AsInt = encounterTable.encRateOldRod;
                    encounterTableField["encRate_turi_ii"].AsInt = encounterTable.encRateGoodRod;
                    encounterTableField["encRate_sugoi"].AsInt = encounterTable.encRateSuperRod;

                    //Write ground tables
                    SetEncounters(encounterTableField["ground_mons.Array"], encounterTable.groundMons);

                    //Write morning tables
                    SetEncounters(encounterTableField["tairyo.Array"], encounterTable.tairyo);

                    //Write day tables
                    SetEncounters(encounterTableField["day.Array"], encounterTable.day);

                    //Write night tables
                    SetEncounters(encounterTableField["night.Array"], encounterTable.night);

                    //Write pokefinder tables
                    SetEncounters(encounterTableField["swayGrass.Array"], encounterTable.swayGrass);

                    //Write ruby tables
                    SetEncounters(encounterTableField["gbaRuby.Array"], encounterTable.gbaRuby);

                    //Write sapphire tables
                    SetEncounters(encounterTableField["gbaSapp.Array"], encounterTable.gbaSapphire);

                    //Write emerald tables
                    SetEncounters(encounterTableField["gbaEme.Array"], encounterTable.gbaEmerald);

                    //Write fire tables
                    SetEncounters(encounterTableField["gbaFire.Array"], encounterTable.gbaFire);

                    //Write leaf tables
                    SetEncounters(encounterTableField["gbaLeaf.Array"], encounterTable.gbaLeaf);

                    //Write surfing tables
                    SetEncounters(encounterTableField["water_mons.Array"], encounterTable.waterMons);

                    //Write old rod tables
                    SetEncounters(encounterTableField["boro_mons.Array"], encounterTable.oldRodMons);

                    //Write good rod tables
                    SetEncounters(encounterTableField["ii_mons.Array"], encounterTable.goodRodMons);

                    //Write super rod tables
                    SetEncounters(encounterTableField["sugoi_mons.Array"], encounterTable.superRodMons);

                    newEncounterTable.Add(encounterTableField);
                }
                encounterMono["table.Array"].Children = newEncounterTable;

                //Write trophy garden table
                List<AssetTypeValueField> newTrophyGardenTable = new();
                foreach (var trophyGardenMon in encounterTableFile.trophyGardenMons)
                {
                    AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(encounterMono["urayama.Array"]);

                    baseField["monsNo"].AsInt = trophyGardenMon;

                    newTrophyGardenTable.Add(baseField);
                }
                encounterMono["urayama.Array"].Children = newTrophyGardenTable;

                //Write honey tree tables
                List<AssetTypeValueField> newHoneyTable = new();
                foreach (var honeyTreeMon in encounterTableFile.honeyTreeEnconters)
                {
                    AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(encounterMono["mistu.Array"]);

                    baseField["Rate"].AsInt = honeyTreeMon.rate;
                    baseField["Normal"].AsInt = honeyTreeMon.normalDexID;
                    baseField["Rare"].AsInt = honeyTreeMon.rareDexID;
                    baseField["SuperRare"].AsInt = honeyTreeMon.superRareDexID;

                    newHoneyTable.Add(baseField);
                }
                encounterMono["mistu.Array"].Children = newHoneyTable;

                //Write safari table
                List<AssetTypeValueField> newSafariTable = new();
                foreach (var safariMon in encounterTableFile.safariMons)
                {
                    AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(encounterMono["safari.Array"]);

                    baseField["MonsNo"].AsInt = safariMon;

                    newSafariTable.Add(baseField);
                }
                encounterMono["safari.Array"].Children = newSafariTable;
            }

            fileManager.WriteMonoBehaviours(PathEnum.Gamesettings, encounterTableMonoBehaviours);
            return;
        }

        /// <summary>
        ///  Updates loaded bundle with Trainers.
        /// </summary>
        private static void CommitTrainers()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas).Find(m => m["m_Name"].AsString == "TrainerTable");

            List<AssetTypeValueField> newTrainers = new();
            List<AssetTypeValueField> newPokes = new();
            for (int trainerIdx = 0; trainerIdx < gameData.trainers.Count; trainerIdx++)
            {
                Trainer trainer = gameData.trainers[trainerIdx];
                AssetTypeValueField trainerBaseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["TrainerData.Array"]);

                trainerBaseField["TypeID"].AsInt = trainer.trainerTypeID;
                trainerBaseField["ColorID"].AsByte = trainer.colorID;
                trainerBaseField["FightType"].AsByte = trainer.fightType;
                trainerBaseField["ArenaID"].AsInt = trainer.arenaID;
                trainerBaseField["EffectID"].AsInt = trainer.effectID;
                trainerBaseField["Gold"].AsByte = trainer.gold;
                trainerBaseField["UseItem1"].AsUShort = trainer.useItem1;
                trainerBaseField["UseItem2"].AsUShort = trainer.useItem2;
                trainerBaseField["UseItem3"].AsUShort = trainer.useItem3;
                trainerBaseField["UseItem4"].AsUShort = trainer.useItem4;
                trainerBaseField["HpRecoverFlag"].AsByte = trainer.hpRecoverFlag;
                trainerBaseField["GiftItem"].AsUShort = trainer.giftItem;
                trainerBaseField["NameLabel"].AsString = trainer.nameLabel;
                trainerBaseField["AIBit"].AsUInt = trainer.aiBit;

                newTrainers.Add(trainerBaseField);

                //Write trainer pokemon
                AssetTypeValueField pokeBaseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["TrainerPoke.Array"]);

                var pokemon = trainer.trainerPokemon.Count >= 1 ? trainer.trainerPokemon[0] : new();
                pokeBaseField["P1MonsNo"].AsUShort = pokemon.dexID;
                pokeBaseField["P1FormNo"].AsUShort = pokemon.formID;
                pokeBaseField["P1IsRare"].AsByte = pokemon.isRare;
                pokeBaseField["P1Level"].AsByte = pokemon.level;
                pokeBaseField["P1Sex"].AsByte = pokemon.sex;
                pokeBaseField["P1Seikaku"].AsByte = pokemon.natureID;
                pokeBaseField["P1Tokusei"].AsUShort = pokemon.abilityID;
                pokeBaseField["P1Waza1"].AsUShort = pokemon.moveID1;
                pokeBaseField["P1Waza2"].AsUShort = pokemon.moveID2;
                pokeBaseField["P1Waza3"].AsUShort = pokemon.moveID3;
                pokeBaseField["P1Waza4"].AsUShort = pokemon.moveID4;
                pokeBaseField["P1Item"].AsUShort = pokemon.itemID;
                pokeBaseField["P1Ball"].AsByte = pokemon.ballID;
                pokeBaseField["P1Seal"].AsInt = pokemon.seal;
                pokeBaseField["P1TalentHp"].AsByte = pokemon.hpIV;
                pokeBaseField["P1TalentAtk"].AsByte = pokemon.atkIV;
                pokeBaseField["P1TalentDef"].AsByte = pokemon.defIV;
                pokeBaseField["P1TalentSpAtk"].AsByte = pokemon.spAtkIV;
                pokeBaseField["P1TalentSpDef"].AsByte = pokemon.spDefIV;
                pokeBaseField["P1TalentAgi"].AsByte = pokemon.spdIV;
                pokeBaseField["P1EffortHp"].AsByte = pokemon.hpEV;
                pokeBaseField["P1EffortAtk"].AsByte = pokemon.atkEV;
                pokeBaseField["P1EffortDef"].AsByte = pokemon.defEV;
                pokeBaseField["P1EffortSpAtk"].AsByte = pokemon.spAtkEV;
                pokeBaseField["P1EffortSpDef"].AsByte = pokemon.spDefEV;
                pokeBaseField["P1EffortAgi"].AsByte = pokemon.spdEV;

                pokemon = trainer.trainerPokemon.Count >= 2 ? trainer.trainerPokemon[1] : new();
                pokeBaseField["P2MonsNo"].AsUShort = pokemon.dexID;
                pokeBaseField["P2FormNo"].AsUShort = pokemon.formID;
                pokeBaseField["P2IsRare"].AsByte = pokemon.isRare;
                pokeBaseField["P2Level"].AsByte = pokemon.level;
                pokeBaseField["P2Sex"].AsByte = pokemon.sex;
                pokeBaseField["P2Seikaku"].AsByte = pokemon.natureID;
                pokeBaseField["P2Tokusei"].AsUShort = pokemon.abilityID;
                pokeBaseField["P2Waza1"].AsUShort = pokemon.moveID1;
                pokeBaseField["P2Waza2"].AsUShort = pokemon.moveID2;
                pokeBaseField["P2Waza3"].AsUShort = pokemon.moveID3;
                pokeBaseField["P2Waza4"].AsUShort = pokemon.moveID4;
                pokeBaseField["P2Item"].AsUShort = pokemon.itemID;
                pokeBaseField["P2Ball"].AsByte = pokemon.ballID;
                pokeBaseField["P2Seal"].AsInt = pokemon.seal;
                pokeBaseField["P2TalentHp"].AsByte = pokemon.hpIV;
                pokeBaseField["P2TalentAtk"].AsByte = pokemon.atkIV;
                pokeBaseField["P2TalentDef"].AsByte = pokemon.defIV;
                pokeBaseField["P2TalentSpAtk"].AsByte = pokemon.spAtkIV;
                pokeBaseField["P2TalentSpDef"].AsByte = pokemon.spDefIV;
                pokeBaseField["P2TalentAgi"].AsByte = pokemon.spdIV;
                pokeBaseField["P2EffortHp"].AsByte = pokemon.hpEV;
                pokeBaseField["P2EffortAtk"].AsByte = pokemon.atkEV;
                pokeBaseField["P2EffortDef"].AsByte = pokemon.defEV;
                pokeBaseField["P2EffortSpAtk"].AsByte = pokemon.spAtkEV;
                pokeBaseField["P2EffortSpDef"].AsByte = pokemon.spDefEV;
                pokeBaseField["P2EffortAgi"].AsByte = pokemon.spdEV;

                pokemon = trainer.trainerPokemon.Count >= 3 ? trainer.trainerPokemon[2] : new();
                pokeBaseField["P3MonsNo"].AsUShort = pokemon.dexID;
                pokeBaseField["P3FormNo"].AsUShort = pokemon.formID;
                pokeBaseField["P3IsRare"].AsByte = pokemon.isRare;
                pokeBaseField["P3Level"].AsByte = pokemon.level;
                pokeBaseField["P3Sex"].AsByte = pokemon.sex;
                pokeBaseField["P3Seikaku"].AsByte = pokemon.natureID;
                pokeBaseField["P3Tokusei"].AsUShort = pokemon.abilityID;
                pokeBaseField["P3Waza1"].AsUShort = pokemon.moveID1;
                pokeBaseField["P3Waza2"].AsUShort = pokemon.moveID2;
                pokeBaseField["P3Waza3"].AsUShort = pokemon.moveID3;
                pokeBaseField["P3Waza4"].AsUShort = pokemon.moveID4;
                pokeBaseField["P3Item"].AsUShort = pokemon.itemID;
                pokeBaseField["P3Ball"].AsByte = pokemon.ballID;
                pokeBaseField["P3Seal"].AsInt = pokemon.seal;
                pokeBaseField["P3TalentHp"].AsByte = pokemon.hpIV;
                pokeBaseField["P3TalentAtk"].AsByte = pokemon.atkIV;
                pokeBaseField["P3TalentDef"].AsByte = pokemon.defIV;
                pokeBaseField["P3TalentSpAtk"].AsByte = pokemon.spAtkIV;
                pokeBaseField["P3TalentSpDef"].AsByte = pokemon.spDefIV;
                pokeBaseField["P3TalentAgi"].AsByte = pokemon.spdIV;
                pokeBaseField["P3EffortHp"].AsByte = pokemon.hpEV;
                pokeBaseField["P3EffortAtk"].AsByte = pokemon.atkEV;
                pokeBaseField["P3EffortDef"].AsByte = pokemon.defEV;
                pokeBaseField["P3EffortSpAtk"].AsByte = pokemon.spAtkEV;
                pokeBaseField["P3EffortSpDef"].AsByte = pokemon.spDefEV;
                pokeBaseField["P3EffortAgi"].AsByte = pokemon.spdEV;

                pokemon = trainer.trainerPokemon.Count >= 4 ? trainer.trainerPokemon[3] : new();
                pokeBaseField["P4MonsNo"].AsUShort = pokemon.dexID;
                pokeBaseField["P4FormNo"].AsUShort = pokemon.formID;
                pokeBaseField["P4IsRare"].AsByte = pokemon.isRare;
                pokeBaseField["P4Level"].AsByte = pokemon.level;
                pokeBaseField["P4Sex"].AsByte = pokemon.sex;
                pokeBaseField["P4Seikaku"].AsByte = pokemon.natureID;
                pokeBaseField["P4Tokusei"].AsUShort = pokemon.abilityID;
                pokeBaseField["P4Waza1"].AsUShort = pokemon.moveID1;
                pokeBaseField["P4Waza2"].AsUShort = pokemon.moveID2;
                pokeBaseField["P4Waza3"].AsUShort = pokemon.moveID3;
                pokeBaseField["P4Waza4"].AsUShort = pokemon.moveID4;
                pokeBaseField["P4Item"].AsUShort = pokemon.itemID;
                pokeBaseField["P4Ball"].AsByte = pokemon.ballID;
                pokeBaseField["P4Seal"].AsInt = pokemon.seal;
                pokeBaseField["P4TalentHp"].AsByte = pokemon.hpIV;
                pokeBaseField["P4TalentAtk"].AsByte = pokemon.atkIV;
                pokeBaseField["P4TalentDef"].AsByte = pokemon.defIV;
                pokeBaseField["P4TalentSpAtk"].AsByte = pokemon.spAtkIV;
                pokeBaseField["P4TalentSpDef"].AsByte = pokemon.spDefIV;
                pokeBaseField["P4TalentAgi"].AsByte = pokemon.spdIV;
                pokeBaseField["P4EffortHp"].AsByte = pokemon.hpEV;
                pokeBaseField["P4EffortAtk"].AsByte = pokemon.atkEV;
                pokeBaseField["P4EffortDef"].AsByte = pokemon.defEV;
                pokeBaseField["P4EffortSpAtk"].AsByte = pokemon.spAtkEV;
                pokeBaseField["P4EffortSpDef"].AsByte = pokemon.spDefEV;
                pokeBaseField["P4EffortAgi"].AsByte = pokemon.spdEV;

                pokemon = trainer.trainerPokemon.Count >= 5 ? trainer.trainerPokemon[4] : new();
                pokeBaseField["P5MonsNo"].AsUShort = pokemon.dexID;
                pokeBaseField["P5FormNo"].AsUShort = pokemon.formID;
                pokeBaseField["P5IsRare"].AsByte = pokemon.isRare;
                pokeBaseField["P5Level"].AsByte = pokemon.level;
                pokeBaseField["P5Sex"].AsByte = pokemon.sex;
                pokeBaseField["P5Seikaku"].AsByte = pokemon.natureID;
                pokeBaseField["P5Tokusei"].AsUShort = pokemon.abilityID;
                pokeBaseField["P5Waza1"].AsUShort = pokemon.moveID1;
                pokeBaseField["P5Waza2"].AsUShort = pokemon.moveID2;
                pokeBaseField["P5Waza3"].AsUShort = pokemon.moveID3;
                pokeBaseField["P5Waza4"].AsUShort = pokemon.moveID4;
                pokeBaseField["P5Item"].AsUShort = pokemon.itemID;
                pokeBaseField["P5Ball"].AsByte = pokemon.ballID;
                pokeBaseField["P5Seal"].AsInt = pokemon.seal;
                pokeBaseField["P5TalentHp"].AsByte = pokemon.hpIV;
                pokeBaseField["P5TalentAtk"].AsByte = pokemon.atkIV;
                pokeBaseField["P5TalentDef"].AsByte = pokemon.defIV;
                pokeBaseField["P5TalentSpAtk"].AsByte = pokemon.spAtkIV;
                pokeBaseField["P5TalentSpDef"].AsByte = pokemon.spDefIV;
                pokeBaseField["P5TalentAgi"].AsByte = pokemon.spdIV;
                pokeBaseField["P5EffortHp"].AsByte = pokemon.hpEV;
                pokeBaseField["P5EffortAtk"].AsByte = pokemon.atkEV;
                pokeBaseField["P5EffortDef"].AsByte = pokemon.defEV;
                pokeBaseField["P5EffortSpAtk"].AsByte = pokemon.spAtkEV;
                pokeBaseField["P5EffortSpDef"].AsByte = pokemon.spDefEV;
                pokeBaseField["P5EffortAgi"].AsByte = pokemon.spdEV;

                pokemon = trainer.trainerPokemon.Count >= 6 ? trainer.trainerPokemon[5] : new();
                pokeBaseField["P6MonsNo"].AsUShort = pokemon.dexID;
                pokeBaseField["P6FormNo"].AsUShort = pokemon.formID;
                pokeBaseField["P6IsRare"].AsByte = pokemon.isRare;
                pokeBaseField["P6Level"].AsByte = pokemon.level;
                pokeBaseField["P6Sex"].AsByte = pokemon.sex;
                pokeBaseField["P6Seikaku"].AsByte = pokemon.natureID;
                pokeBaseField["P6Tokusei"].AsUShort = pokemon.abilityID;
                pokeBaseField["P6Waza1"].AsUShort = pokemon.moveID1;
                pokeBaseField["P6Waza2"].AsUShort = pokemon.moveID2;
                pokeBaseField["P6Waza3"].AsUShort = pokemon.moveID3;
                pokeBaseField["P6Waza4"].AsUShort = pokemon.moveID4;
                pokeBaseField["P6Item"].AsUShort = pokemon.itemID;
                pokeBaseField["P6Ball"].AsByte = pokemon.ballID;
                pokeBaseField["P6Seal"].AsInt = pokemon.seal;
                pokeBaseField["P6TalentHp"].AsByte = pokemon.hpIV;
                pokeBaseField["P6TalentAtk"].AsByte = pokemon.atkIV;
                pokeBaseField["P6TalentDef"].AsByte = pokemon.defIV;
                pokeBaseField["P6TalentSpAtk"].AsByte = pokemon.spAtkIV;
                pokeBaseField["P6TalentSpDef"].AsByte = pokemon.spDefIV;
                pokeBaseField["P6TalentAgi"].AsByte = pokemon.spdIV;
                pokeBaseField["P6EffortHp"].AsByte = pokemon.hpEV;
                pokeBaseField["P6EffortAtk"].AsByte = pokemon.atkEV;
                pokeBaseField["P6EffortDef"].AsByte = pokemon.defEV;
                pokeBaseField["P6EffortSpAtk"].AsByte = pokemon.spAtkEV;
                pokeBaseField["P6EffortSpDef"].AsByte = pokemon.spDefEV;
                pokeBaseField["P6EffortAgi"].AsByte = pokemon.spdEV;

                newPokes.Add(pokeBaseField);
            }
            monoBehaviour["TrainerData.Array"].Children = newTrainers;
            monoBehaviour["TrainerPoke.Array"].Children = newPokes;

            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, monoBehaviour);
        }

        /// <summary>
        ///  Updates loaded bundle with Battle Tower Trainers.
        /// </summary>
        private static void CommitBattleTowerTrainers()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas).Find(m => m["m_Name"].AsString == "TowerSingleStockTable");
            AssetTypeValueField monoBehaviour2 = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas).Find(m => m["m_Name"].AsString == "TowerDoubleStockTable");

            List<AssetTypeValueField> newSingleTrainers = new();
            for (int trainerIdx = 0; trainerIdx < gameData.battleTowerTrainers.Count; trainerIdx++)
            {
                BattleTowerTrainer trainer = gameData.battleTowerTrainers[trainerIdx];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["TowerSingleStock.Array"]);

                baseField["ID"].AsUInt = trainer.trainerID2;
                baseField["TrainerID"].AsInt = trainer.trainerTypeID;

                CommitUIntArray(baseField["PokeID.Array"], new uint[] { trainer.battleTowerPokemonID1, trainer.battleTowerPokemonID2, trainer.battleTowerPokemonID3 });

                baseField["BattleBGM"].AsString = trainer.battleBGM;
                baseField["WinBGM"].AsString = trainer.winBGM;

                newSingleTrainers.Add(baseField);
            }
            monoBehaviour["TowerSingleStock.Array"].Children = newSingleTrainers;

            List<AssetTypeValueField> newDoubleTrainers = new();
            for (int trainerIdx = 0; trainerIdx < gameData.battleTowerTrainersDouble.Count; trainerIdx++)
            {
                BattleTowerTrainer trainer = gameData.battleTowerTrainersDouble[trainerIdx];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour2["TowerDoubleStock.Array"]);

                baseField["ID"].AsUInt = trainer.trainerID2;

                CommitIntArray(baseField["TrainerID.Array"], new int[] { trainer.trainerTypeID, trainer.trainerTypeID2 });
                CommitUIntArray(baseField["PokeID.Array"], new uint[] { trainer.battleTowerPokemonID1, trainer.battleTowerPokemonID2, trainer.battleTowerPokemonID3, trainer.battleTowerPokemonID4 });

                baseField["BattleBGM"].AsString = trainer.battleBGM;
                baseField["WinBGM"].AsString = trainer.winBGM;

                newDoubleTrainers.Add(baseField);
            }
            monoBehaviour2["TowerDoubleStock.Array"].Children = newDoubleTrainers;

            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, monoBehaviour);
            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, monoBehaviour2);
        }
        private static void CommitBattleTowerPokemon()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas).Find(m => m["m_Name"].AsString == "TowerTrainerTable");

            //Parse battle tower trainer pokemon
            List<AssetTypeValueField> newPokes = new();
            for (int pokemonIdx = 0; pokemonIdx < gameData.battleTowerTrainerPokemons.Count; pokemonIdx++)
            {
                BattleTowerTrainerPokemon pokemon = gameData.battleTowerTrainerPokemons[pokemonIdx];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["TrainerPoke.Array"]);

                baseField["ID"].AsUInt = pokemon.pokemonID;
                baseField["MonsNo"].AsInt = pokemon.dexID;
                baseField["FormNo"].AsUShort = (ushort)pokemon.formID;
                baseField["IsRare"].AsByte = pokemon.isRare;
                baseField["Level"].AsByte = pokemon.level;
                baseField["Sex"].AsByte = pokemon.sex;
                baseField["Seikaku"].AsInt = pokemon.natureID;
                baseField["Tokusei"].AsInt = pokemon.abilityID;
                baseField["Waza1"].AsInt = pokemon.moveID1;
                baseField["Waza2"].AsInt = pokemon.moveID2;
                baseField["Waza3"].AsInt = pokemon.moveID3;
                baseField["Waza4"].AsInt = pokemon.moveID4;
                baseField["Item"].AsUShort = pokemon.itemID;
                baseField["Ball"].AsByte = pokemon.ballID;
                baseField["Seal"].AsInt = pokemon.seal;
                baseField["TalentHp"].AsByte = pokemon.hpIV;
                baseField["TalentAtk"].AsByte = pokemon.atkIV;
                baseField["TalentDef"].AsByte = pokemon.defIV;
                baseField["TalentSpAtk"].AsByte = pokemon.spAtkIV;
                baseField["TalentSpDef"].AsByte = pokemon.spDefIV;
                baseField["TalentAgi"].AsByte = pokemon.spdIV;
                baseField["EffortHp"].AsByte = pokemon.hpEV;
                baseField["EffortAtk"].AsByte = pokemon.atkEV;
                baseField["EffortDef"].AsByte = pokemon.defEV;
                baseField["EffortSpAtk"].AsByte = pokemon.spAtkEV;
                baseField["EffortSpDef"].AsByte = pokemon.spDefEV;
                baseField["EffortAgi"].AsByte = pokemon.spdEV;

                newPokes.Add(baseField);
            }
            monoBehaviour["TrainerPoke.Array"].Children = newPokes;

            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, monoBehaviour);
        }

        /// <summary>
        ///  Updates loaded bundle with ShopTables.
        /// </summary>
        private static void CommitShopTables()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas).Find(m => m["m_Name"].AsString == "ShopTable");

            List<AssetTypeValueField> martItems = new();
            for (int martItemIdx = 0; martItemIdx < gameData.shopTables.martItems.Count; martItemIdx++)
            {
                MartItem martItem = gameData.shopTables.martItems[martItemIdx];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["FS.Array"]);

                baseField["ItemNo"].AsUShort = martItem.itemID;
                baseField["BadgeNum"].AsInt = martItem.badgeNum;
                baseField["ZoneID"].AsInt = martItem.zoneID;

                martItems.Add(baseField);
            }
            monoBehaviour["FS.Array"].Children = martItems;

            List<AssetTypeValueField> fixedShopItems = new();
            for (int fixedShopItemIdx = 0; fixedShopItemIdx < gameData.shopTables.fixedShopItems.Count; fixedShopItemIdx++)
            {
                FixedShopItem fixedShopItem = gameData.shopTables.fixedShopItems[fixedShopItemIdx];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["FixedShop.Array"]);

                baseField["ItemNo"].AsUShort = fixedShopItem.itemID;
                baseField["ShopID"].AsInt = fixedShopItem.shopID;

                martItems.Add(baseField);
            }
            monoBehaviour["FixedShop.Array"].Children = fixedShopItems;

            List<AssetTypeValueField> bpShopItems = new();
            for (int bpShopItemIdx = 0; bpShopItemIdx < gameData.shopTables.bpShopItems.Count; bpShopItemIdx++)
            {
                BpShopItem bpShopItem = gameData.shopTables.bpShopItems[bpShopItemIdx];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["BPShop.Array"]);

                baseField["ItemNo"].AsUShort = bpShopItem.itemID;
                baseField["NPCID"].AsInt = bpShopItem.npcID;

                bpShopItems.Add(baseField);
            }
            monoBehaviour["BPShop.Array"].Children = bpShopItems;

            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, monoBehaviour);
        }

        /// <summary>
        ///  Updates loaded bundle with PickupItems.
        /// </summary>
        private static void CommitPickupItems()
        {
            AssetTypeValueField monoBehaviour = fileManager.GetMonoBehaviours(PathEnum.DprMasterdatas).Find(m => m["m_Name"].AsString == "MonohiroiTable");

            List<AssetTypeValueField> pickupItems = new();
            for (int pickupItemIdx = 0; pickupItemIdx < gameData.pickupItems.Count; pickupItemIdx++)
            {
                PickupItem pickupItem = gameData.pickupItems[pickupItemIdx];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(monoBehaviour["MonoHiroi.Array"]);

                baseField["ID"].AsUShort = pickupItem.itemID;
                CommitByteArray(baseField["Ratios.Array"], pickupItem.ratios.ToArray());

                pickupItems.Add(baseField);
            }
            monoBehaviour["MonoHiroi.Array"].Children = pickupItems;

            fileManager.WriteMonoBehaviour(PathEnum.DprMasterdatas, monoBehaviour);
        }

        /// <summary>
        ///  Updates loaded bundle with EvScripts.
        /// </summary>
        private static void CommitEvScripts()
        {
            List<AssetTypeValueField> monoBehaviours = fileManager.GetMonoBehaviours(PathEnum.EvScript).Where(m => !m["Scripts"].IsDummy && !m["StrList"].IsDummy).ToList();

            for (int mIdx = 0; mIdx < monoBehaviours.Count; mIdx++)
            {
                var mono = monoBehaviours[mIdx];
                EvScript evScript = gameData.evScripts.Find(s => s.mName == mono["m_Name"].AsString);

                //Write Scripts
                List<AssetTypeValueField> newScripts = new();
                for (int scriptIdx = 0; scriptIdx < evScript.scripts.Count; scriptIdx++)
                {
                    Script script = evScript.scripts[scriptIdx];
                    AssetTypeValueField scriptField = ValueBuilder.DefaultValueFieldFromArrayTemplate(mono["Scripts.Array"]);

                    scriptField["Label"].AsString = script.evLabel;

                    //Write Commands
                    List<AssetTypeValueField> newCommands = new();
                    for (int commandIdx = 0; commandIdx < script.commands.Count; commandIdx++)
                    {
                        Command command = script.commands[commandIdx];
                        AssetTypeValueField commandField = ValueBuilder.DefaultValueFieldFromArrayTemplate(scriptField["Commands.Array"]);

                        AssetTypeValueField arg0Field = ValueBuilder.DefaultValueFieldFromArrayTemplate(commandField["Arg.Array"]);
                        arg0Field["argType"].AsInt = 0;
                        arg0Field["data"].AsInt = command.cmdType;

                        //Write Arguments
                        List<AssetTypeValueField> newArgs = new();
                        newArgs.Add(arg0Field);
                        for (int argIdx = 1; argIdx < command.args.Count; argIdx++)
                        {
                            Argument arg = command.args[argIdx - 1];
                            AssetTypeValueField argField = ValueBuilder.DefaultValueFieldFromArrayTemplate(commandField["Arg.Array"]);

                            argField["argType"].AsInt = arg.argType;
                            if (arg.argType == 1)
                                argField["data"].AsInt = ConvertToInt((int)arg.data);
                            else
                                argField["data"].AsInt = (int)arg.data;

                            newArgs.Add(argField);
                        }
                        commandField["Arg.Array"].Children = newArgs;
                    }
                    scriptField["Commands.Array"].Children = newCommands;
                }
                mono["Scripts.Array"].Children = newScripts;

                //Write StrLists
                List<AssetTypeValueField> newStrs = new();
                for (int stringIdx = 0; stringIdx < evScript.strList.Count; stringIdx++)
                {
                    string str = evScript.strList[stringIdx];
                    AssetTypeValueField strField = ValueBuilder.DefaultValueFieldFromArrayTemplate(mono["StrList.Array"]);

                    strField.AsString = str;

                    newStrs.Add(strField);
                }
                mono["StrList.Array"].Children = newStrs;
            }

            fileManager.WriteMonoBehaviours(PathEnum.EvScript, monoBehaviours.ToArray());
        }

        /// <summary>
        ///  Converts a List of Encounters into a AssetTypeValueField array.
        /// </summary>
        private static void SetEncounters(AssetTypeValueField encounterSetAtvf, List<Encounter> encounters)
        {
            List<AssetTypeValueField> newEncounters = new();
            for (int encounterIdx = 0; encounterIdx < encounters.Count; encounterIdx++)
            {
                Encounter encounter = encounters[encounterIdx];
                AssetTypeValueField baseField = ValueBuilder.DefaultValueFieldFromArrayTemplate(encounterSetAtvf);

                baseField["maxlv"].AsInt = encounter.maxLv;
                baseField["minlv"].AsInt = encounter.minLv;
                baseField["monsNo"].AsInt = encounter.dexID;

                newEncounters.Add(baseField);
            }
            encounterSetAtvf.Children = newEncounters;
        }

        /// <summary>
        ///  Interprets bytes of a float as an int32.
        /// </summary>
        private static int ConvertToInt(float n)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(n));
        }
    }
}
