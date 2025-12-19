using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class EvData : ScriptableObject
    {
        public List<Script> Scripts = new List<Script>();
        public List<string> StrList = new List<string>();

        public enum ArgType : int
        {
            Command = 0,
            Float = 1,
            Work = 2,
            Flag = 3,
            SysFlag = 4,
            String = 5,
        }

        public class Argument
        {
            public ArgType argType;
            public int data;
        }

        public class Script
        {
            public string Label = "";
            public List<Command> Commands = new List<Command>();
        }

        public class Command
        {
            public List<Argument> Arg = new List<Argument>();
        }
    }
}
