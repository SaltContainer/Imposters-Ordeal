using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImpostersOrdeal
{
    public class AssetsToolsAssetBundleIO
    {
        private FileManager fileManager;
        private AssetsManager am = new AssetsManager();
        private Dictionary<string, AssetsToolsAssetBundle> bundles = new Dictionary<string, AssetsToolsAssetBundle>();

        public class AssetsToolsAssetBundle
        {
            public BundleFileInstance bundle;
            public AssetsFileInstance assetsFile;
            public string path;
        }

        public AssetsManager AssetsManager => am;

        // TODO: Get from FileManager
        private string DumpPath => string.Empty;

        public AssetsToolsAssetBundleIO(FileManager fileManager)
        {
            this.fileManager = fileManager;
        }

        public AssetsToolsAssetBundle GetBundleAtPath(string path)
        {
            AssetsToolsAssetBundle bundle;
            if (bundles.TryGetValue(path, out bundle))
                return bundle;

            LoadBundleAtPath(path);

            if (bundles.TryGetValue(path, out bundle))
                return bundle;
            else
                return null;
        }

        public Dictionary<long, AssetTypeValueField> GetAllAssetsOfTypeFromBundle(AssetsToolsAssetBundle bundle, AssetClassID classID)
        {
            return bundle.assetsFile.file.GetAssetsOfType(classID).ToDictionary(afie => afie.PathId, afie => am.GetBaseField(bundle.assetsFile, afie));
        }

        public void SaveAssetsToBundle(AssetsToolsAssetBundle bundle, Dictionary<long, AssetTypeValueField> assets)
        {
            foreach (var asset in assets)
                bundle.assetsFile.file.GetAssetInfo(asset.Key).SetNewData(asset.Value);
        }

        public void SaveAssetsFileToBundle(AssetsToolsAssetBundle bundle)
        {
            bundle.bundle.file.BlockAndDirInfo.DirectoryInfos[0].SetNewData(bundle.assetsFile.file);
        }

        public void SaveBundleToFile(AssetsToolsAssetBundle bundle, string outputPath, AssetBundleCompressionType compression = AssetBundleCompressionType.LZ4)
        {
            // Create directories if needed
            Directory.CreateDirectory(Path.Combine(outputPath, Path.GetDirectoryName(bundle.path)));

            switch (compression)
            {
                case AssetBundleCompressionType.None:
                    {
                        // Write directly to file if no compression
                        using FileStream stream = File.OpenWrite(Path.Combine(outputPath, bundle.path));
                        using AssetsFileWriter bundleWriter = new AssetsFileWriter(stream);
                        bundle.bundle.file.Write(bundleWriter);
                        bundleWriter.Close();
                    }
                    break;

                default:
                    {
                        // Write to a temp file and then write to the actual file if compressed
                        string tempPath = Path.Combine(outputPath, bundle.path) + ".temp";

                        using (FileStream tempStream = File.OpenWrite(tempPath))
                        {
                            using AssetsFileWriter tempBundleWriter = new AssetsFileWriter(tempStream);
                            bundle.bundle.file.Write(tempBundleWriter);
                            tempBundleWriter.Close();
                        }

                        var tempBundle = new AssetBundleFile();

                        using (FileStream tempReadStream = File.OpenRead(tempPath))
                        {
                            using AssetsFileReader tempBundleReader = new AssetsFileReader(tempReadStream);
                            tempBundle.Read(tempBundleReader);

                            using AssetsFileWriter writer = new AssetsFileWriter(Path.Combine(outputPath, bundle.path));
                            tempBundle.Pack(writer, compression);
                            tempBundle.Close();
                        }
                    }
                    break;
            }
        }

        public void UnloadBundleAtPath(string path)
        {
            am.UnloadBundleFile(path);
            bundles.Remove(path);
        }

        private void LoadBundleAtPath(string path)
        {
            var bfi = am.LoadBundleFile(path);
            var afi = am.LoadAssetsFileFromBundle(bfi, 0);

            bundles.Add(path, new AssetsToolsAssetBundle()
            {
                bundle = bfi,
                assetsFile = afi,
                path = path,
            });
        }
    }
}
