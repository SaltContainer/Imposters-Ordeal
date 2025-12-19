namespace ImpostersOrdeal
{
    /// <summary>
    /// Base class for a source of data at a specific path in the file system.
    /// </summary>
    public abstract class DataSource
    {
        protected FileManager fileManager;
        protected string path;
        protected bool dirty = false;

        /// <summary>
        /// The path in the file system for the data source.
        /// </summary>
        public string Path { get => path; }

        public DataSource(FileManager fileManager, string path)
        {
            this.fileManager = fileManager;
            this.path = path;
        }

        /// <summary>
        /// Frees any used memory by the data source.
        /// </summary>
        public abstract void Free();

        /// <summary>
        /// Saves the data source in the file system.
        /// </summary>
        public abstract void Save(string outputPath);
    }
}
