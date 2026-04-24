using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class YamlMonoContainer
    {
        [YamlPropertyName("MonoBehaviour")]
        public YamlMonoBehaviour MonoBehaviour { get; set; }
    }
}
