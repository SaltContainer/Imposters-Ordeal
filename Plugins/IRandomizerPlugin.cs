using System;
using System.Windows.Forms;

namespace ImpostersOrdeal.Plugins
{
    /// <summary>
    /// Interface for plugins that add custom randomization logic.
    /// </summary>
    public interface IRandomizerPlugin : IPlugin
    {
        /// <summary>
        /// Display name for this randomizer.
        /// </summary>
        string RandomizerName { get; }

        /// <summary>
        /// Execution order for this randomizer. Lower numbers run first.
        /// Use values like:
        /// - 0-100: Early randomization (base data modifications)
        /// - 100-200: Mid-level randomization (encounters, trainers)
        /// - 200-300: Late randomization (final adjustments)
        /// </summary>
        int ExecutionOrder { get; }

        /// <summary>
        /// Category for grouping randomizers in the UI.
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Creates a control for configuring this randomizer.
        /// This will be embedded in the main randomizer settings panel.
        /// </summary>
        /// <param name="context">The plugin context.</param>
        /// <returns>A control containing the randomizer's configuration options.</returns>
        Control GetConfigurationControl(PluginContext context);

        /// <summary>
        /// Performs the randomization.
        /// </summary>
        /// <param name="context">The plugin context.</param>
        /// <param name="rng">The random number generator to use for consistent seeding.</param>
        void Randomize(PluginContext context, Random rng);

        /// <summary>
        /// Gets the data types that this randomizer modifies.
        /// </summary>
        Type[] ModifiedDataTypes { get; }

        /// <summary>
        /// Whether this randomizer is currently enabled by the user.
        /// </summary>
        bool IsEnabled { get; set; }
    }
}
