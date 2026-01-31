using System.IO;

namespace ImpostersOrdeal.DataSources
{
    public class UnknownFile : DataSource
    {
        public UnknownFile(string path, string rootPath) : base(path, rootPath) { }

        public override void Free()
        {
            // Empty
        }

        public override void Save(string outputPath)
        {
            SaveBinaryFileToFile(outputPath);
        }

        protected void SaveBinaryFileToFile(string outputPath)
        {
            // Create directories if needed
            Directory.CreateDirectory(System.IO.Path.Combine(outputPath, System.IO.Path.GetDirectoryName(path)));

            File.Copy(System.IO.Path.Combine(rootPath, path), System.IO.Path.Combine(outputPath, path));
        }
    }
}
