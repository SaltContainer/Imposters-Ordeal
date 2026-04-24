using SharpYaml.Model;
using SharpYaml.Serialization;
using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class YamlMonoBehaviour
    {
        [YamlPropertyName("m_ObjectHideFlags")]
        [YamlPropertyOrder(0)]
        public int ObjectHideFlags { get; set; }

        [YamlPropertyName("m_CorrespondingSourceObject")]
        [YamlPropertyOrder(1)]
        //[YamlStyle(YamlStyle.Flow)]
        public UnityFile CorrespondingSourceObject { get; set; }

        [YamlPropertyName("m_PrefabInstance")]
        [YamlPropertyOrder(2)]
        //[YamlStyle(YamlStyle.Flow)]
        public UnityFile PrefabInstance { get; set; }

        [YamlPropertyName("m_PrefabAsset")]
        [YamlPropertyOrder(3)]
        //[YamlStyle(YamlStyle.Flow)]
        public UnityFile PrefabAsset { get; set; }

        [YamlPropertyName("m_GameObject")]
        [YamlPropertyOrder(4)]
        //[YamlStyle(YamlStyle.Flow)]
        public UnityFile GameObject { get; set; }

        [YamlPropertyName("m_Enabled")]
        [YamlPropertyOrder(5)]
        public int Enabled { get; set; }

        [YamlPropertyName("m_EditorHideFlags")]
        [YamlPropertyOrder(6)]
        public int EditorHideFlags { get; set; }

        [YamlPropertyName("m_Script")]
        [YamlPropertyOrder(7)]
        //[YamlStyle(YamlStyle.Flow)]
        public UnityFile Script { get; set; }

        [YamlPropertyName("m_Name")]
        [YamlPropertyOrder(8)]
        public string Name { get; set; }

        [YamlPropertyName("m_EditorClassIdentifier")]
        [YamlPropertyOrder(9)]
        public string EditorClassIdentifier { get; set; }

        [YamlExtensionData]
        public YamlMapping Fields { get; set; }
    }
}
