using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public class YamlVanillaEvDataParser : IParser<EvDataCollection>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (EvDataCollection)data);

        public IEnumerable<Type> GetRequiredDataSources() => [typeof(EventAssetYamlFolder)];

        // Arrays
        private static readonly string ARG_FIELD = "Arg";
        private static readonly string COMMANDS_FIELD = "Commands";
        private static readonly string SCRIPTS_FIELD = "Scripts";
        private static readonly string STRLIST_FIELD = "StrList";

        // Fields
        private static readonly string ARGTYPE_FIELD = "argType";
        private static readonly string DATA_FIELD = "data";
        private static readonly string LABEL_FIELD = "Label";

        public EvDataCollection ParseFromSources(FileManager fileManager)
        {
            var data = new EvDataCollection();

            var eventAssetFolder = fileManager.GetEventAssetYamlFolder();
            var yamls = eventAssetFolder.GetAllYamls();

            foreach (var (name, yaml) in yamls)
            {
                EvData evData = new();
                evData.m_Name = yaml.MonoBehaviour.Name;
                evData.fileName = name;

                // Parse Scripts
                evData.Scripts = new();
                var scriptFields = yaml.MonoBehaviour.Fields.GetSequenceOfMappings(SCRIPTS_FIELD);
                foreach (var scriptField in scriptFields)
                {
                    EvData.Script script = new();
                    script.Label = scriptField[LABEL_FIELD].ToObject<string>();

                    // Parse Commands
                    script.Commands = new();
                    var commandFields = scriptField.GetSequenceOfMappings(COMMANDS_FIELD);
                    foreach (var commandField in commandFields)
                    {
                        EvData.Command command = new();

                        // Parse Arguments
                        command.Arg = new List<EvData.Argument>();
                        var argumentFields = commandField.GetSequenceOfMappings(ARG_FIELD);
                        foreach (var argumentField in argumentFields)
                        {
                            EvData.Argument arg = new();
                            arg.argType = (EvData.ArgType)argumentField[ARGTYPE_FIELD].ToObject<int>();
                            arg.data = argumentField[DATA_FIELD].ToObject<int>();

                            command.Arg.Add(arg);
                        }

                        script.Commands.Add(command);
                    }

                    evData.Scripts.Add(script);
                }

                // Parse StrLists
                evData.StrList = yaml.MonoBehaviour.Fields.GetSequenceOfElements(STRLIST_FIELD).Select(x => x.ToObject<string>()).ToList();

                data.Add(evData);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, EvDataCollection data)
        {
            var eventAssetFolder = fileManager.GetEventAssetYamlFolder();

            foreach (var evData in data)
            {
                // TODO
            }
        }
    }
}
