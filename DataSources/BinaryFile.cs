namespace ImpostersOrdeal
{
    public abstract class BinaryFile : DataSource
    {
        protected BinaryFileIO.BinaryFile file;

        public BinaryFile(FileManager fileManager, string path) : base(fileManager, path) { }

        protected bool IsRawDataLoaded => file?.data != null;

        protected byte[] RawData
        {
            get
            {
                if (!IsRawDataLoaded)
                    LoadBinaryFileFromFile();

                return file?.data;
            }
        }

        public byte[] GetRawData()
        {
            return RawData;
        }

        public void SetDataFromBuffer(byte[] buffer)
        {
            file.data = buffer;
            dirty = true;
        }

        public override void Free()
        {
            file.data = null;
        }

        public override void Save(string outputPath)
        {
            if (dirty)
            {
                fileManager.binaryFileIO.SaveBinaryFileToFile(file, outputPath);
            }
        }

        protected void LoadBinaryFileFromFile()
        {
            file = fileManager.binaryFileIO.GetBinaryFileAtPath(path);
        }
    }
}
