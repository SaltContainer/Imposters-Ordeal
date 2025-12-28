using System.Collections.Generic;
using System.Linq;
using ImpostersOrdeal.Utils;

namespace ImpostersOrdeal
{
    public class VanillaEvDataParser : IParser<EvDataCollection>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (EvDataCollection)data);

        // Monos
        private static readonly string EVDATA_MONOSCRIPTNAME = "EvData";

        // Arrays
        private static readonly string ARG_FIELD = "Arg";
        private static readonly string COMMANDS_FIELD = "Commands";
        private static readonly string SCRIPTS_FIELD = "Scripts";
        private static readonly string STRLIST_FIELD = "StrList";

        // Fields
        private static readonly string ARGTYPE_FIELD = "argType";
        private static readonly string DATA_FIELD = "data";
        private static readonly string LABEL_FIELD = "Label";
        private static readonly string MNAME_FIELD = "m_Name";

        public EvDataCollection ParseFromSources(FileManager fileManager)
        {
            var data = new EvDataCollection();

            var evScriptBundle = fileManager.GetEvScriptBundle();
            var monoBehaviours = evScriptBundle.GetMonosByScriptName(EVDATA_MONOSCRIPTNAME);

            foreach (var (pathId, monoBehaviour) in monoBehaviours)
            {
                EvData evData = new();
                evData.pathID = pathId;
                evData.m_Name = monoBehaviour[MNAME_FIELD].AsString;

                // Parse Scripts
                evData.Scripts = new();
                var scriptFields = monoBehaviour[SCRIPTS_FIELD].GetArrayElements();
                foreach (var scriptField in scriptFields)
                {
                    EvData.Script script = new();
                    script.Label = scriptField[LABEL_FIELD].AsString;

                    // Parse Commands
                    script.Commands = new();
                    var commandFields = scriptField[COMMANDS_FIELD].GetArrayElements();
                    foreach (var commandField in commandFields)
                    {
                        EvData.Command command = new();

                        // Parse Arguments
                        command.Arg = new List<EvData.Argument>();
                        var argumentFields = commandField[ARG_FIELD].GetArrayElements();
                        foreach (var argumentField in argumentFields)
                        {
                            EvData.Argument arg = new();
                            arg.argType = (EvData.ArgType)argumentField[ARGTYPE_FIELD].AsInt;
                            arg.data = argumentField[DATA_FIELD].AsInt;

                            command.Arg.Add(arg);
                        }

                        script.Commands.Add(command);
                    }

                    evData.Scripts.Add(script);
                }

                // Parse StrLists
                evData.StrList = monoBehaviour[STRLIST_FIELD].GetArrayElements().Select(f => f.AsString).ToList();

                data.Add(evData);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, EvDataCollection data)
        {
            var evScriptBundle = fileManager.GetEvScriptBundle();
            var monoBehaviours = evScriptBundle.GetMonosByScriptName(EVDATA_MONOSCRIPTNAME);

            foreach (var evData in data)
            {
                var mono = evScriptBundle.GetMonoByPathID(evData.pathID);

                mono[SCRIPTS_FIELD].SetArrayElementsAndInit(evData.Scripts, (scriptField, script) =>
                {
                    scriptField[LABEL_FIELD].AsString = script.Label;
                    scriptField[SCRIPTS_FIELD].SetArrayElementsAndInit(script.Commands, (commandField, command) =>
                    {
                        commandField[ARG_FIELD].SetArrayElementsAndInit(command.Arg, (argField, arg) =>
                        {
                            argField[ARGTYPE_FIELD].AsInt = (int)arg.argType;
                            argField[DATA_FIELD].AsInt = arg.data;
                        });
                    });
                });

                mono[STRLIST_FIELD].SetArrayElementsAndInit(evData.StrList, (f, v) => f.AsString = v);

                evScriptBundle.SetMonoByPathID(evData.pathID, mono);
            }
        }
    }
}
