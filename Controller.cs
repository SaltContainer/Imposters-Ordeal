using System;
using System.Collections.Generic;
using System.Data;
using ImpostersOrdeal.Plugins;

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

        // Plugin system
        private PluginLoader pluginLoader;
        private PluginContext pluginContext;
        private IDataSourceProvider activeProvider;

        /// <summary>
        /// The plugin loader instance.
        /// </summary>
        public PluginLoader PluginLoader => pluginLoader;

        /// <summary>
        /// The plugin context instance.
        /// </summary>
        public PluginContext PluginContext => pluginContext;

        /// <summary>
        /// The currently active data source provider.
        /// </summary>
        public IDataSourceProvider ActiveProvider => activeProvider;

        /// <summary>
        /// The FileManager instance (for backward compatibility).
        /// </summary>
        public FileManager FileManager => fileManager;

        public Controller()
        {
            fileManager = new FileManager();
            analyzer = new Analyzer();
            randomizer = new Randomizer();
            gameData = new GameDataSet();
            flavor = new Flavor(this);

            // Initialize plugin system
            pluginLoader = new PluginLoader();
            pluginContext = new PluginContext(this, pluginLoader);
            pluginLoader.SetContext(pluginContext);

            InitializeParsers();
            InitializeAbsoluteBoundaries();
        }

        private void InitializeParsers()
        {
            parserCollection = new(fileManager);
            InitializeParsersInternal();
        }

        private void InitializeParsersInternal()
        {
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

        /// <summary>
        /// Adds a parser for a specific data type. Used by plugins to register custom parsers.
        /// </summary>
        public void AddParser<T>(IParser<T> parser)
        {
            parserCollection.AddParserForType(parser);
        }

        /// <summary>
        /// Removes a parser for a specific data type.
        /// </summary>
        public void RemoveParser<T>(IParser<T> parser)
        {
            parserCollection.RemoveParserForType(parser);
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

        /// <summary>
        /// Initializes the plugin system. Should be called after the main form is created.
        /// </summary>
        public void InitializePlugins()
        {
            pluginLoader.Initialize();
        }

        /// <summary>
        /// Sets the active data source provider.
        /// </summary>
        /// <param name="provider">The provider to use.</param>
        public void SetActiveProvider(IDataSourceProvider provider)
        {
            var oldProvider = activeProvider;
            activeProvider = provider;

            // Update fileManager reference for backward compatibility
            if (provider != null)
            {
                fileManager = provider.GetFileManager();
                // Recreate parser collection with new file manager
                parserCollection = new ParserCollection(fileManager);
                InitializeParsersInternal();
            }

            pluginContext.DataSourceProvider = provider;
            pluginContext.RaiseDataSourceProviderChanged(oldProvider, provider);
        }

        public bool InitializeFromConfig()
        {
            if (activeProvider != null)
                return activeProvider.InitializeFromConfig();
            return fileManager.InitializeFromConfig();
        }

        public bool InitializeFromInput()
        {
            if (activeProvider != null)
                return activeProvider.InitializeFromUserInput();
            return fileManager.InitializeFromInput();
        }

        public void ParseAllData()
        {
            parserCollection.ParseAllDataForSet(gameData);
            if (activeProvider != null)
                activeProvider.FreeAll();
            else
                fileManager.FreeAll();

            // Notify plugins that game data is ready
            pluginContext.RaiseGameDataReady();
        }

        public void SaveAllData()
        {
            parserCollection.SaveAllChangedDataForSet(gameData);
        }

        public void ExportMod()
        {
            pluginContext.RaiseBeforeExport();

            if (activeProvider != null)
                activeProvider.ExportMod(System.IO.Path.Combine(Environment.CurrentDirectory, Constants.OUTPUT_FOLDER));
            else
                fileManager.ExportMod();

            pluginContext.RaiseAfterExport();
        }

        public bool AddMod()
        {
            bool result;
            List<Type> updatedSourceTypes;

            if (activeProvider != null)
                result = activeProvider.AddMod(out updatedSourceTypes);
            else
                result = fileManager.AddMod(out updatedSourceTypes);

            if (result)
            {
                var gameDataToReparse = parserCollection.GetAllGameDataTypesUsingSourceTypes(updatedSourceTypes);
                parserCollection.ParseSpecificDataForSet(gameData, gameDataToReparse);
                foreach (var reparsedType in gameDataToReparse)
                    gameData.SetModified(reparsedType);
            }

            return result;
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
