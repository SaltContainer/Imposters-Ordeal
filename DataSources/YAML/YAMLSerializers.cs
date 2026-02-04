using SharpYaml.Events;
using SharpYaml.Serialization;
using System;
using System.Globalization;

namespace ImpostersOrdeal
{
    /// <summary>
    /// Custom YAML serializer for Unity file references.
    /// </summary>
    public class YAMLUnityFileSerializer : IYamlSerializable
    {
        public bool Accepts(Type type) => type == typeof(YAMLUnityFile);

        public object ReadYaml(ref ObjectContext ctx)
        {
            ctx.Reader.Allow<MappingStart>();

            var file = new YAMLUnityFile();

            while (ctx.Reader.Peek<MappingEnd>() == null)
            {
                var scalar = ctx.Reader.Allow<Scalar>();
                switch (scalar.Value)
                {
                    case "fileID":
                        if (long.TryParse(ctx.Reader.Allow<Scalar>().Value, out long fileID))
                            file.FileID = fileID;
                        else
                            file.FileID = 0;
                        break;

                    case "guid":
                        if (Guid.TryParse(ctx.Reader.Allow<Scalar>().Value, out Guid guid))
                            file.GUID = guid;
                        else
                            file.GUID = Guid.Empty;
                        break;

                    case "type":
                        if (int.TryParse(ctx.Reader.Allow<Scalar>().Value, out int type))
                            file.Type = type;
                        else
                            file.Type = 0;
                        break;
                }
            }

            ctx.Reader.Allow<MappingEnd>();

            return file;
        }

        public void WriteYaml(ref ObjectContext ctx)
        {
            ctx.Writer.Emit(new MappingStartEventInfo(ctx.Instance, ctx.Instance.GetType()) { Style = ctx.Style });

            var file = ctx.Instance as YAMLUnityFile;

            ctx.Writer.Emit(new Scalar("fileID"));
            ctx.Writer.Emit(new Scalar(file.FileID.ToString(CultureInfo.InvariantCulture)));

            if (file.GUID != Guid.Empty)
            {
                ctx.Writer.Emit(new Scalar("guid"));
                ctx.Writer.Emit(new Scalar(file.GUID.ToString("N")));
            }

            if (file.Type != 0)
            {
                ctx.Writer.Emit(new Scalar("type"));
                ctx.Writer.Emit(new Scalar(file.Type.ToString(CultureInfo.InvariantCulture)));
            }

            ctx.Writer.Emit(new MappingEndEventInfo(ctx.Instance, ctx.Instance.GetType()));
        }
    }

    /// <summary>
    /// Custom YAML serializer for float values with invariant culture.
    /// </summary>
    public class YAMLFloatSerializer : IYamlSerializable
    {
        public bool Accepts(Type type) => type == typeof(float);

        public object ReadYaml(ref ObjectContext ctx)
        {
            if (!float.TryParse(ctx.Reader.Allow<Scalar>().Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                value = 0.0f;

            return value;
        }

        public void WriteYaml(ref ObjectContext ctx)
        {
            ctx.Writer.Emit(new Scalar(((float)ctx.Instance).ToString(CultureInfo.InvariantCulture)));
        }
    }

    /// <summary>
    /// Custom YAML serializer for strings that handles single-quote edge cases.
    /// </summary>
    public class YAMLStringSerializer : IYamlSerializable
    {
        public bool Accepts(Type type) => type == typeof(string);

        public object ReadYaml(ref ObjectContext ctx)
        {
            return ctx.Reader.Allow<Scalar>().Value;
        }

        public void WriteYaml(ref ObjectContext ctx)
        {
            var value = (string)ctx.Instance ?? string.Empty;

            if (value.StartsWith("'"))
            {
                value = value.Replace("'", "''");
                value = "'" + value + "'";
            }

            ctx.Writer.Emit(new Scalar(value));
        }
    }
}
