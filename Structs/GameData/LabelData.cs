using System;

namespace ImpostersOrdeal
{
    [Serializable]
    public class LabelData
    {
        public int labelIndex;
        public int arrayIndex;
        public string labelName;
        public StyleInfo styleInfo = new StyleInfo();
        public int[] attributeValueArray;
        public TagData[] tagDataArray;
        public WordData[] wordDataArray;
    }
}
