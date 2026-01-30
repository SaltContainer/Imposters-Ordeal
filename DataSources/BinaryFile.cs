using System.IO;

namespace ImpostersOrdeal
{
    public abstract class BinaryFile : DataSource
    {
        protected byte[] data;

        public BinaryFile(string path, string rootPath) : base(path, rootPath) { }

        protected bool IsRawDataLoaded => data != null;

        protected byte[] RawData
        {
            get
            {
                if (!IsRawDataLoaded)
                    LoadBinaryFileFromFile();

                return data;
            }
        }

        public byte[] GetRawData()
        {
            return RawData;
        }

        public void SetDataFromBuffer(byte[] buffer)
        {
            data = buffer;
            dirty = true;
        }

        public override void Free()
        {
            data = null;
        }

        public override void Save(string outputPath)
        {
            if (dirty)
            {
                SaveBinaryFileToFile(outputPath);
            }
        }

        protected void LoadBinaryFileFromFile()
        {
            data = File.ReadAllBytes(System.IO.Path.Combine(rootPath, path));
        }

        protected void SaveBinaryFileToFile(string outputPath)
        {
            // Create directories if needed
            Directory.CreateDirectory(System.IO.Path.Combine(outputPath, System.IO.Path.GetDirectoryName(path)));

            using FileStream stream = File.OpenWrite(System.IO.Path.Combine(outputPath, path));
            stream.Write(data);
        }
    }
}
