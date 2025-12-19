using AssetsTools.NET;
using System.Collections.Generic;
using ImpostersOrdeal.Utils;

namespace ImpostersOrdeal
{
    public class VanillaEvDataParser : IParser<EvDataCollection>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (EvDataCollection)data);

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
            var monoBehaviours = evScriptBundle.GetMonosWhere((p, m) => !m[SCRIPTS_FIELD].IsDummy && !m[STRLIST_FIELD].IsDummy);

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
                evData.StrList = new();
                var stringFields = monoBehaviour[STRLIST_FIELD].GetArrayElements();
                foreach (var stringField in stringFields)
                    evData.StrList.Add(stringField.AsString);

                data.Add(evData);
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, EvDataCollection data)
        {
            var evScriptBundle = fileManager.GetEvScriptBundle();
            var monoBehaviours = evScriptBundle.GetMonosWhere((p, m) => !m[SCRIPTS_FIELD].IsDummy && !m[STRLIST_FIELD].IsDummy);

            foreach (var evData in data)
            {
                var mono = evScriptBundle.GetMonoByPathID(evData.pathID);

                // Write Scripts
                List<AssetTypeValueField> newScripts = new();
                foreach (var script in evData.Scripts)
                {
                    AssetTypeValueField scriptField = mono[SCRIPTS_FIELD].CreateArrayElement();

                    scriptField[LABEL_FIELD].AsString = script.Label;

                    // Write Commands
                    List<AssetTypeValueField> newCommands = new();
                    foreach (var command in script.Commands)
                    {
                        AssetTypeValueField commandField = mono[COMMANDS_FIELD].CreateArrayElement();

                        // Write Arguments
                        List<AssetTypeValueField> newArgs = new();
                        foreach (var arg in command.Arg)
                        {
                            AssetTypeValueField argField = mono[ARG_FIELD].CreateArrayElement();

                            argField[ARGTYPE_FIELD].AsInt = (int)arg.argType;
                            argField[DATA_FIELD].AsInt = arg.data;

                            newArgs.Add(argField);
                        }
                        commandField[ARG_FIELD].SetArrayElements(newArgs);
                    }
                    scriptField[COMMANDS_FIELD].SetArrayElements(newCommands);
                }
                mono[SCRIPTS_FIELD].SetArrayElements(newScripts);

                // Write StrLists
                List<AssetTypeValueField> newStrs = new();
                foreach (var str in evData.StrList)
                {
                    AssetTypeValueField strField = mono[STRLIST_FIELD].CreateArrayElement();

                    strField.AsString = str;

                    newStrs.Add(strField);
                }
                mono[STRLIST_FIELD].SetArrayElements(newStrs);

                evScriptBundle.SetMonoByPathID(evData.pathID, mono);
            }
        }
    }
}
