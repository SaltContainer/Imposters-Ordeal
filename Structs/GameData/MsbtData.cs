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
            }

            // TODO: Improve this?
            public override string ToString()
            {
                return string.Join("\n", wordDataArray.Select(w => w.str));
            }
        }
    }
}
