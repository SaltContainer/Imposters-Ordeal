using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ImpostersOrdeal.Plugins
{
    /// <summary>
    /// Data source provider for loading game data from YAML mod files.
    /// </summary>
    public class YAMLDataSourceProvider : IDataSourceProvider
    {
        private Dictionary<string, DataSource> sources = new();
        private string rootPath = string.Empty;
        private PluginContext context;

        // FileManager wrapper for backward compatibility with existing parsers
        private YAMLFileManagerWrapper fileManagerWrapper;

        public string Id => "builtin.yaml";
        public string Name => "YAML Mod Provider";
        public string Version => "1.0.0";
        public string Author => "Imposter's Ordeal";
        public string Description => "Loads game data from YAML mod files (human-readable format).";

        public string ProviderName => "YAML Mod";
        public string ProviderDescription => "Load game data from YAML mod files. Easier to edit manually but requires YAML-formatted data.";

        public bool IsInitialized => !string.IsNullOrEmpty(rootPath) && sources.Count > 0;
        public string RootPath => rootPath;

        public YAMLDataSourceProvider()
        {
            fileManagerWrapper = new YAMLFileManagerWrapper(this);
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
            string savedPath = ConfigurationManager.AppSettings["yamlModPath"];
            if (string.IsNullOrEmpty(savedPath) || !Directory.Exists(savedPath))
                return false;

            return Initialize(savedPath);
        }

        public bool InitializeFromUserInput()
        {
            using var fbd = new FolderBrowserDialog
            {
                Description = "Select the YAML mod folder (contains .asset files)"
            };

            if (fbd.ShowDialog() != DialogResult.OK)
                return false;

            if (!IsValidYAMLModDirectory(fbd.SelectedPath))
            {
                MessageBox.Show("Selected folder does not appear to be a valid YAML mod directory.\n" +
                    "Expected to find .asset files in the folder structure.",
                    "Invalid Directory", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            SavePathToConfig(fbd.SelectedPath);
            return Initialize(fbd.SelectedPath);
        }

        public bool Initialize(string path)
        {
            if (!Directory.Exists(path))
                return false;

            rootPath = path;
            sources.Clear();

            // Scan for all .asset files and create data sources
            var assetFiles = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);

            foreach (var file in assetFiles)
            {
                string relativePath = file.Substring(path.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var source = new YAMLDataSource(relativePath, path);
                sources[relativePath] = source;
            }

            return sources.Count > 0;
        }

        public T GetDataSource<T>() where T : DataSource
        {
            // For YAML provider, we return sources based on matching paths
            // This allows compatibility with the existing parser system
            foreach (var source in sources.Values)
            {
                if (source is T typed)
                    return typed;
            }
            return null;
        }

        public DataSource GetDataSourceByPath(string path)
        {
            return sources.TryGetValue(path, out var source) ? source : null;
        }

        public IEnumerable<DataSource> GetAllDataSources()
        {
            return sources.Values;
        }

        public bool AddMod(out List<Type> updatedSourceTypes)
        {
            updatedSourceTypes = new List<Type>();

            using var fbd = new FolderBrowserDialog
            {
                Description = "Select a YAML mod folder to merge"
            };

            if (fbd.ShowDialog() != DialogResult.OK)
                return false;

            if (!IsValidYAMLModDirectory(fbd.SelectedPath))
            {
                MessageBox.Show("Selected folder does not appear to be a valid YAML mod directory.",
                    "Invalid Directory", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var newAssetFiles = Directory.GetFiles(fbd.SelectedPath, "*.asset", SearchOption.AllDirectories);

            foreach (var file in newAssetFiles)
            {
                string relativePath = file.Substring(fbd.SelectedPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                // Replace or add the data source
                var newSource = new YAMLDataSource(relativePath, fbd.SelectedPath);

                if (sources.ContainsKey(relativePath))
                {
                    // Track that this source type was updated
                    updatedSourceTypes.Add(sources[relativePath].GetType());
                }

                sources[relativePath] = newSource;
                updatedSourceTypes.Add(newSource.GetType());
            }

            return updatedSourceTypes.Count > 0;
        }

        public void ExportMod(string outputPath)
        {
            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, true);

            Directory.CreateDirectory(outputPath);

            foreach (var source in sources.Values)
            {
                source.Save(outputPath);
            }
        }

        public void FreeAll()
        {
            foreach (var source in sources.Values)
            {
                source.Free();
            }
            GC.Collect();
        }

        public FileManager GetFileManager()
        {
            // Return wrapper that delegates to YAML sources
            return fileManagerWrapper;
        }

        private bool IsValidYAMLModDirectory(string path)
        {
            // Check if there are any .asset files
            return Directory.Exists(path) &&
                   Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories).Length > 0;
        }

        private void SavePathToConfig(string path)
        {
            try
            {
                Configuration c = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Add setting if it doesn't exist
                if (c.AppSettings.Settings["yamlModPath"] == null)
                    c.AppSettings.Settings.Add("yamlModPath", path);
                else
                    c.AppSettings.Settings["yamlModPath"].Value = path;

                c.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(c.AppSettings.SectionInformation.Name);
            }
            catch
            {
                // Ignore config save failures
            }
        }
    }

    /// <summary>
    /// Wrapper around FileManager that delegates to YAML data sources.
    /// This provides compatibility with existing parsers that expect FileManager.
    /// </summary>
    internal class YAMLFileManagerWrapper : FileManager
    {
        private readonly YAMLDataSourceProvider provider;

        public YAMLFileManagerWrapper(YAMLDataSourceProvider provider)
        {
            this.provider = provider;
        }
    }
}
