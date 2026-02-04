using System;
using System.ComponentModel;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    /// <summary>
    /// Represents a Unity file reference in YAML format.
    /// </summary>
    public class YAMLUnityFile
    {
        [YamlMember("fileID", 0)]
        public long FileID { get; set; }

        [YamlMember("guid", 1)]
        [DefaultValue(typeof(Guid), "00000000-0000-0000-0000-000000000000")]
        public Guid GUID { get; set; }

        [YamlMember("type", 2)]
        [DefaultValue(0)]
        public int Type { get; set; }
    }
}
