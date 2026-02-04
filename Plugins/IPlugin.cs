using System;

namespace ImpostersOrdeal.Plugins
{
    /// <summary>
    /// Base interface for all plugins.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Unique identifier for the plugin.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Human-readable name of the plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Version string of the plugin.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Author of the plugin.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Optional description of the plugin.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Called when the plugin is loaded. Use this to register services, parsers, etc.
        /// </summary>
        /// <param name="context">The plugin context providing access to application services.</param>
        void Initialize(PluginContext context);

        /// <summary>
        /// Called when the plugin is being unloaded. Use this to clean up resources.
        /// </summary>
        void Shutdown();
    }
}
