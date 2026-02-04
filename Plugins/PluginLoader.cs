using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ImpostersOrdeal.Plugins
{
    /// <summary>
    /// Discovers, loads, and manages plugins.
    /// </summary>
    public class PluginLoader
    {
        private readonly List<IPlugin> loadedPlugins = new();
        private readonly List<IDataSourceProvider> dataSourceProviders = new();
        private readonly List<IEditorPlugin> editorPlugins = new();
        private readonly List<IToolPlugin> toolPlugins = new();
        private readonly List<IRandomizerPlugin> randomizerPlugins = new();
        private PluginContext context;

        /// <summary>
        /// All loaded plugins.
        /// </summary>
        public IReadOnlyList<IPlugin> LoadedPlugins => loadedPlugins.AsReadOnly();

        /// <summary>
        /// All registered data source providers.
        /// </summary>
        public IReadOnlyList<IDataSourceProvider> DataSourceProviders => dataSourceProviders.AsReadOnly();

        /// <summary>
        /// All registered editor plugins.
        /// </summary>
        public IReadOnlyList<IEditorPlugin> EditorPlugins => editorPlugins.AsReadOnly();

        /// <summary>
        /// All registered tool plugins.
        /// </summary>
        public IReadOnlyList<IToolPlugin> ToolPlugins => toolPlugins.AsReadOnly();

        /// <summary>
        /// All registered randomizer plugins, sorted by execution order.
        /// </summary>
        public IReadOnlyList<IRandomizerPlugin> RandomizerPlugins =>
            randomizerPlugins.OrderBy(r => r.ExecutionOrder).ToList().AsReadOnly();

        /// <summary>
        /// The plugin directory path.
        /// </summary>
        public string PluginsDirectory { get; }

        public PluginLoader(string pluginsDirectory = null)
        {
            PluginsDirectory = pluginsDirectory ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
        }

        /// <summary>
        /// Sets the plugin context. Must be called before Initialize.
        /// </summary>
        public void SetContext(PluginContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Discovers and loads all plugins from the plugins directory and built-in providers.
        /// </summary>
        public void Initialize()
        {
            if (context == null)
                throw new InvalidOperationException("PluginContext must be set before Initialize is called.");

            // Load built-in providers first
            LoadBuiltInProviders();

            // Then load external plugins
            LoadExternalPlugins();

            // Initialize all loaded plugins
            foreach (var plugin in loadedPlugins)
            {
                try
                {
                    plugin.Initialize(context);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to initialize plugin {plugin.Id}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Loads built-in data source providers.
        /// </summary>
        private void LoadBuiltInProviders()
        {
            // Register the built-in RomFS provider (primary)
            var romfsProvider = new RomFSDataSourceProvider();
            RegisterPlugin(romfsProvider);

            // Register the YAML provider
            var yamlProvider = new YAMLDataSourceProvider();
            RegisterPlugin(yamlProvider);
        }

        /// <summary>
        /// Loads plugins from external DLL files in the plugins directory.
        /// </summary>
        private void LoadExternalPlugins()
        {
            if (!Directory.Exists(PluginsDirectory))
            {
                Directory.CreateDirectory(PluginsDirectory);
                return;
            }

            var pluginFiles = Directory.GetFiles(PluginsDirectory, "*.dll", SearchOption.AllDirectories);

            foreach (var pluginFile in pluginFiles)
            {
                try
                {
                    LoadPluginAssembly(pluginFile);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load plugin from {pluginFile}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Loads plugins from a specific assembly file.
        /// </summary>
        private void LoadPluginAssembly(string assemblyPath)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);
            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var pluginType in pluginTypes)
            {
                try
                {
                    var plugin = (IPlugin)Activator.CreateInstance(pluginType);
                    RegisterPlugin(plugin);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to create plugin instance of {pluginType.FullName}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Registers a plugin instance.
        /// </summary>
        public void RegisterPlugin(IPlugin plugin)
        {
            if (loadedPlugins.Any(p => p.Id == plugin.Id))
            {
                System.Diagnostics.Debug.WriteLine($"Plugin with ID {plugin.Id} is already registered.");
                return;
            }

            loadedPlugins.Add(plugin);

            // Register specialized plugin types
            if (plugin is IDataSourceProvider provider)
                dataSourceProviders.Add(provider);

            if (plugin is IEditorPlugin editor)
                editorPlugins.Add(editor);

            if (plugin is IToolPlugin tool)
                toolPlugins.Add(tool);

            if (plugin is IRandomizerPlugin randomizer)
                randomizerPlugins.Add(randomizer);
        }

        /// <summary>
        /// Unregisters a plugin by ID.
        /// </summary>
        public void UnregisterPlugin(string pluginId)
        {
            var plugin = loadedPlugins.FirstOrDefault(p => p.Id == pluginId);
            if (plugin == null)
                return;

            try
            {
                plugin.Shutdown();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during plugin shutdown: {ex.Message}");
            }

            loadedPlugins.Remove(plugin);

            if (plugin is IDataSourceProvider provider)
                dataSourceProviders.Remove(provider);

            if (plugin is IEditorPlugin editor)
                editorPlugins.Remove(editor);

            if (plugin is IToolPlugin tool)
                toolPlugins.Remove(tool);

            if (plugin is IRandomizerPlugin randomizer)
                randomizerPlugins.Remove(randomizer);
        }

        /// <summary>
        /// Gets a plugin by ID.
        /// </summary>
        public IPlugin GetPlugin(string pluginId)
        {
            return loadedPlugins.FirstOrDefault(p => p.Id == pluginId);
        }

        /// <summary>
        /// Gets a data source provider by name.
        /// </summary>
        public IDataSourceProvider GetDataSourceProvider(string providerName)
        {
            return dataSourceProviders.FirstOrDefault(p => p.ProviderName == providerName);
        }

        /// <summary>
        /// Shuts down all plugins.
        /// </summary>
        public void Shutdown()
        {
            foreach (var plugin in loadedPlugins.ToList())
            {
                try
                {
                    plugin.Shutdown();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error during plugin shutdown: {ex.Message}");
                }
            }

            loadedPlugins.Clear();
            dataSourceProviders.Clear();
            editorPlugins.Clear();
            toolPlugins.Clear();
            randomizerPlugins.Clear();
        }
    }
}
