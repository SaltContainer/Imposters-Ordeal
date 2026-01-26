using System;
using System.Data;

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
        private DataTable absoluteBoundaries;

        public Controller()
        {
            fileManager = new FileManager();
            analyzer = new Analyzer();
            randomizer = new Randomizer();
            gameData = new GameDataSet();
            flavor = new Flavor(this);

            InitializeParsers();
            InitializeAbsoluteBoundaries();
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

        private void InitializeAbsoluteBoundaries()
        {
            absoluteBoundaries = new DataTable();

            DataColumn[] columns = {
                new DataColumn("Value", typeof(string)),
                new DataColumn("Minimum", typeof(int)),
                new DataColumn("Maximum", typeof(int)),
                new DataColumn("Increment", typeof(int)),
            };

            absoluteBoundaries.Columns.AddRange(columns);

            columns[0].ReadOnly = true;

            absoluteBoundaries.Rows.Add(new object[] { "Level", 1, 100, 1 });
            absoluteBoundaries.Rows.Add(new object[] { "Base Stat", 1, 255, 1 });
            absoluteBoundaries.Rows.Add(new object[] { "Catch Rate", 1, 255, 1 });
            absoluteBoundaries.Rows.Add(new object[] { "EV Yield", 0, 3, 1 });
            absoluteBoundaries.Rows.Add(new object[] { "EV Yield Total", 1, 3, 1 });
            absoluteBoundaries.Rows.Add(new object[] { "Initial Friendship", 0, 250, 10 });
            absoluteBoundaries.Rows.Add(new object[] { "EXP Yield", 1, 65535, 1 });
            absoluteBoundaries.Rows.Add(new object[] { "Level Up Move Count", 1, 255, 1 });
            absoluteBoundaries.Rows.Add(new object[] { "Egg Move Count", 0, 255, 1 });
            absoluteBoundaries.Rows.Add(new object[] { "Power", 5, 255, 5 });
            absoluteBoundaries.Rows.Add(new object[] { "Accuracy", 5, 100, 5 });
            absoluteBoundaries.Rows.Add(new object[] { "PP", 5, 40, 5 });
            absoluteBoundaries.Rows.Add(new object[] { "Trainer Pokémon Count", 1, 6, 1 });
            absoluteBoundaries.Rows.Add(new object[] { "IV", 0, 31, 1 });
            absoluteBoundaries.Rows.Add(new object[] { "EV", 0, 255, 1 });
            absoluteBoundaries.Rows.Add(new object[] { "EV Total", 0, 510, 1 });
            absoluteBoundaries.Rows.Add(new object[] { "Price", 10, 999990, 10 });
        }

        public bool IsDataTypeModified(Type t)
        {
            return gameData.IsModified(t);
        }

        public void SetDataTypeModified(Type t)
        {
            gameData.SetModified(t);
        }

        public DataTable GetAbsoluteBoundariesTable()
        {
            return absoluteBoundaries;
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

        public RandomizerSetupConfig GetSetupConfig()
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
    }
}
