using System;
using System.Windows.Forms;

namespace ImpostersOrdeal.Plugins
{
    /// <summary>
    /// Interface for plugins that add utility tools.
    /// </summary>
    public interface IToolPlugin : IPlugin
    {
        /// <summary>
        /// Text displayed on the button that runs this tool.
        /// </summary>
        string ButtonText { get; }

        /// <summary>
        /// Category for grouping tools (e.g., "Import/Export", "Conversion", "Analysis")
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Executes the tool.
        /// </summary>
        /// <param name="context">The plugin context.</param>
        void Execute(PluginContext context);

        /// <summary>
        /// Optional: Whether this tool should be enabled.
        /// Return false to disable the button (e.g., if prerequisites are not met).
        /// </summary>
        /// <param name="context">The plugin context.</param>
        /// <returns>True if the tool should be enabled, false otherwise.</returns>
        bool IsEnabled(PluginContext context) => true;

        /// <summary>
        /// Optional: Creates a form for this tool if it has a GUI.
        /// Return null if the tool is executed directly without a form.
        /// </summary>
        /// <param name="context">The plugin context.</param>
        /// <returns>The tool form to display, or null for immediate execution.</returns>
        Form CreateToolForm(PluginContext context) => null;
    }
}
