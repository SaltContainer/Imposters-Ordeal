using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public class GameDataSet
    {
        [ParsableData]
        public EvDataCollection evScriptFiles;

        [ParsableData]
        public PickupTable pickupTable;

        [ParsableData]
        public ShopTable shopTable;

        [ParsableData]
        public TrainerTable trainerTable;

        [ParsableData]
        public BattleTowerTable battleTowerTable;

        [ParsableData]
        public FieldEncountTableCollection encounterTableFiles;

        [ParsableData]
        public MessageFileTable messageFileTable;

        [ParsableData]
        public GrowTable growthRateTable;

        [ParsableData]
        public UgHideawayTable ugHideawayTable;

        [ParsableData]
        public UgEncounterTableCollection ugEncounterFiles;

        [ParsableData]
        public UgEncounterLevelTable ugEncounterLevelTable;

        [ParsableData]
        public UgPokemonDataTable ugPokemonDataTable;

        [ParsableData]
        public PokemonDataTable pokemonDataTable; //Ordered, idx=personalID

        [ParsableData]
        public ItemTable itemTable; //Ordered, idx=itemID, tmID

        [ParsableData]
        public MoveTable moveTable; //Ordered, idx=moveID

        [ParsableData]
        public DelphisMain delphisMainBank;

        [ParsableData]
        public GlobalMetadata globalMetadata;

        [ParsableData]
        public DprBin dprBin;

        // TODO: Remove these?
        /*public List<Ability> abilities; //Readonly
        public List<Typing> typings; //Readonly
        public List<DamageCategory> damageCategories; //Readonly
        public List<Nature> natures; //Readonly
        public List<TrainerType> trainerTypes; //Readonly
        public List<DexEntry> dexEntries; //Ordered, idx=dexID

        public List<BattleMasterdatas.MotionTimingData> motionTimingData;
        public List<Masterdatas.PokemonInfoCatalog> pokemonInfos;
        public List<PersonalMasterdatas.AddPersonalTable> addPersonalTables;
        public List<UIMasterdatas.PokemonIcon> uiPokemonIcon;
        public List<UIMasterdatas.AshiatoIcon> uiAshiatoIcon;
        public List<UIMasterdatas.PokemonVoice> uiPokemonVoice;
        public List<UIMasterdatas.ZukanDisplay> uiZukanDisplay;
        public List<UIMasterdatas.ZukanCompareHeight> uiZukanCompareHeights;
        public List<UIMasterdatas.SearchPokeIconSex> uiSearchPokeIconSex;
        public UIMasterdatas.DistributionTable uiDistributionTable;
        public List<ResultMotion> contestResultMotion;

        public List<(string name, Starter obj)> externalStarters;
        public List<(string name, HoneyTreeZone obj)> externalHoneyTrees;

        public Dictionary<string, string> trainerNames;
        public StringBuilder audioSourceLog;
        public ModArgs modArgs;*/

        /// <summary>
        /// Gets a label from a message file in a specific language by name.
        /// </summary>
        public string GetLabelByName(string fileName, string labelName, MessageEnumData.MsgLangId language = MessageEnumData.MsgLangId.USA, bool isKanji = false)
        {
            if (messageFileTable == null)
                return string.Empty;

            return messageFileTable.GetLabelByName(fileName, labelName, language, isKanji)?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Gets a label from a message file in a specific language by index.
        /// </summary>
        public string GetLabelByIndex(string fileName, int labelIndex, MessageEnumData.MsgLangId language = MessageEnumData.MsgLangId.USA, bool isKanji = false)
        {
            if (messageFileTable == null)
                return string.Empty;

            return messageFileTable.GetLabelByIndex(fileName, labelIndex, language, isKanji)?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Gets all the labels of a message file in a specific language.
        /// </summary>
        public List<string> GetAllLabels(string fileName, MessageEnumData.MsgLangId language = MessageEnumData.MsgLangId.USA, bool isKanji = false)
        {
            if (messageFileTable == null)
                return new List<string>();

            return messageFileTable.GetAllLabels(fileName, language, isKanji).Select(l => l.ToString()).ToList();
        }

        /// <summary>
        /// Gets all the labels of a message file in a specific language excluding empty label names.
        /// </summary>
        public Dictionary<string, string> GetAllLabelsDictionary(string fileName, MessageEnumData.MsgLangId language = MessageEnumData.MsgLangId.USA, bool isKanji = false)
        {
            if (messageFileTable == null)
                return new Dictionary<string, string>();

            return messageFileTable.GetAllLabels(fileName, language, isKanji).Where(l => l.labelName != string.Empty).ToDictionary(l => l.labelName, l => l.ToString());
        }

        /// <summary>
        /// Gets all the form names of a specific Pokémon species in a specific language.
        /// </summary>
        public List<string> GetAllFormNames(int dexID, MessageEnumData.MsgLangId language = MessageEnumData.MsgLangId.USA, bool isKanji = false)
        {
            if (messageFileTable == null)
                return new List<string>();

            var formatedName = string.Format("ZKN_FORM_{0:D3}", dexID);
            var baseFormName = messageFileTable.GetLabelByIndex(Constants.POKEMONSPECIES_MESSAGEFILE_NAME, dexID, language, isKanji)?.ToString() ?? string.Empty;
            var otherFormNames = messageFileTable.GetAllLabels(Constants.POKEMONFORM_MESSAGEFILE_NAME, l => l.labelName.StartsWith(formatedName) && l.labelName != formatedName + "_000", language, isKanji)
                .Select(l => l.ToString()).ToList();
            return new List<string>() { baseFormName }.Concat(otherFormNames).ToList();
        }

        protected readonly Dictionary<Type, bool> fieldStates = new Dictionary<Type, bool>();

        public bool IsModified(Type t)
        {
            if (fieldStates.ContainsKey(t))
                return fieldStates[t];
            else
                return false;
        }

        public void SetModified(Type t)
        {
            fieldStates[t] = true;
        }

        /*public enum DataField
        {
            EvScripts,
            MapWarpAssets,
            PickupItems,
            ShopTables,
            Trainers,
            BattleTowerTrainers,
            battleTowerTrainerPokemons,
            EncounterTableFiles,
            MessageFileSets,
            GrowthRates,
            UgAreas,
            UgEncounterFiles,
            UgEncounterLevelSets,
            UgSpecialEncounters,
            UgPokemonData,
            Abilities,
            Typings,
            DamageCategories,
            Natures,
            PersonalEntries,
            DexEntries,
            Items,
            TMs,
            Moves,
            AudioData,
            GlobalMetadata,
            UIMasterdatas,
            AddPersonalTable,
            MotionTimingData,
            PokemonInfo,
            ContestResultMotion,
            DprBin,
            ExternalStarters,
            ExternalHoneyTrees
        }*/



        /*public Pokemon GetPokemon(int dexID, int formID)
        {
            return dexEntries[dexID].forms[formID];
        }

        public string GetTPDisplayName(TrainerPokemon tp)
        {
            return "Lv. " + tp.level + " " + GetPokemon(tp.dexID, tp.formID).GetName();
        }

        public bool UgVersionsUnbounded()
        {
            return modArgs != null ? modArgs.ugVersionsUnbounded : ugEncounterFiles
                .SelectMany(o => o.ugEncounters)
                .Any(e => e.version < 1 || e.version > 3);
        }

        public bool Uint16UgTables()
        {
            return modArgs != null ? modArgs.uint16UgTables : ugEncounterFiles
                .SelectMany(ugef => ugef.ugEncounters)
                .Any(uge => (uint)uge.dexID > 0xFFFF); 
        }

        public bool Uint16EncounterTables()
        {
            return modArgs != null ? modArgs.uint16EncounterTables : encounterTableFiles
                .Any(etf => etf.encounterTables
                .Any(o => o.GetAllTables()
                .Any(l => l
                .Any(e => (uint)e.dexID > 0xFFFF))));
        }

        public int GetEvolutionMethodCount()
        {
            int emCount = modArgs != null ? modArgs.evolutionMethodCount : 0;
            if (emCount == 0)
                emCount = Math.Max(48, (int)personalEntries.SelectMany(p => p.evolutionPaths.Select(em => em.method)).Max());
            return emCount;
        }

        public bool FormDescriptionsExist()
        {
            return messageFileSets.SelectMany(mfs => mfs.messageFiles)
                .First(mf => mf.mName.Contains("dp_pokedex_diamond"))
                .labelDatas.Count >= personalEntries.Count - 1;
        }

        public int GetTMCompatibilitySetSize()
        {
            return personalEntries.First(p => p.IsValid()).GetTMCompatibility().Length;
        }*/
    }
}
