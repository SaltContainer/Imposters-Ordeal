using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public class GlobalMetadata
    {
        public byte[] buffer;

        // Readonly
        public uint stringOffset;
        public uint defaultValuePtrOffset;
        public uint defaultValuePtrSecSize;
        public uint defaultValueOffset;
        public uint defaultValueSecSize;
        public uint fieldOffset;
        public uint typeOffset;
        public uint imageOffset;
        public uint imageSecSize;

        public Dictionary<uint, FieldDefaultValue> defaultValueDic;
        public List<ImageDefinition> images;

        public long[] typeMatchupOffsets;

        public byte GetTypeMatchup(int off, int def)
        {
            return buffer[typeMatchupOffsets[off] + def];
        }

        public void SetTypeMatchup(int off, int def, byte aff)
        {
            buffer[typeMatchupOffsets[off] + def] = aff;
        }

        public class ImageDefinition : IGMObject
        {
            public string name;
            public uint typeStart;
            public uint typeCount;
            public List<TypeDefinition> types;

            public override string ToString()
            {
                return name;
            }

            public bool HasDefault()
            {
                return types.Any(t => t.HasDefault());
            }
        }

        public class TypeDefinition : IGMObject
        {
            public string name;
            public int fieldStart;
            public ushort fieldCount;
            public List<FieldDefinition> fields;

            public override string ToString()
            {
                return name;
            }

            public bool HasDefault()
            {
                return fields.Any(f => f.HasDefault());
            }
        }

        public class FieldDefinition : IGMObject
        {
            public string name;
            public FieldDefaultValue defautValue;

            public override string ToString()
            {
                return name;
            }

            public bool HasDefault()
            {
                return defautValue != null;
            }
        }

        public class FieldDefaultValue
        {
            public long offset;
            public int length;
        }

        public interface IGMObject
        {
            public bool HasDefault();
            public string ToString();
        }
    }
}
