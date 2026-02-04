using SharpYaml;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    /// <summary>
    /// Base class for YAML-serialized Unity MonoBehaviour data.
    /// </summary>
    public abstract class YAMLMonoBehaviour
    {
        [YamlMember("m_ObjectHideFlags", 0)]
        public int ObjectHideFlags { get; set; }

        [YamlMember("m_CorrespondingSourceObject", 1)]
        [YamlStyle(YamlStyle.Flow)]
        public YAMLUnityFile CorrespondingSourceObject { get; set; }

        [YamlMember("m_PrefabInstance", 2)]
        [YamlStyle(YamlStyle.Flow)]
        public YAMLUnityFile PrefabInstance { get; set; }

        [YamlMember("m_PrefabAsset", 3)]
        [YamlStyle(YamlStyle.Flow)]
        public YAMLUnityFile PrefabAsset { get; set; }

        [YamlMember("m_GameObject", 4)]
        [YamlStyle(YamlStyle.Flow)]
        public YAMLUnityFile GameObject { get; set; }

        [YamlMember("m_Enabled", 5)]
        public int Enabled { get; set; }

        [YamlMember("m_EditorHideFlags", 6)]
        public int EditorHideFlags { get; set; }

        [YamlMember("m_Script", 7)]
        [YamlStyle(YamlStyle.Flow)]
        public YAMLUnityFile Script { get; set; }

        [YamlMember("m_Name", 8)]
        public string Name { get; set; }

        [YamlMember("m_EditorClassIdentifier", 9)]
        public string EditorClassIdentifier { get; set; }
    }

    /// <summary>
    /// Container for deserializing YAML files with MonoBehaviour root.
    /// </summary>
    public class YAMLMonoContainer
    {
        [YamlMember("MonoBehaviour")]
        public YAMLMonoBehaviour MonoBehaviour { get; set; }
    }
}
