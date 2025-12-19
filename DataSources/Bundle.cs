using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public abstract class Bundle : DataSource
    {
        protected AssetsToolsAssetBundleIO.AssetsToolsAssetBundle assetBundle;
        protected Dictionary<long, AssetTypeValueField> monoBehaviours;

        protected bool IsBundleLoaded => assetBundle?.bundle != null;
        protected bool IsAssetsFileLoaded => assetBundle?.assetsFile != null;
        protected bool AreMonosLoaded => monoBehaviours != null;

        protected Dictionary<long, AssetTypeValueField> MonoBehaviours
        {
            get
            {
                if (!AreMonosLoaded)
                    LoadAllMonoBehavioursFromAssetsFile();

                return monoBehaviours;
            }
        }

        public Bundle(FileManager fileManager, string path) : base(fileManager, path) { }

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

        public List<AssetTypeValueField> GetMonosWhere(Func<AssetTypeValueField, bool> predicate)
        {
            return MonoBehaviours.Values.Where(predicate).ToList();
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
                fileManager.assetBundleIO.SaveAssetsToBundle(assetBundle, MonoBehaviours);
                fileManager.assetBundleIO.SaveAssetsFileToBundle(assetBundle);
                fileManager.assetBundleIO.SaveBundleToFile(assetBundle, outputPath);
            }
        }

        protected void ClearMonoCollection()
        {
            monoBehaviours = null;
        }

        protected void UnloadBundleFromFile()
        {
            fileManager.assetBundleIO.UnloadBundleAtPath(path);
            assetBundle = null;
        }

        protected void LoadBundleFromFile()
        {
            assetBundle = fileManager.assetBundleIO.GetBundleAtPath(path);
        }

        protected void LoadAllMonoBehavioursFromAssetsFile()
        {
            if (!IsBundleLoaded || !IsAssetsFileLoaded)
                LoadBundleFromFile();

            monoBehaviours = fileManager.assetBundleIO.GetAllAssetsOfTypeFromBundle(assetBundle, AssetClassID.MonoBehaviour);
        }
    }
}
