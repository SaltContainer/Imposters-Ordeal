using AssetsTools.NET;
using ImpostersOrdeal.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public class VanillaMessageFileParser : IParser<MessageFileTable>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (MessageFileTable)data);

        public IEnumerable<Type> GetRequiredDataSources() => [
            typeof(CommonMsbtBundle),
            typeof(JapaneseMessageBundle),
            typeof(JapaneseKanjiMessageBundle),
            typeof(EnglishMessageBundle),
            typeof(FrenchMessageBundle),
            typeof(ItalianMessageBundle),
            typeof(GermanMessageBundle),
            typeof(SpanishMessageBundle),
            typeof(KoreanMessageBundle),
            typeof(SimplifiedChineseMessageBundle),
            typeof(TraditionalChineseMessageBundle),
        ];

        // Arrays
        private static readonly string ATTRIBUTEVALUEARRAY_FIELD = "attributeValueArray";
        private static readonly string LABELDATAARRAY_FIELD = "labelDataArray";
        private static readonly string TAGDATAARRAY_FIELD = "tagDataArray";
        private static readonly string TAGWORDARRAY_FIELD = "tagWordArray";
        private static readonly string WORDDATAARRAY_FIELD = "wordDataArray";

        // Fields
        private static readonly string ARRAYINDEX_FIELD = "arrayIndex";
        private static readonly string COLORINDEX_FIELD = "colorIndex";
        private static readonly string CONTROLID_FIELD = "controlID";
        private static readonly string EVENTID_FIELD = "eventID";
        private static readonly string FONTSIZE_FIELD = "fontSize";
        private static readonly string FORCEARTICLE_FIELD = "forceArticle";
        private static readonly string FORCEGRMID_FIELD = "forceGrmID";
        private static readonly string GROUPID_FIELD = "groupID";
        private static readonly string HASH_FIELD = "hash";
        private static readonly string ISKANJI_FIELD = "isKanji";
        private static readonly string ISRESIDENT_FIELD = "isResident";
        private static readonly string LABELINDEX_FIELD = "labelIndex";
        private static readonly string LABELNAME_FIELD = "labelName";
        private static readonly string LANGID_FIELD = "langID";
        private static readonly string MNAME_FIELD = "m_Name";
        private static readonly string MAXWIDTH_FIELD = "maxWidth";
        private static readonly string PATTERNID_FIELD = "patternID";
        private static readonly string STR_FIELD = "str";
        private static readonly string STRWIDTH_FIELD = "strWidth";
        private static readonly string STYLEINDEX_FIELD = "styleIndex";
        private static readonly string STYLEINFO_FIELD = "styleInfo";
        private static readonly string TAGID_FIELD = "tagID";
        private static readonly string TAGINDEX_FIELD = "tagIndex";
        private static readonly string TAGPARAMETER_FIELD = "tagParameter";
        private static readonly string TAGPATTERNID_FIELD = "tagPatternID";
        private static readonly string TAGVALUE_FIELD = "tagValue";

        public MessageFileTable ParseFromSources(FileManager fileManager)
        {
            var data = new MessageFileTable();

            var sources = new List<Bundle>()
            {
                fileManager.GetCommonMsbtBundle(),
                fileManager.GetJapaneseMessageBundle(),
                fileManager.GetJapaneseKanjiMessageBundle(),
                fileManager.GetEnglishMessageBundle(),
                fileManager.GetFrenchMessageBundle(),
                fileManager.GetItalianMessageBundle(),
                fileManager.GetGermanMessageBundle(),
                fileManager.GetSpanishMessageBundle(),
                fileManager.GetKoreanMessageBundle(),
                fileManager.GetSimplifiedChineseMessageBundle(),
                fileManager.GetTraditionalChineseMessageBundle(),
            };

            data.messageFilesByLanguage = new();
            data.messageFilesByDataSource = new();
            foreach (var source in sources)
            {
                var monos = source.GetAllMonos();

                var messageFiles = new MsbtDataCollection();
                foreach (var (pathID, mono) in monos)
                {
                    var messageFile = new MsbtData();

                    messageFile.pathID = pathID;
                    messageFile.m_Name = mono[MNAME_FIELD].AsString;

                    messageFile.hash = mono[HASH_FIELD].AsInt;
                    messageFile.langID = (MessageEnumData.MsgLangId)mono[LANGID_FIELD].AsInt;
                    messageFile.isResident = mono[ISRESIDENT_FIELD].AsBool;
                    messageFile.isKanji = mono[ISKANJI_FIELD].AsBool;

                    messageFile.labelDataArray = new();
                    var labelDataFields = mono[LABELDATAARRAY_FIELD].GetArrayElements();
                    foreach (var labelDataField in labelDataFields)
                    {
                        var labelData = new MsbtData.LabelData();

                        labelData.labelIndex = labelDataField[LABELINDEX_FIELD].AsInt;
                        labelData.arrayIndex = labelDataField[ARRAYINDEX_FIELD].AsInt;
                        labelData.labelName = labelDataField[LABELNAME_FIELD].AsString;
                        labelData.styleInfo = ParseStyleInfo(labelDataField[STYLEINFO_FIELD]);
                        labelData.attributeValueArray = labelDataField[ATTRIBUTEVALUEARRAY_FIELD].GetArrayElements().Select(f => f.AsInt).ToList();

                        labelData.tagDataArray = new();
                        var tagDataFields = labelDataField[TAGDATAARRAY_FIELD].GetArrayElements();
                        foreach (var tagDataField in tagDataFields)
                        {
                            var tagData = new MsbtData.LabelData.TagData();

                            tagData.tagIndex = tagDataField[TAGINDEX_FIELD].AsInt;
                            tagData.groupID = (MessageEnumData.GroupTagID)tagDataField[GROUPID_FIELD].AsInt;
                            tagData.tagID = tagDataField[TAGID_FIELD].AsInt;
                            tagData.tagPatternID = (MessageEnumData.TagPatternID)tagDataField[TAGPATTERNID_FIELD].AsInt;
                            tagData.forceArticle = tagDataField[FORCEARTICLE_FIELD].AsInt;
                            tagData.tagParameter = tagDataField[TAGPARAMETER_FIELD].AsInt;
                            tagData.tagWordArray = tagDataField[TAGWORDARRAY_FIELD].GetArrayElements().Select(f => f.AsString).ToList();
                            tagData.forceGrmID = (MessageEnumData.ForceGrmID)tagDataField[FORCEGRMID_FIELD].AsInt;

                            labelData.tagDataArray.Add(tagData);
                        }

                        labelData.wordDataArray = new();
                        var wordDataFields = labelDataField[WORDDATAARRAY_FIELD].GetArrayElements();
                        foreach (var wordDataField in wordDataFields)
                        {
                            var wordData = new MsbtData.LabelData.WordData();

                            wordData.patternID = (MessageEnumData.WordDataPatternID)wordDataField[PATTERNID_FIELD].AsInt;
                            wordData.eventID = (MessageEnumData.MsgEventID)wordDataField[EVENTID_FIELD].AsInt;
                            wordData.tagIndex = wordDataField[TAGINDEX_FIELD].AsInt;
                            wordData.tagValue = wordDataField[TAGVALUE_FIELD].AsFloat;
                            wordData.str = wordDataField[STR_FIELD].AsString;
                            wordData.strWidth = wordDataField[STRWIDTH_FIELD].AsFloat;

                            labelData.wordDataArray.Add(wordData);
                        }

                        messageFile.labelDataArray.Add(labelData);
                    }

                    if (!data.messageFilesByLanguage.ContainsKey(messageFile.langID))
                        data.messageFilesByLanguage[messageFile.langID] = new MsbtDataCollection();

                    data.messageFilesByLanguage[messageFile.langID].Add(messageFile);

                    messageFiles.Add(messageFile);
                }

                data.messageFilesByDataSource[source] = messageFiles;
            }

            return data;
        }

        public void SaveToSources(FileManager fileManager, MessageFileTable data)
        {
            foreach (var (dataSource, messageFiles) in data.messageFilesByDataSource)
            {
                var bundle = dataSource as Bundle;

                foreach (var messageFile in messageFiles)
                {
                    var mono = bundle.GetMonoByPathID(messageFile.pathID);

                    mono[HASH_FIELD].AsInt = messageFile.hash;
                    mono[LANGID_FIELD].AsInt = (int)messageFile.langID;
                    mono[ISRESIDENT_FIELD].AsBool = messageFile.isResident;
                    mono[ISKANJI_FIELD].AsBool = messageFile.isKanji;

                    mono[LABELDATAARRAY_FIELD].SetArrayElementsAndInit(messageFile.labelDataArray, (labelDataField, labelData) =>
                    {
                        labelDataField[LABELINDEX_FIELD].AsInt = labelData.labelIndex;
                        labelDataField[ARRAYINDEX_FIELD].AsInt = labelData.arrayIndex;
                        labelDataField[LABELNAME_FIELD].AsString = labelData.labelName;
                        SaveStyleInfo(labelDataField[STYLEINFO_FIELD], labelData.styleInfo);
                        labelDataField[ATTRIBUTEVALUEARRAY_FIELD].SetArrayElementsAndInit(labelData.attributeValueArray, (f, v) => f.AsInt = v);

                        labelDataField[TAGDATAARRAY_FIELD].SetArrayElementsAndInit(labelData.tagDataArray, (tagDataField, tagData) =>
                        {
                            tagDataField[TAGINDEX_FIELD].AsInt = tagData.tagIndex;
                            tagDataField[GROUPID_FIELD].AsInt = (int)tagData.groupID;
                            tagDataField[TAGID_FIELD].AsInt = tagData.tagID;
                            tagDataField[TAGPATTERNID_FIELD].AsInt = (int)tagData.tagPatternID;
                            tagDataField[FORCEARTICLE_FIELD].AsInt = tagData.forceArticle;
                            tagDataField[TAGPARAMETER_FIELD].AsInt = tagData.tagParameter;
                            tagDataField[TAGWORDARRAY_FIELD].SetArrayElementsAndInit(tagData.tagWordArray, (f, v) => f.AsString = v);
                            tagDataField[FORCEGRMID_FIELD].AsInt = (int)tagData.forceGrmID;
                        });

                        labelDataField[WORDDATAARRAY_FIELD].SetArrayElementsAndInit(labelData.wordDataArray, (wordDataField, wordData) =>
                        {
                            wordDataField[PATTERNID_FIELD].AsInt = (int)wordData.patternID;
                            wordDataField[EVENTID_FIELD].AsInt = (int)wordData.eventID;
                            wordDataField[TAGINDEX_FIELD].AsInt = wordData.tagIndex;
                            wordDataField[TAGVALUE_FIELD].AsFloat = wordData.tagValue;
                            wordDataField[STR_FIELD].AsString = wordData.str;
                            wordDataField[STRWIDTH_FIELD].AsFloat = wordData.strWidth;
                        });
                    });

                    bundle.SetMonoByPathID(messageFile.pathID, mono);
                }
            }
        }

        private MsbtData.LabelData.StyleInfo ParseStyleInfo(AssetTypeValueField field)
        {
            return new MsbtData.LabelData.StyleInfo()
            {
                styleIndex = field[STYLEINDEX_FIELD].AsInt,
                colorIndex = field[COLORINDEX_FIELD].AsInt,
                fontSize = field[FONTSIZE_FIELD].AsInt,
                maxWidth = field[MAXWIDTH_FIELD].AsInt,
                controlID = (MessageEnumData.MsgControlID)field[CONTROLID_FIELD].AsInt,
            };
        }

        private void SaveStyleInfo(AssetTypeValueField field, MsbtData.LabelData.StyleInfo data)
        {
            field[STYLEINDEX_FIELD].AsInt = data.styleIndex;
            field[COLORINDEX_FIELD].AsInt = data.colorIndex;
            field[FONTSIZE_FIELD].AsInt = data.fontSize;
            field[MAXWIDTH_FIELD].AsInt = data.maxWidth;
            field[CONTROLID_FIELD].AsInt = (int)data.controlID;
        }
    }
}
