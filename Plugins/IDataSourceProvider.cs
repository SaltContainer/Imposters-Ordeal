using System;
using System.Collections.Generic;

namespace ImpostersOrdeal.Plugins
{
    /// <summary>
    /// Interface for data source providers that can load game data from different formats (RomFS, YAML, etc.)
    /// </summary>
    public interface IDataSourceProvider : IPlugin
    {
        /// <summary>
        /// Display name for this provider (e.g., "RomFS Binary", "YAML Mod")
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Short description of what this provider does.
        /// </summary>
        string ProviderDescription { get; }

        /// <summary>
        /// Whether this provider is currently initialized and ready to use.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// The root path where data is loaded from.
        /// </summary>
        string RootPath { get; }

        /// <summary>
        /// Initialize the provider from a saved configuration.
        /// </summary>
        /// <returns>True if initialization succeeded, false otherwise.</returns>
        bool InitializeFromConfig();

        /// <summary>
        /// Initialize the provider by prompting the user for input.
        /// </summary>
        /// <returns>True if initialization succeeded, false otherwise.</returns>
        bool InitializeFromUserInput();

        /// <summary>
        /// Initialize the provider with a specific root path.
        /// </summary>
        /// <param name="rootPath">The root path to load data from.</param>
        /// <returns>True if initialization succeeded, false otherwise.</returns>
        bool Initialize(string rootPath);

        /// <summary>
        /// Gets a data source of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of data source to retrieve.</typeparam>
        /// <returns>The data source, or null if not available.</returns>
        T GetDataSource<T>() where T : DataSource;

        /// <summary>
        /// Gets a data source by its path.
        /// </summary>
        /// <param name="path">The relative path of the data source.</param>
        /// <returns>The data source, or null if not found.</returns>
        DataSource GetDataSourceByPath(string path);

        /// <summary>
        /// Gets all registered data sources.
        /// </summary>
        IEnumerable<DataSource> GetAllDataSources();

        /// <summary>
        /// Adds a mod from a directory.
        /// </summary>
        /// <param name="updatedSourceTypes">Output list of data source types that were updated.</param>
        /// <returns>True if the mod was added successfully.</returns>
        bool AddMod(out List<Type> updatedSourceTypes);

        /// <summary>
        /// Exports the current modifications to the specified output path.
        /// </summary>
        /// <param name="outputPath">The directory to export to.</param>
        void ExportMod(string outputPath);

        /// <summary>
        /// Frees all memory used by loaded data sources.
        /// </summary>
        void FreeAll();

        /// <summary>
        /// Gets the FileManager instance used by this provider.
        /// This is for compatibility with existing parsers.
        /// </summary>
        FileManager GetFileManager();
    }
}
