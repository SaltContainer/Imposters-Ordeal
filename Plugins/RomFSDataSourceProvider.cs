using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImpostersOrdeal.Plugins
{
    /// <summary>
    /// Built-in data source provider for loading game data from RomFS binary bundles.
    /// This wraps the existing FileManager functionality as a plugin.
    /// </summary>
    public class RomFSDataSourceProvider : IDataSourceProvider
    {
        private FileManager fileManager;
        private PluginContext context;

        public string Id => "builtin.romfs";
        public string Name => "RomFS Binary Provider";
        public string Version => "1.0.0";
        public string Author => "Imposter's Ordeal";
        public string Description => "Loads game data from RomFS binary asset bundles.";

        public string ProviderName => "RomFS Binary";
        public string ProviderDescription => "Load game data directly from binary asset bundles (original game format)";

        public bool IsInitialized => fileManager != null && !string.IsNullOrEmpty(fileManager.DumpPath);
        public string RootPath => fileManager?.DumpPath ?? string.Empty;

        public RomFSDataSourceProvider()
        {
            fileManager = new FileManager();
        }

        public void Initialize(PluginContext context)
        {
            this.context = context;
        }

        public void Shutdown()
        {
            FreeAll();
        }

        public bool InitializeFromConfig()
        {
            return fileManager.InitializeFromConfig();
        }

        public bool InitializeFromUserInput()
        {
            return fileManager.InitializeFromInput();
        }

        public bool Initialize(string rootPath)
        {
            // For RomFS, we use the fileManager's internal initialization
            // which validates the path and sets up data sources
            fileManager.DumpPath = rootPath;

            // Check if it's a valid game directory
            if (!Directory.Exists(Path.Combine(rootPath, "romfs")))
                return false;

            // Use reflection to call the private SetupDumpDataSources method
            var method = typeof(FileManager).GetMethod("SetupDumpDataSources",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                method.Invoke(fileManager, new object[] { rootPath });
                return true;
            }

            return false;
        }

        public T GetDataSource<T>() where T : DataSource
        {
            // Map types to their getter methods
            return typeof(T).Name switch
            {
                nameof(BattleMasterdatasBundle) => fileManager.GetBattleMasterdatasBundle() as T,
                nameof(ContestMasterdatasBundle) => fileManager.GetContestMasterdatasBundle() as T,
                nameof(EvScriptBundle) => fileManager.GetEvScriptBundle() as T,
                nameof(DprMasterdatasBundle) => fileManager.GetDprMasterdatasBundle() as T,
                nameof(GameSettingsBundle) => fileManager.GetGameSettingsBundle() as T,
                nameof(CommonMsbtBundle) => fileManager.GetCommonMsbtBundle() as T,
                nameof(EnglishMessageBundle) => fileManager.GetEnglishMessageBundle() as T,
                nameof(FrenchMessageBundle) => fileManager.GetFrenchMessageBundle() as T,
                nameof(GermanMessageBundle) => fileManager.GetGermanMessageBundle() as T,
                nameof(ItalianMessageBundle) => fileManager.GetItalianMessageBundle() as T,
                nameof(JapaneseMessageBundle) => fileManager.GetJapaneseMessageBundle() as T,
                nameof(JapaneseKanjiMessageBundle) => fileManager.GetJapaneseKanjiMessageBundle() as T,
                nameof(KoreanMessageBundle) => fileManager.GetKoreanMessageBundle() as T,
                nameof(SimplifiedChineseMessageBundle) => fileManager.GetSimplifiedChineseMessageBundle() as T,
                nameof(SpanishMessageBundle) => fileManager.GetSpanishMessageBundle() as T,
                nameof(TraditionalChineseMessageBundle) => fileManager.GetTraditionalChineseMessageBundle() as T,
                nameof(PersonalMasterdatasBundle) => fileManager.GetPersonalMasterdatasBundle() as T,
                nameof(UIMasterdatasBundle) => fileManager.GetUIMasterdatasBundle() as T,
                nameof(UGDataBundle) => fileManager.GetUGDataBundle() as T,
                nameof(DelphisMainBank) => fileManager.GetDelphisMainBank() as T,
                nameof(GlobalMetadataFile) => fileManager.GetGlobalMetadataFile() as T,
                nameof(DprBinABDM) => fileManager.GetDprBinFile() as T,
                _ => null
            };
        }

        public DataSource GetDataSourceByPath(string path)
        {
            // Use reflection to access the private sources dictionary
            var sourcesField = typeof(FileManager).GetField("sources",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (sourcesField != null)
            {
                var sources = sourcesField.GetValue(fileManager) as Dictionary<string, DataSource>;
                if (sources != null && sources.TryGetValue(path, out var source))
                    return source;
            }

            return null;
        }

        public IEnumerable<DataSource> GetAllDataSources()
        {
            var sourcesField = typeof(FileManager).GetField("sources",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (sourcesField != null)
            {
                var sources = sourcesField.GetValue(fileManager) as Dictionary<string, DataSource>;
                if (sources != null)
                    return sources.Values;
            }

            return Enumerable.Empty<DataSource>();
        }

        public bool AddMod(out List<Type> updatedSourceTypes)
        {
            return fileManager.AddMod(out updatedSourceTypes);
        }

        public void ExportMod(string outputPath)
        {
            fileManager.ExportMod();
        }

        public void FreeAll()
        {
            fileManager?.FreeAll();
        }

        public FileManager GetFileManager()
        {
            return fileManager;
        }
    }
}
