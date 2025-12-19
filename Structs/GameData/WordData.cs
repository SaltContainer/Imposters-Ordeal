using System;

namespace ImpostersOrdeal
{
    [Serializable]
    public class WordData
    {
        public MessageEnumData.WordDataPatternID patternID;
        public MessageEnumData.MsgEventID eventID;
        public int tagIndex = -1;
        public float tagValue;
        public string str;
        public float strWidth = -1.0f;
    }
}
