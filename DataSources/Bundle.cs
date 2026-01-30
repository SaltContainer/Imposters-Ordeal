using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImpostersOrdeal
{
    public abstract class Bundle : DataSource
    {
        protected AssetsManager am;
        protected BundleFileInstance bundle;
        protected AssetsFileInstance assetsFile;

        protected Dictionary<long, AssetTypeValueField> monoBehaviours;
        protected Dictionary<long, string> monoScriptNames;

        protected bool IsBundleLoaded => bundle != null;
        protected bool IsAssetsFileLoaded => assetsFile != null;
        protected bool AreMonosLoaded => monoBehaviours != null;
        protected bool AreScriptNamesLoaded => monoScriptNames != null;

        protected Dictionary<long, AssetTypeValueField> MonoBehaviours
        {
            get
            {
                if (!AreMonosLoaded)
                    LoadAllMonoBehavioursFromAssetsFile();

                return monoBehaviours;
            }
        }

        protected Dictionary<long, string> MonoScriptNames
        {
            get
            {
                if (!AreScriptNamesLoaded)
                    LoadAllMonoScriptNamesFromAssetsFile();

                return monoScriptNames;
            }
        }

        public Bundle(string path, string rootPath) : base(path, rootPath)
        {
            am = new AssetsManager();
        }

        public KeyValuePair<long, AssetTypeValueField> GetMonoByName(string name)
        {
            return MonoBehaviours.First(kvp => kvp.Value["m_Name"].AsString == name);
        }

        public AssetTypeValueField GetMonoByPathID(long pathID)
        {
            return MonoBehaviours[pathID];
        }

        public List<KeyValuePair<long, AssetTypeValueField>> GetMonosWhere(Func<long, AssetTypeValueField, bool> predicate)
        {
            return MonoBehaviours.Where(kvp => predicate.Invoke(kvp.Key, kvp.Value)).ToList();
        }

        public List<KeyValuePair<long, AssetTypeValueField>> GetAllMonos()
        {
            return GetMonosWhere((p, f) => true);
        }

        public List<KeyValuePair<long, AssetTypeValueField>> GetMonosByScriptName(string name)
        {
            return GetMonosWhere((p, f) => MonoScriptNames[f["m_Script.m_PathID"].AsLong] == name);
        }

        public void SetMonoByPathID(long pathID, AssetTypeValueField data)
        {
            MonoBehaviours[pathID] = data;
            dirty = true;
        }

        public override void Free()
        {
            ClearMonoCollection();

            // TODO: Do we unload the bundle also?
            //UnloadBundleFromFile();
        }

        public override void Save(string outputPath)
        {
            if (dirty)
            {
                SaveAssetsToBundle(MonoBehaviours);
                SaveAssetsFileToBundle();
                SaveBundleToFile(outputPath);
            }
        }

        protected void ClearMonoCollection()
        {
            monoBehaviours?.Clear();
            monoBehaviours = null;
        }

        protected void UnloadBundleFromFile()
        {
            am.UnloadBundleFile(path);
            bundle = null;
            assetsFile = null;
        }

        protected void LoadBundleFromFile()
        {
            bundle = am.LoadBundleFile(System.IO.Path.Combine(rootPath, path));
            assetsFile = am.LoadAssetsFileFromBundle(bundle, 0);
        }

        protected void LoadAllMonoBehavioursFromAssetsFile()
        {
            if (!IsBundleLoaded || !IsAssetsFileLoaded)
                LoadBundleFromFile();

            monoBehaviours = GetAllAssetsOfTypeFromBundle(AssetClassID.MonoBehaviour);
        }

        protected void LoadAllMonoScriptNamesFromAssetsFile()
        {
            if (!IsBundleLoaded || !IsAssetsFileLoaded)
                LoadBundleFromFile();

            monoScriptNames = GetAllAssetsOfTypeFromBundle(AssetClassID.MonoScript).ToDictionary(kvp => kvp.Key, kvp => kvp.Value["m_Name"].AsString);
        }

        protected Dictionary<long, AssetTypeValueField> GetAllAssetsOfTypeFromBundle(AssetClassID classID)
        {
            return assetsFile.file.GetAssetsOfType(classID).ToDictionary(afie => afie.PathId, afie => am.GetBaseField(assetsFile, afie));
        }

        protected void SaveAssetsToBundle(Dictionary<long, AssetTypeValueField> assets)
        {
            foreach (var asset in assets)
                assetsFile.file.GetAssetInfo(asset.Key).SetNewData(asset.Value);
        }

        protected void SaveAssetsFileToBundle()
        {
            bundle.file.BlockAndDirInfo.DirectoryInfos[0].SetNewData(assetsFile.file);
        }

        protected void SaveBundleToFile(string outputPath, AssetBundleCompressionType compression = AssetBundleCompressionType.LZ4)
        {
            // Create directories if needed
            Directory.CreateDirectory(System.IO.Path.Combine(outputPath, System.IO.Path.GetDirectoryName(path)));

            switch (compression)
            {
                case AssetBundleCompressionType.None:
                    {
                        // Write directly to file if no compression
                        using FileStream stream = File.OpenWrite(System.IO.Path.Combine(outputPath, path));
                        using AssetsFileWriter bundleWriter = new AssetsFileWriter(stream);
                        bundle.file.Write(bundleWriter);
                        bundleWriter.Close();
                    }
                    break;

                default:
                    {
                        // Write to a temp file and then write to the actual file if compressed
                        string tempPath = System.IO.Path.Combine(outputPath, path) + ".temp";

                        using (FileStream tempStream = File.OpenWrite(tempPath))
                        {
                            using AssetsFileWriter tempBundleWriter = new AssetsFileWriter(tempStream);
                            bundle.file.Write(tempBundleWriter);
                            tempBundleWriter.Close();
                        }

                        var tempBundle = new AssetBundleFile();

                        using (FileStream tempReadStream = File.OpenRead(tempPath))
                        {
                            using AssetsFileReader tempBundleReader = new AssetsFileReader(tempReadStream);
                            tempBundle.Read(tempBundleReader);

                            using AssetsFileWriter writer = new AssetsFileWriter(System.IO.Path.Combine(outputPath, path));
                            tempBundle.Pack(writer, compression);
                            tempBundle.Close();
                        }

                        File.Delete(tempPath);
                    }
                    break;
            }
        }
    }
}
