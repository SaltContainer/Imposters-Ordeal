namespace ImpostersOrdeal
{
    /// <summary>
    /// Base class for a source of data at a specific path in the file system.
    /// </summary>
    public abstract class DataSource
    {
        protected string path;
        protected string rootPath;
        protected bool dirty = false;

        /// <summary>
        /// The relative path based on the root path for the data source.
        /// </summary>
        public string Path { get => path; }

        /// <summary>
        /// The absolute root path for the data source.
        /// </summary>
        public string RootPath { get => rootPath; }

        public DataSource(string path, string rootPath)
        {
            this.path = path;
            this.rootPath = rootPath;
        }

        /// <summary>
        /// Sets the data source as having been modified.
        /// </summary>
        public void SetModified()
        {
            dirty = true;
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
