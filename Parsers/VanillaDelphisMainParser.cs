using System.Collections.Generic;
using System;

namespace ImpostersOrdeal
{
    public class VanillaDelphisMainParser : IParser<DelphisMain>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (DelphisMain)data);

        public IEnumerable<Type> GetRequiredDataSources() => [typeof(DelphisMainBank)];

        public DelphisMain ParseFromSources(FileManager fileManager)
        {
            var data = new DelphisMain();

            var delphisMainBank = fileManager.GetDelphisMainBank();
            data.bankData = new Wwise.WwiseData(delphisMainBank.GetRawData());

            return data;
        }

        public void SaveToSources(FileManager fileManager, DelphisMain data)
        {
            var delphisMainBank = fileManager.GetDelphisMainBank();

            delphisMainBank.SetDataFromBuffer(data.bankData.GetBytes());
        }
    }
}
