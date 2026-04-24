using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class StringConverter : YamlConverter<string>
    {
        public override string Read(YamlReader reader)
        {
            var value = reader.GetScalarValue();
            reader.Read();
            return value;
        }

        public override void Write(YamlWriter writer, string value)
        {
            if (value.StartsWith("\'"))
            {
                value = value.Replace("\'", "\'\'");
                value = "\'" + value + "\'";
            }

            writer.WriteScalar(value);
        }
    }
}
