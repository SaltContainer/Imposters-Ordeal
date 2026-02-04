using System;
using System.Windows.Forms;

namespace ImpostersOrdeal.Plugins
{
    /// <summary>
    /// Interface for plugins that add custom editor forms.
    /// </summary>
    public interface IEditorPlugin : IPlugin
    {
        /// <summary>
        /// Text displayed on the button that opens this editor.
        /// </summary>
        string ButtonText { get; }

        /// <summary>
        /// Category for grouping editors (e.g., "Pokemon", "Trainers", "Items", "Misc")
        /// </summary>
        string Category { get; }

        /// <summary>
        /// The data types that this editor modifies.
        /// When the editor is opened, these types will be marked as modified.
        /// </summary>
        Type[] ModifiedDataTypes { get; }

        /// <summary>
        /// Creates the editor form instance.
        /// </summary>
        /// <param name="context">The plugin context.</param>
        /// <returns>The editor form to display.</returns>
        Form CreateEditorForm(PluginContext context);

        /// <summary>
        /// Optional: Whether this editor should be enabled.
        /// Return false to disable the button (e.g., if required data is not loaded).
        /// </summary>
        /// <param name="context">The plugin context.</param>
        /// <returns>True if the editor should be enabled, false otherwise.</returns>
        bool IsEnabled(PluginContext context) => true;
    }
}
