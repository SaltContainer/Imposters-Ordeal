using System.Linq;
using System;
using System.Text;
using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public class VanillaGlobalMetadataParser : IParser<GlobalMetadata>
    {
        object IParser.ParseFromSources(FileManager fileManager) => ParseFromSources(fileManager);
        void IParser.SaveToSources(FileManager fileManager, object data) => SaveToSources(fileManager, (GlobalMetadata)data);

        // Array Names
        private static readonly string TYPEAFF_NORMAL_ARRAYNAME =    "A3758C06C7FB42A47D220A11FBA532C6E8C62A77";
        private static readonly string TYPEAFF_FIGHTING_ARRAYNAME =  "4B289ECFF3C0F0970CFBB23E3106E05803CB0010";
        private static readonly string TYPEAFF_FLYING_ARRAYNAME =    "B9D3FD531E1A63CC167C4B98C0EC93F0249D9944";
        private static readonly string TYPEAFF_POISON_ARRAYNAME =    "347E5A9763B5C5AD3094AEC4B91A98983001E87D";
        private static readonly string TYPEAFF_GROUND_ARRAYNAME =    "C089A0863406C198B5654996536BAC473C816234";
        private static readonly string TYPEAFF_ROCK_ARRAYNAME =      "BCEEC8610D8506C3EDAC1C28CED532E5E2D8AD32";
        private static readonly string TYPEAFF_BUG_ARRAYNAME =       "A6F987666C679A4472D8CD64F600B501D2241486";
        private static readonly string TYPEAFF_GHOST_ARRAYNAME =     "ACBC28AD33161A13959E63783CBFC94EB7FB2D90";
        private static readonly string TYPEAFF_STEEL_ARRAYNAME =     "0459498E9764395D87F7F43BE89CCE657C669BFC";
        private static readonly string TYPEAFF_FIRE_ARRAYNAME =      "C4215116A59F8DBC29910FA47BFBC6A82702816F";
        private static readonly string TYPEAFF_WATER_ARRAYNAME =     "AEDBD0B97A96E5BDD926058406DB246904438044";
        private static readonly string TYPEAFF_GRASS_ARRAYNAME =     "DF2387E4B816070AE396698F2BD7359657EADE81";
        private static readonly string TYPEAFF_ELECTRIC_ARRAYNAME =  "64FFED43123BBC9517F387412947F1C700527EB4";
        private static readonly string TYPEAFF_PSYCHIC_ARRAYNAME =   "B5D988D1CB442CF60C021541BF2DC2A008819FD4";
        private static readonly string TYPEAFF_ICE_ARRAYNAME =       "D64329EA3A838F1B4186746A734070A5DFDA4983";
        private static readonly string TYPEAFF_DRAGON_ARRAYNAME =    "37DF3221C4030AC4E0EB9DD64616D020BB628CC1";
        private static readonly string TYPEAFF_DARK_ARRAYNAME =      "B2DD1970DDE852F750899708154090300541F4DE";
        private static readonly string TYPEAFF_FAIRY_ARRAYNAME =     "F774719D6A36449B152496136177E900605C9778";

        private static readonly string ASSEMBlYCSHARP_NAME = "Assembly-CSharp.dll";
        private static readonly string PRIVATEIMPLEMENTATION_NAME = "<PrivateImplementationDetails>";

        private static readonly string SCOPE_SEPARATOR = ".";

        public GlobalMetadata ParseFromSources(FileManager fileManager)
        {
            var data = new GlobalMetadata();

            var delphisMainBank = fileManager.GetDelphisMainBank();

            byte[] buffer = fileManager.GetGlobalMetadataBuffer();
            data.buffer = buffer;

            data.stringOffset = BitConverter.ToUInt32(buffer, 0x18);

            data.defaultValuePtrOffset = BitConverter.ToUInt32(buffer, 0x40);
            data.defaultValuePtrSecSize = BitConverter.ToUInt32(buffer, 0x44);
            uint defaultValuePtrSize = 0xC;
            uint defaultValuePtrCount = data.defaultValuePtrSecSize / defaultValuePtrSize;

            data.defaultValueOffset = BitConverter.ToUInt32(buffer, 0x48);
            data.defaultValueSecSize = BitConverter.ToUInt32(buffer, 0x4C);

            data.fieldOffset = BitConverter.ToUInt32(buffer, 0x60);
            uint fieldSize = 0xC;

            data.typeOffset = BitConverter.ToUInt32(buffer, 0xA0);
            uint typeSize = 0x5C;

            data.imageOffset = BitConverter.ToUInt32(buffer, 0xA8);
            data.imageSecSize = BitConverter.ToUInt32(buffer, 0xAC);
            uint imageSize = 0x28;
            uint imageCount = data.imageSecSize / imageSize;

            data.defaultValueDic = new();
            uint defaultValuePtrOffset = data.defaultValuePtrOffset;
            for (int defaultValuePtrIdx = 0; defaultValuePtrIdx < defaultValuePtrCount; defaultValuePtrIdx++)
            {
                GlobalMetadata.FieldDefaultValue fdv = new();
                fdv.offset = data.defaultValueOffset + BitConverter.ToUInt32(buffer, (int)defaultValuePtrOffset + 8);
                long nextOffset = data.defaultValueOffset + data.defaultValueSecSize;
                if (defaultValuePtrIdx < defaultValuePtrCount - 1)
                    nextOffset = data.defaultValueOffset + BitConverter.ToUInt32(buffer, (int)defaultValuePtrOffset + 20);
                fdv.length = (int)(nextOffset - fdv.offset);
                uint fieldIdx = BitConverter.ToUInt32(buffer, (int)defaultValuePtrOffset + 0);

                data.defaultValueDic[fieldIdx] = fdv;
                defaultValuePtrOffset += defaultValuePtrSize;
            }

            data.images = new();
            uint imageOffset = data.imageOffset;
            for (int imageIdx = 0; imageIdx < imageCount; imageIdx++)
            {
                GlobalMetadata.ImageDefinition id = new();
                uint imageNameIdx = BitConverter.ToUInt32(buffer, (int)imageOffset + 0);
                id.name = ReadNullTerminatedString(buffer, data.stringOffset + imageNameIdx);
                id.typeStart = BitConverter.ToUInt32(buffer, (int)imageOffset + 8);
                id.typeCount = BitConverter.ToUInt32(buffer, (int)imageOffset + 12);

                id.types = new();
                uint typeOffset = data.typeOffset + id.typeStart * typeSize;
                for (uint typeIdx = id.typeStart; typeIdx < id.typeStart + id.typeCount; typeIdx++)
                {
                    GlobalMetadata.TypeDefinition td = new();
                    uint typeNameIdx = BitConverter.ToUInt32(buffer, (int)typeOffset + 0);
                    uint namespaceNameIdx = BitConverter.ToUInt32(buffer, (int)typeOffset + 4);
                    td.name = ReadNullTerminatedString(buffer, data.stringOffset + namespaceNameIdx);
                    td.name += td.name.Length > 0 ? SCOPE_SEPARATOR : string.Empty;
                    td.name += ReadNullTerminatedString(buffer, data.stringOffset + typeNameIdx);
                    td.fieldStart = BitConverter.ToInt32(buffer, (int)typeOffset + 36);
                    td.fieldCount = BitConverter.ToUInt16(buffer, (int)typeOffset + 72);

                    td.fields = new();
                    uint fieldOffset = (uint)(data.fieldOffset + td.fieldStart * fieldSize);
                    for (uint fieldIdx = (uint)td.fieldStart; fieldIdx < td.fieldStart + td.fieldCount; fieldIdx++)
                    {
                        GlobalMetadata.FieldDefinition fd = new();
                        uint fieldNameIdx = BitConverter.ToUInt32(buffer, (int)fieldOffset + 0);
                        fd.name = ReadNullTerminatedString(buffer, data.stringOffset + fieldNameIdx);
                        if (data.defaultValueDic.TryGetValue(fieldIdx, out GlobalMetadata.FieldDefaultValue fdv))
                            fd.defautValue = fdv;

                        td.fields.Add(fd);
                        fieldOffset += fieldSize;
                    }

                    id.types.Add(td);
                    typeOffset += typeSize;
                }

                data.images.Add(id);
                imageOffset += imageSize;
            }

            GlobalMetadata.TypeDefinition privateImplementationDetails = data.images
                .Where(i => i.name == ASSEMBlYCSHARP_NAME).SelectMany(i => i.types)
                .First(t => t.name == PRIVATEIMPLEMENTATION_NAME);

            data.typeMatchupOffsets = new List<string>()
            {
                TYPEAFF_NORMAL_ARRAYNAME,   TYPEAFF_FIGHTING_ARRAYNAME, TYPEAFF_FLYING_ARRAYNAME,
                TYPEAFF_POISON_ARRAYNAME,   TYPEAFF_GROUND_ARRAYNAME,   TYPEAFF_ROCK_ARRAYNAME,
                TYPEAFF_BUG_ARRAYNAME,      TYPEAFF_GHOST_ARRAYNAME,    TYPEAFF_STEEL_ARRAYNAME,
                TYPEAFF_FIRE_ARRAYNAME,     TYPEAFF_WATER_ARRAYNAME,    TYPEAFF_GRASS_ARRAYNAME,
                TYPEAFF_ELECTRIC_ARRAYNAME, TYPEAFF_PSYCHIC_ARRAYNAME,  TYPEAFF_ICE_ARRAYNAME,
                TYPEAFF_DRAGON_ARRAYNAME,   TYPEAFF_DARK_ARRAYNAME,     TYPEAFF_FAIRY_ARRAYNAME,
            }
            .Select(s => privateImplementationDetails.fields.First(f => f.name == s).defautValue.offset).ToArray();

            return data;
        }

        public void SaveToSources(FileManager fileManager, GlobalMetadata data)
        {
            var globalMetadataFile = fileManager.GetGlobalMetadataFile();

            globalMetadataFile.SetDataFromBuffer(data.buffer);
        }

        /// <summary>
        /// Returns the null terminated UTF8 string starting at the specified offset.
        /// </summary>
        private string ReadNullTerminatedString(byte[] buffer, long offset)
        {
            long endOffset = offset;
            while (buffer[endOffset] != 0)
                endOffset++;

            return Encoding.UTF8.GetString(buffer, (int)offset, (int)(endOffset - offset));
        }
    }
}
