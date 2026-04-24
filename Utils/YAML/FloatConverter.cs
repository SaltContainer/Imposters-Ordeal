using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
namespace ImpostersOrdeal
{
    public class FloatConverter : YamlConverter<float>
    {
        public override float Read(YamlReader reader)
        {
            if (!float.TryParse(reader.GetScalarValue(), out float value))
                value = 0.0f;

            return value;
        }

        public override void Write(YamlWriter writer, float value)
        {
            writer.WriteScalar<float>(value);
        }
    }
}
