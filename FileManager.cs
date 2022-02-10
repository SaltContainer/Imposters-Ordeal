﻿using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BDSP_Randomizer
{
    /// <summary>
    ///  Responsible for handling all files.
    /// </summary>
    public class FileManager
    {
        private readonly string randomizerModName = "Randomizer Output";
        public static readonly string[] assetAssistantRandomizerFiles = new string[]
        {
            "\\Dpr\\ev_script",
            "\\Dpr\\masterdatas",
            "\\Dpr\\scriptableobjects\\gamesettings",
            "\\Message\\common_msbt",
            "\\Message\\english",
            "\\Message\\french",
            "\\Message\\german",
            "\\Message\\italian",
            "\\Message\\jpn",
            "\\Message\\jpn_kanji",
            "\\Message\\korean",
            "\\Message\\simp_chinese",
            "\\Message\\spanish",
            "\\Message\\trad_chinese",
            "\\Pml\\personal_masterdatas",
            "\\UnderGround\\data\\ugdata"
        };

        private Dictionary<string, FileData> fileArchive;
        private AssetsManager am = new();
        private int fileIndex = 0;

        private class FileData
        {
            public string fileLocation;
            public string gamePath;
            public FileSource fileSource;
            public BundleFileInstance bundle;
            public bool tempLocation;

            public bool IsBundle()
            {
                return bundle != null;
            }
        }

        private enum FileSource
        {
            Dump,
            Mod,
            UnrelatedMod,
            Randomizer
        }

        /// <summary>
        ///  Gets dump from user and opens all necessary files.
        /// </summary>
        internal bool InitializeFromDump()
        {
            //Get the dump path from user.
            FolderBrowserDialog fbd = new();
            fbd.Description = "Select the folder containing the romfs/exefs.";
            if (fbd.ShowDialog() != DialogResult.OK)
                return false;

            //Check that it's a game directory
            if (!IsGameDirectory(fbd.SelectedPath, true))
            {
                MessageBox.Show("Path does not contain a romfs folder.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //Get the AssetAssistant path
            string assetAssistantPath = GetAssetAssistantPath(fbd.SelectedPath);
            if (assetAssistantPath == "")
            {
                MessageBox.Show("Path does not contain path:\n\\romfs\\Data\\StreamingAssets\\AssetAssistant",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //Setup fileArchive
            fileArchive = new();
            for (int i = 0; i < assetAssistantRandomizerFiles.Length; i++)
            {
                string absolutePath = assetAssistantPath + assetAssistantRandomizerFiles[i];
                string gamePath = "romfs\\Data\\StreamingAssets\\AssetAssistant" + assetAssistantRandomizerFiles[i];
                if (!File.Exists(absolutePath))
                {
                    MessageBox.Show("File not found:\n" + gamePath + "\nIncomplete dump.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fileArchive = null;
                    return false;
                }

                FileData fd = new();
                fd.fileLocation = absolutePath;
                fd.gamePath = gamePath;
                fd.fileSource = FileSource.Dump;
                fd.bundle = am.LoadBundleFile(absolutePath, false);
                DecompressBundle(fd.bundle);
                fileArchive[gamePath] = fd;
            }

            return true;
        }

        /// <summary>
        ///  Gets a mod directory from user and loads all the files it contains into fileArchive.
        /// </summary>
        public bool AddMod()
        {
            bool reanalysisNecessary = false;

            //Get the dump path from user
            FolderBrowserDialog fbd = new();
            fbd.Description = "Select a mod folder containing a romfs and/or exefs.";
            if (fbd.ShowDialog() != DialogResult.OK)
                return reanalysisNecessary;

            //Check that it's a game directory
            if (!IsGameDirectory(fbd.SelectedPath))
            {
                MessageBox.Show("Path does not contain a romfs or exefs folder.",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return reanalysisNecessary;
            }

            //Loads all files
            string[] modFilePaths = Directory.GetFiles(fbd.SelectedPath, "*", SearchOption.AllDirectories);
            List<(int, string)> conflicts = new();
            for (int fileIdx = 0; fileIdx < modFilePaths.Length; fileIdx++)
            {
                string absolutePath = fbd.SelectedPath;
                string gamePath = modFilePaths[fileIdx].Substring(absolutePath.Length + 1, modFilePaths[fileIdx].Length - absolutePath.Length - 1);
                if (!fileArchive.ContainsKey(gamePath))
                {
                    FileData fd = new();
                    fd.fileLocation = modFilePaths[fileIdx];
                    fd.gamePath = gamePath;
                    fd.fileSource = FileSource.UnrelatedMod;

                    if (gamePath.Contains("AssetAssistant") && Path.GetExtension(gamePath) == "")
                        try
                        {
                            fd.bundle = am.LoadBundleFile(modFilePaths[fileIdx], false);
                            DecompressBundle(fd.bundle);
                        }
                        catch (Exception) { }

                    fileArchive[gamePath] = fd;
                    continue;
                }

                if (fileArchive[gamePath].fileSource == FileSource.Dump)
                {
                    fileArchive[gamePath].fileLocation = modFilePaths[fileIdx];
                    fileArchive[gamePath].fileSource = FileSource.Mod;
                    fileArchive[gamePath].bundle = am.LoadBundleFile(modFilePaths[fileIdx], false);
                    DecompressBundle(fileArchive[gamePath].bundle);
                    reanalysisNecessary = true;
                    continue;
                }

                if (fileArchive[gamePath].fileSource == FileSource.Mod)
                {
                    BundleFileInstance bfi = am.LoadBundleFile(modFilePaths[fileIdx], false);
                    DecompressBundle(bfi);
                    Merge(fileArchive[gamePath], bfi);
                    reanalysisNecessary = true;
                    continue;
                }

                if (fileArchive[gamePath].fileSource == FileSource.UnrelatedMod)
                {
                    if (fileArchive[gamePath].IsBundle())
                    {
                        BundleFileInstance bfi = am.LoadBundleFile(modFilePaths[fileIdx], false);
                        DecompressBundle(bfi);
                        Merge(fileArchive[gamePath], bfi);
                    }
                    else
                        conflicts.Add((fileIdx, Path.GetFileName(fileArchive[gamePath].fileLocation)));
                    continue;
                }
            }

            if (conflicts.Count == 0)
                return reanalysisNecessary;

            //Resolve file conflicts
            List<int> overwrites = new();
            FileSelectForm fsf = new(conflicts, overwrites);
            fsf.ShowDialog();
            for (int i = 0; i < overwrites.Count; i++)
            {
                int fileIdx = overwrites[i];
                string absolutePath = fbd.SelectedPath;
                string gamePath = modFilePaths[fileIdx].Substring(absolutePath.Length + 1, modFilePaths[fileIdx].Length - absolutePath.Length - 1);
                fileArchive[gamePath].fileLocation = modFilePaths[fileIdx];
            }
            return reanalysisNecessary;
        }

        /// <summary>
        ///  Exports current file archive into local directory.
        /// </summary>
        public void ExportMod()
        {
            string outputDirectory = Environment.CurrentDirectory + "\\" + randomizerModName;
            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, true);
            Directory.CreateDirectory(outputDirectory);
            for (int fileDataIdx = 0; fileDataIdx < fileArchive.Count; fileDataIdx++)
                if (fileArchive.Values.ToList()[fileDataIdx].fileSource != FileSource.Dump)
                    ExportFile(fileArchive.Values.ToList()[fileDataIdx], outputDirectory);
        }

        /// <summary>
        ///  Gets a list of MonoBehaviour value fields by PathEnum.
        /// </summary>
        public List<AssetTypeValueField> GetMonoBehaviours(GlobalData.PathEnum pathEnum)
        {
            BundleFileInstance bfi = fileArchive[GlobalData.randomizerPaths[pathEnum]].bundle;
            AssetsFileInstance afi = am.LoadAssetsFileFromBundle(bfi, 0);

            return afi.table.GetAssetsOfType(114).Select(afie => am.GetTypeInstance(afi, afie).GetBaseField()).ToList();
        }

        /// <summary>
        ///  Overwrites a MonoBehaviour in a bundle.
        /// </summary>
        public void WriteMonoBehaviour(GlobalData.PathEnum pathEnum, AssetTypeValueField monoBehaviour)
        {
            WriteMonoBehaviours(pathEnum, new AssetTypeValueField[] { monoBehaviour });
        }

        /// <summary>
        ///  Overwrites an array of MonoBehaviours in a bundle.
        /// </summary>
        public void WriteMonoBehaviours(GlobalData.PathEnum pathEnum, AssetTypeValueField[] atvfs)
        {
            FileData fd = fileArchive[GlobalData.randomizerPaths[pathEnum]];
            List<AssetsReplacer> ars = new();
            for (int i = 0; i < atvfs.Length; i++)
            {
                BundleFileInstance bfi = fd.bundle;
                AssetsFileInstance afi = am.LoadAssetsFileFromBundle(bfi, bfi.file.bundleInf6.dirInf[0].name);
                AssetTypeValueField atvf = atvfs[i];

                byte[] b = atvf.WriteToByteArray();
                AssetFileInfoEx afie = afi.table.GetAssetInfo(atvf.Get("m_Name").GetValue().AsString(), 114);
                AssetsReplacerFromMemory arfm = new AssetsReplacerFromMemory(0, afie.index, (int)afie.curFileType, AssetHelper.GetScriptIndex(afi.file, afie), b);
                ars.Add(arfm);
            }
            MakeTempBundle(fd, ars, Environment.CurrentDirectory + "\\" + Path.GetFileName(fd.gamePath) + GetFileIndex());
            fd.fileSource = FileSource.Randomizer;
        }

        /// <summary>
        ///  Places a file relative to the mod root in accordance with its FileData.
        /// </summary>
        private void ExportFile(FileData fd, string modRoot)
        {
            Directory.CreateDirectory(modRoot + "\\" + Path.GetDirectoryName(fd.gamePath));
            string newLocation = modRoot + "\\" + fd.gamePath;

            if (!fd.IsBundle())
            {
                File.Copy(fd.fileLocation, newLocation);
                return;
            }

            FileStream stream = File.OpenWrite(newLocation);
            AssetsFileWriter afw = new AssetsFileWriter(stream);
            fd.bundle.file.Pack(fd.bundle.file.reader, afw, AssetBundleCompressionType.LZ4);
            afw.Close();
            fd.bundle.file.Close();
            fd.bundle.stream.Dispose();
            if (fd.tempLocation)
                File.Delete(fd.fileLocation);
        }

        /// <summary>
        ///  Merges two bundles by swapping out select files.
        /// </summary>
        private void Merge(FileData fd, BundleFileInstance bfi2)
        {
            BundleFileInstance bfi1 = fd.bundle;
            AssetsFileInstance afi1 = am.LoadAssetsFileFromBundle(bfi1, bfi1.file.bundleInf6.dirInf[0].name);
            am = new();
            AssetsFileInstance afi2 = am.LoadAssetsFileFromBundle(bfi2, bfi2.file.bundleInf6.dirInf[0].name);
            AssetFileInfoEx[] assetFiles1 = afi1.table.assetFileInfo;
            AssetFileInfoEx[] assetFiles2 = afi2.table.assetFileInfo;

            //Collects conflicts
            List<(int, string)> conflicts = new();
            for (int i = 0; i < assetFiles1.Length; i++)
            {
                AssetTypeValueField atvf1 = am.GetTypeInstance(afi1, assetFiles1[i]).GetBaseField();
                AssetTypeValueField atvf2 = am.GetTypeInstance(afi2, assetFiles2[i]).GetBaseField();
                byte[] b1 = atvf1.WriteToByteArray();
                byte[] b2 = atvf2.WriteToByteArray();

                if (!IsMatch(b1, b2))
                    conflicts.Add((i, atvf1.Get("m_Name").GetValue().AsString()));
            }

            //No conflicts, everyone is happy
            if (conflicts.Count == 0)
                return;

            //Gets what files to overwrite from user
            List<int> overwrites = new();
            FileSelectForm fsf = new(conflicts, overwrites);
            fsf.ShowDialog();

            //Overwrites selected files, and I gotta say, surprisingly complicated.
            List<AssetsReplacer> ars = new();
            for (int i = 0; i < overwrites.Count; i++)
            {
                bfi1 = fd.bundle;
                afi1 = am.LoadAssetsFileFromBundle(bfi1, bfi1.file.bundleInf6.dirInf[0].name);
                AssetTypeValueField atvf = am.GetTypeInstance(afi2, assetFiles2[overwrites[i]]).GetBaseField();
                assetFiles1 = afi1.table.assetFileInfo;

                byte[] b2 = atvf.WriteToByteArray();
                AssetFileInfoEx afie1 = assetFiles1[overwrites[i]];
                AssetsReplacerFromMemory arfm = new AssetsReplacerFromMemory(0, afie1.index, (int)afie1.curFileType, AssetHelper.GetScriptIndex(afi1.file, afie1), b2);
                ars.Add(arfm);
            }

            MakeTempBundle(fd, ars, Environment.CurrentDirectory + "\\" + Path.GetFileName(fd.gamePath) + GetFileIndex());
        }

        /// <summary>
        ///  Swaps out the loaded bundle with a modified bundle
        /// </summary>
        private void MakeTempBundle(FileData fd, List<AssetsReplacer> ars, string fileLocation)
        {
            BundleFileInstance bfi1 = fd.bundle;
            AssetsFileInstance afi1 = am.LoadAssetsFileFromBundle(bfi1, bfi1.file.bundleInf6.dirInf[0].name);
            MemoryStream memoryStream = new MemoryStream();
            AssetsFileWriter afw = new(memoryStream);
            afi1.file.Write(afw, 0, ars, 0);
            BundleReplacerFromMemory brfm = new BundleReplacerFromMemory(bfi1.file.bundleInf6.dirInf[0].name, null, true, memoryStream.ToArray(), -1);

            afw = new(File.OpenWrite(fileLocation));
            fd.bundle.file.Write(afw, new List<BundleReplacer> { brfm });
            afw.Close();
            fd.bundle.file.Close();
            fd.bundle.stream.Dispose();
            fd.bundle = am.LoadBundleFile(fileLocation, false);
            DecompressBundle(fd.bundle);
            if (fd.tempLocation)
                File.Delete(fd.fileLocation);
            fd.fileLocation = fileLocation;
            fd.tempLocation = true;
        }

        private void DecompressBundle(BundleFileInstance bfi)
        {
            AssetBundleFile abf = bfi.file;

            MemoryStream stream = new MemoryStream();
            abf.Unpack(abf.reader, new AssetsFileWriter(stream));

            stream.Position = 0;

            AssetBundleFile newAbf = new AssetBundleFile();
            newAbf.Read(new AssetsFileReader(stream), false);

            abf.reader.Close();
            bfi.file = newAbf;
        }

        /// <summary>
        ///  Checks if two byte arrays are identical.
        /// </summary>
        private static bool IsMatch(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
                return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;
            return true;
        }

        /// <summary>
        ///  Gets the path of the AssetAssistant folder given a game directory.
        /// </summary>
        private string GetAssetAssistantPath(string gameLocation)
        {
            string correctPath = gameLocation + "\\romfs\\Data\\StreamingAssets\\AssetAssistant";
            if (Directory.Exists(correctPath))
                return correctPath;

            //Yuzu outputs a stupid dump with the wrong folder structure.
            string yuzuDumpPath = gameLocation + "\\romfs\\StreamingAssets\\AssetAssistant";
            if (Directory.Exists(yuzuDumpPath))
                return yuzuDumpPath;

            return "";
        }

        /// <summary>
        ///  Checks whether the path contains romfs or exefs folders.
        /// </summary>
        private static bool IsGameDirectory(string path, bool needRomfs = false)
        {
            return Directory.Exists(path + "\\romfs") || Directory.Exists(path + "\\exefs") && !needRomfs;
        }

        /// <summary>
        ///  For generating unique file names.
        /// </summary>
        private int GetFileIndex()
        {
            fileIndex++;
            return fileIndex;
        }
    }
}