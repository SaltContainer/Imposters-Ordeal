using System;

namespace ImpostersOrdeal
{
    [Serializable]
    public class TagData
    {
        public int tagIndex;
        public MessageEnumData.GroupTagID groupID;
        public int tagID;
        public MessageEnumData.TagPatternID tagPatternID;
        public int forceArticle;
        public int tagParameter;
        public string[] tagWordArray;
        public MessageEnumData.ForceGrmID forceGrmID;
    }
}
