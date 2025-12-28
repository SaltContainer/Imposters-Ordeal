using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public class MessageFileTable
    {
        public Dictionary<MessageEnumData.MsgLangId, MsbtDataCollection> messageFilesByLanguage = new Dictionary<MessageEnumData.MsgLangId, MsbtDataCollection>();
        public Dictionary<DataSource, MsbtDataCollection> messageFilesByDataSource = new Dictionary<DataSource, MsbtDataCollection>();

        /// <summary>
        /// Gets a label from a message file in a specific language by name.
        /// </summary>
        public MsbtData.LabelData GetLabelByName(string fileName, string label, MessageEnumData.MsgLangId lang = MessageEnumData.MsgLangId.USA, bool isKanji = false)
        {
            return messageFilesByLanguage[lang]
                .First(m => m.m_Name == FormatMessageFileNameForLanguage(fileName, lang, isKanji) && m.isKanji == isKanji)?
                .labelDataArray.Find(l => l.labelName == label);
        }

        /// <summary>
        /// Gets a label from a message file in a specific language by index.
        /// </summary>
        public MsbtData.LabelData GetLabelByIndex(string fileName, int index, MessageEnumData.MsgLangId lang = MessageEnumData.MsgLangId.USA, bool isKanji = false)
        {
            return messageFilesByLanguage[lang]
                .First(m => m.m_Name == FormatMessageFileNameForLanguage(fileName, lang, isKanji) && m.isKanji == isKanji)?
                .labelDataArray.Find(l => l.labelIndex == index);
        }

        /// <summary>
        /// Gets the appropriate message file prefix for a specific language.
        /// </summary>
        public string GetMessageFilePrefixForLanguage(MessageEnumData.MsgLangId language = MessageEnumData.MsgLangId.USA, bool isKanji = false)
        {
            return language switch
            {
                MessageEnumData.MsgLangId.JPN => isKanji ? "jpn_kanji" : "jpn",
                MessageEnumData.MsgLangId.USA => "english",
                MessageEnumData.MsgLangId.FRA => "french",
                MessageEnumData.MsgLangId.ITA => "italian",
                MessageEnumData.MsgLangId.DEU => "german",
                MessageEnumData.MsgLangId.ESP => "spanish",
                MessageEnumData.MsgLangId.KOR => "korean",
                MessageEnumData.MsgLangId.SCH => "simp_chinese",
                MessageEnumData.MsgLangId.TCH => "trad_chinese",
                _ => "",
            };
        }

        /// <summary>
        /// Formats the message file name to have the proper language prefix.
        /// </summary>
        public string FormatMessageFileNameForLanguage(string fileName, MessageEnumData.MsgLangId language = MessageEnumData.MsgLangId.USA, bool isKanji = false)
        {
            return string.Join("_", GetMessageFilePrefixForLanguage(language, isKanji), fileName);
        }
    }
}
