using System;

namespace ImpostersOrdeal.Plugins
{
    /// <summary>
    /// Attribute to mark a class as a plugin entry point.
    /// The class must implement IPlugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginAttribute : Attribute
    {
        /// <summary>
        /// Unique identifier for the plugin.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Human-readable name of the plugin.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Version string of the plugin.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Author of the plugin.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Optional description of the plugin.
        /// </summary>
        public string Description { get; set; }

        public PluginAttribute(string id, string name, string version)
        {
            Id = id;
            Name = name;
            Version = version;
        }
    }
}
