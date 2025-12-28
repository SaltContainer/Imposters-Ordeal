using System.Collections.Generic;
using System.IO;

namespace ImpostersOrdeal
{
    public class BinaryFileIO
    {
        private FileManager fileManager;
        private Dictionary<string, BinaryFile> binaryFiles = new Dictionary<string, BinaryFile>();

        public class BinaryFile
        {
            public byte[] data;
            public string path;
        }

        // TODO: Get from FileManager
        private string DumpPath => string.Empty;

        public BinaryFileIO(FileManager fileManager)
        {
            this.fileManager = fileManager;
        }

        public BinaryFile GetBinaryFileAtPath(string path)
        {
            BinaryFile file;
            if (binaryFiles.TryGetValue(path, out file))
                return file;

            LoadBinaryFileAtPath(path);

            if (binaryFiles.TryGetValue(path, out file))
                return file;
            else
                return null;
        }

        public void SaveBinaryFileToFile(BinaryFile file, string outputPath)
        {
            // Create directories if needed
            Directory.CreateDirectory(Path.Combine(outputPath, Path.GetDirectoryName(file.path)));

            using FileStream stream = File.OpenWrite(Path.Combine(outputPath, file.path));
            stream.Write(file.data);
        }

        private void LoadBinaryFileAtPath(string path)
        {
            var data = File.ReadAllBytes(path);

            binaryFiles.Add(path, new BinaryFile()
            {
                data = data,
                path = path,
            });
        }
    }
}
