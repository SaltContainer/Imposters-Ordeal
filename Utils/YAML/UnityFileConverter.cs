using System;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class UnityFileConverter : YamlConverter<UnityFile>
    {
        public override UnityFile Read(YamlReader reader)
        {
            if (reader.TokenType == YamlTokenType.StartMapping)
                reader.Read();

            var file = new UnityFile();

            while (reader.TokenType != YamlTokenType.EndMapping)
            {
                var scalar = reader.GetScalarValue();
                reader.Read();

                switch (scalar)
                {
                    case "fileID":
                        if (int.TryParse(reader.GetScalarValue(), out int fileID))
                            file.FileID = fileID;
                        else
                            file.FileID = 0;
                        break;

                    case "guid":
                        if (Guid.TryParse(reader.GetScalarValue(), out Guid guid))
                            file.GUID = guid;
                        else
                            file.GUID = Guid.Empty;
                        break;

                    case "type":
                        if (int.TryParse(reader.GetScalarValue(), out int type))
                            file.Type = type;
                        else
                            file.Type = 0;
                        break;
                }

                reader.Read();
            }

            if (reader.TokenType == YamlTokenType.EndMapping)
                reader.Read();

            return file;
        }

        public override void Write(YamlWriter writer, UnityFile value)
        {
            writer.WriteStartMapping();

            writer.WriteScalar("fileID");
            writer.WriteScalar<long>(value.FileID);

            if (value.GUID != Guid.Empty)
            {
                writer.WriteScalar("guid");
                writer.WriteScalar(value.GUID.ToString("N"));
            }

            if (value.Type != 0)
            {
                writer.WriteScalar("type");
                writer.WriteScalar<int>(value.Type);
            }

            writer.WriteEndMapping();
        }
    }
}
