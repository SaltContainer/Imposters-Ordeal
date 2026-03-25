using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public class MsbtData : ScriptableObject
    {
        public int hash;
        public MessageEnumData.MsgLangId langID;
        public bool isResident;
        public bool isKanji;
        public List<LabelData> labelDataArray = new List<LabelData>();

        public class LabelData
        {
            public int labelIndex;
            public int arrayIndex;
            public string labelName;
            public StyleInfo styleInfo = new StyleInfo();
            public List<int> attributeValueArray = new List<int>();
            public List<TagData> tagDataArray = new List<TagData>();
            public List<WordData> wordDataArray = new List<WordData>();

            public class StyleInfo
            {
                public int styleIndex;
                public int colorIndex;
                public int fontSize;
                public int maxWidth;
                public MessageEnumData.MsgControlID controlID;
            }

            public class TagData
            {
                public int tagIndex;
                public MessageEnumData.GroupTagID groupID;
                public int tagID;
                public MessageEnumData.TagPatternID tagPatternID;
                public int forceArticle;
                public int tagParameter;
                public List<string> tagWordArray;
                public MessageEnumData.ForceGrmID forceGrmID;
            }

            public class WordData
            {
                public MessageEnumData.WordDataPatternID patternID;
                public MessageEnumData.MsgEventID eventID;
                public int tagIndex = -1;
                public float tagValue;
                public string str;
                public float strWidth = -1.0f;

                public string GetEndChar()
                {
                    return eventID switch
                    {
                        MessageEnumData.MsgEventID.None => "", //No marker
                        MessageEnumData.MsgEventID.NewLine => "\n", //New line marker
                        MessageEnumData.MsgEventID.Wait => "", //Wait marker
                        MessageEnumData.MsgEventID.ScrollPage => "\n", //New textbox marker
                        MessageEnumData.MsgEventID.ScrollLine => "\n", //Scroll textbox marker
                        MessageEnumData.MsgEventID.CallBack => "", //Start/join event marker?
                        MessageEnumData.MsgEventID.End => "", //End of message
                        _ => "\0", //Unknown
                    };
                }

                public string GetMacro()
                {
                    return eventID switch
                    {
                        MessageEnumData.MsgEventID.None => "\\0", //No marker
                        MessageEnumData.MsgEventID.NewLine => "\\n", //New line marker
                        MessageEnumData.MsgEventID.Wait => "\\w", //Wait marker
                        MessageEnumData.MsgEventID.ScrollPage => "\\r", //New textbox marker
                        MessageEnumData.MsgEventID.ScrollLine => "\\f", //Scroll textbox marker
                        MessageEnumData.MsgEventID.CallBack => "\\e", //Start/join event marker?
                        MessageEnumData.MsgEventID.End => "", //End of message
                        _ => "\\0", //Unknown
                    };
                }
            }

            public string GetString()
            {
                string str = "";
                for (int i = 0; i < wordDataArray.Count; i++)
                    str += wordDataArray[i].str + wordDataArray[i].GetEndChar();
                return str;
            }

            public bool IsValidString()
            {
                if (GetString().Length < 1)
                    return false;
                for (int i = 0; i < wordDataArray.Count; i++)
                    if (wordDataArray[i].tagIndex >= 0 || wordDataArray[i].eventID == MessageEnumData.MsgEventID.CallBack)
                        return false;
                return true;
            }

            public bool IsDialogString()
            {
                for (int i = 0; i < wordDataArray.Count; i++)
                    if (wordDataArray[i].eventID == MessageEnumData.MsgEventID.ScrollPage)
                        return true;
                return false;
            }

            public string GetMacroString()
            {
                string s = "";
                foreach (var wd in wordDataArray)
                    s += wd.str + wd.GetMacro();
                return s;
            }

            // TODO: Improve this?
            public override string ToString()
            {
                return string.Join("\n", wordDataArray.Select(w => w.str));
            }
        }

        public void SetStrings(List<LabelData> strings)
        {
            for (int i = labelDataArray.Count - 1; i >= 0; i--)
                if (labelDataArray[i].IsValidString())
                {
                    labelDataArray[i].wordDataArray = strings[^1].wordDataArray;
                    strings.RemoveAt(strings.Count - 1);
                }
        }
    }
}
