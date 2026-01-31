using SmartPoint.AssetAssistant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ImpostersOrdeal
{
    public class VanillaDprBinParser : IParser<DprBin>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (DprBin)data);

        public IEnumerable<Type> GetRequiredDataSources() => [typeof(DprBinABDM)];

        public DprBin ParseFromSources(FileManager fileManager)
        {
            var data = new DprBin();

            var dprBinFile = fileManager.GetDprBinFile();
            data.manifest = AssetBundleDownloadManifest.Load(dprBinFile.GetRawData());

            return data;
        }

        public void SaveToSources(FileManager fileManager, DprBin data)
        {
            var dprBinFile = fileManager.GetDprBinFile();

            // Probably need to find a way to do this without BinaryFormatter eventually since it's deprecated
            var formatter = new BinaryFormatter();
            using var stream = new MemoryStream();
            formatter.Serialize(stream, data.manifest);

            dprBinFile.SetDataFromBuffer(stream.ToArray());
        }
    }
}
