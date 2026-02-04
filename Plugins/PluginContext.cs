using System;
using System.Windows.Forms;

namespace ImpostersOrdeal.Plugins
{
    /// <summary>
    /// Context object passed to plugins providing access to application services.
    /// </summary>
    public class PluginContext
    {
        /// <summary>
        /// The main controller instance.
        /// </summary>
        public Controller Controller { get; }

        /// <summary>
        /// The active data source provider.
        /// </summary>
        public IDataSourceProvider DataSourceProvider { get; internal set; }

        /// <summary>
        /// The main application form.
        /// </summary>
        public Form MainForm { get; internal set; }

        /// <summary>
        /// The plugin loader instance for registering additional plugins.
        /// </summary>
        public PluginLoader PluginLoader { get; }

        /// <summary>
        /// Access to the game data set.
        /// </summary>
        public GameDataSet GameData => Controller?.GetGameData();

        /// <summary>
        /// Event raised when the data source provider changes.
        /// </summary>
        public event EventHandler<DataSourceProviderChangedEventArgs> DataSourceProviderChanged;

        /// <summary>
        /// Event raised when game data has been parsed and is ready.
        /// </summary>
        public event EventHandler GameDataReady;

        /// <summary>
        /// Event raised before the application exports a mod.
        /// </summary>
        public event EventHandler BeforeExport;

        /// <summary>
        /// Event raised after the application exports a mod.
        /// </summary>
        public event EventHandler AfterExport;

        internal PluginContext(Controller controller, PluginLoader pluginLoader)
        {
            Controller = controller;
            PluginLoader = pluginLoader;
        }

        internal void RaiseDataSourceProviderChanged(IDataSourceProvider oldProvider, IDataSourceProvider newProvider)
        {
            DataSourceProviderChanged?.Invoke(this, new DataSourceProviderChangedEventArgs(oldProvider, newProvider));
        }

        internal void RaiseGameDataReady()
        {
            GameDataReady?.Invoke(this, EventArgs.Empty);
        }

        internal void RaiseBeforeExport()
        {
            BeforeExport?.Invoke(this, EventArgs.Empty);
        }

        internal void RaiseAfterExport()
        {
            AfterExport?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Event arguments for when the data source provider changes.
    /// </summary>
    public class DataSourceProviderChangedEventArgs : EventArgs
    {
        public IDataSourceProvider OldProvider { get; }
        public IDataSourceProvider NewProvider { get; }

        public DataSourceProviderChangedEventArgs(IDataSourceProvider oldProvider, IDataSourceProvider newProvider)
        {
            OldProvider = oldProvider;
            NewProvider = newProvider;
        }
    }
}
