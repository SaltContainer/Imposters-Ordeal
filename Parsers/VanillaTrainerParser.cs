using System.Collections.Generic;
using static ImpostersOrdeal.GameDataTypes;

namespace ImpostersOrdeal
{
    public class VanillaTrainerParser : IParser<List<Trainer>>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (List<Trainer>)data);

        public List<Trainer> ParseFromSources(FileManager fileManager)
        {
            var dpr = fileManager.GetDprMasterdatasBundle();
            var mono = dpr.GetMonoByName(Constants.TRAINERTABLE_NAME);
            return null;
        }

        public void SaveToSources(FileManager fileManager, List<Trainer> data)
        {

        }
    }
}
