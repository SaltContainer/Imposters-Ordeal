using SharpYaml;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    /// <summary>
    /// Unity Vector2 type for YAML serialization.
    /// </summary>
    public class YAMLUnityVector2
    {
        [YamlMember("x", 0)]
        public float X { get; set; }

        [YamlMember("y", 1)]
        public float Y { get; set; }
    }

    /// <summary>
    /// Unity Vector2Int type for YAML serialization.
    /// </summary>
    public class YAMLUnityVector2Int
    {
        [YamlMember("x", 0)]
        public int X { get; set; }

        [YamlMember("y", 1)]
        public int Y { get; set; }
    }

    /// <summary>
    /// Unity Vector3 type for YAML serialization.
    /// </summary>
    [YamlStyle(YamlStyle.Flow)]
    public class YAMLUnityVector3
    {
        [YamlMember("x", 0)]
        public float X { get; set; }

        [YamlMember("y", 1)]
        public float Y { get; set; }

        [YamlMember("z", 2)]
        public float Z { get; set; }
    }

    /// <summary>
    /// Unity Color type for YAML serialization.
    /// </summary>
    [YamlStyle(YamlStyle.Flow)]
    public class YAMLUnityColor
    {
        [YamlMember("r", 0)]
        public float R { get; set; }

        [YamlMember("g", 1)]
        public float G { get; set; }

        [YamlMember("b", 2)]
        public float B { get; set; }

        [YamlMember("a", 3)]
        public float A { get; set; }
    }

    /// <summary>
    /// Unity Color32 type for YAML serialization.
    /// </summary>
    [YamlStyle(YamlStyle.Flow)]
    public class YAMLUnityColor32
    {
        [YamlMember("r", 0)]
        public byte R { get; set; }

        [YamlMember("g", 1)]
        public byte G { get; set; }

        [YamlMember("b", 2)]
        public byte B { get; set; }

        [YamlMember("a", 3)]
        public byte A { get; set; }

        [YamlMember("rgba", 4)]
        public uint RGBA { get; set; }
    }
}
