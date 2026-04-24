using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ImpostersOrdeal
{
    /// <summary>
    /// The main class that controls the inner workings of the application.
    /// </summary>
    public class Controller
    {
        private FileManager fileManager;
        private Randomizer randomizer;
        private Analyzer analyzer;
        private GameDataSet gameData;
        private Flavor flavor;

        private ParserCollection parserCollection;

        public Controller()
        {
            fileManager = new FileManager();
            analyzer = new Analyzer();
            randomizer = new Randomizer();
            gameData = new GameDataSet();
            flavor = new Flavor(this);

            InitializeParsers();
        }

        private void InitializeParsers()
        {
            parserCollection = new(fileManager);

            parserCollection.AddParserForType(new VanillaEvDataParser());
            parserCollection.AddParserForType(new VanillaPickupParser());
            parserCollection.AddParserForType(new VanillaShopParser());
            parserCollection.AddParserForType(new VanillaTrainerParser());
            parserCollection.AddParserForType(new VanillaTowerTrainerParser());
            parserCollection.AddParserForType(new VanillaEncounterTableParser());
            parserCollection.AddParserForType(new VanillaMessageFileParser());
            parserCollection.AddParserForType(new VanillaGrowthRateParser());
            parserCollection.AddParserForType(new VanillaUgHideawayParser());
            parserCollection.AddParserForType(new VanillaUgEncounterParser());
            parserCollection.AddParserForType(new VanillaUgEncounterLevelParser());
            parserCollection.AddParserForType(new VanillaUgPokemonDataParser());
            parserCollection.AddParserForType(new VanillaPokemonDataParser());
            parserCollection.AddParserForType(new VanillaItemParser());
            parserCollection.AddParserForType(new VanillaMoveParser());
            parserCollection.AddParserForType(new VanillaDelphisMainParser());
            parserCollection.AddParserForType(new VanillaGlobalMetadataParser());
            parserCollection.AddParserForType(new VanillaDprBinParser());
        }

        public bool IsDataTypeModified(Type t)
        {
            return gameData.IsModified(t);
        }

        public void SetDataTypeModified(Type t)
        {
            gameData.SetModified(t);
        }

        public bool InitializeFromConfig()
        {
            return fileManager.InitializeFromConfig();
        }

        public bool InitializeFromInput()
        {
            return fileManager.InitializeFromInput();
        }

        public void ParseAllData()
        {
            parserCollection.ParseAllDataForSet(gameData);
            fileManager.FreeAll();
        }

        public void SaveAllData()
        {
            parserCollection.SaveAllChangedDataForSet(gameData);
        }

        public void ExportMod()
        {
            fileManager.ExportMod();
        }

        public bool AddMod()
        {
            // TODO: Remove these when done testing yaml
            parserCollection.RemoveParserForType<EvDataCollection>();
            parserCollection.AddParserForType(new YamlVanillaEvDataParser());

            var result = fileManager.AddMod(out HashSet<Type> updatedSourceTypes);

            if (result)
            {
                var gameDataToReparse = parserCollection.GetAllGameDataTypesUsingSourceTypes(updatedSourceTypes.ToList());
                parserCollection.ParseSpecificDataForSet(gameData, gameDataToReparse);
                foreach (var reparsedType in gameDataToReparse)
                    gameData.SetModified(reparsedType);
            }

            return result;
        }

        public DataTable GetAbsoluteBoundariesTable()
        {
            return AbsoluteBoundaries.GetTable();
        }

        public DistributionsSetupConfig GetInitialDistributionConfig()
        {
            return analyzer.GetSetupConfig(gameData);
        }

        public GameDataSet GetGameData()
        {
            return gameData;
        }

        public string GetFlavorSubTask()
        {
            return flavor.GetSubTask();
        }

        public string GetFlavorThought()
        {
            return flavor.GetThought();
        }

        public void Randomize(RandomizerSetupConfig rsc)
        {
            randomizer.Randomize(gameData, rsc);
        }
    }
}
