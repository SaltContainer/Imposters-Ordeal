using SharpYaml.Serialization;
using System;
using System.ComponentModel;

namespace ImpostersOrdeal
{
    public class UnityFile
    {
        [YamlPropertyName("fileID")]
        [YamlPropertyOrder(0)]
        public long FileID { get; set; }

        [YamlPropertyName("guid")]
        [YamlPropertyOrder(1)]
        [DefaultValue(0)]
        public Guid GUID { get; set; }

        [YamlPropertyName("type")]
        [YamlPropertyOrder(2)]
        [DefaultValue(0)]
        public int Type { get; set; }
    }
}
