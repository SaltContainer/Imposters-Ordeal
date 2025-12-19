using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal.Utils
{
    public static class AssetTypeValueFieldExtensions
    {
        /// <summary>
        /// Gets the array elements of this array-typed AssetTypeValueField.
        /// </summary>
        public static List<AssetTypeValueField> GetArrayElements(this AssetTypeValueField self)
        {
            return GetInnerArrayField(self).Children;
        }

        /// <summary>
        /// Generates a new element of this array-typed AssetTypeValueField.
        /// </summary>
        public static AssetTypeValueField CreateArrayElement(this AssetTypeValueField self)
        {
            return ValueBuilder.DefaultValueFieldFromArrayTemplate(GetInnerArrayField(self));
        }

        /// <summary>
        /// Replaces the elements of this array-typed AssetTypeValueField.
        /// </summary>
        public static void SetArrayElements(this AssetTypeValueField self, List<AssetTypeValueField> items)
        {
            GetInnerArrayField(self).Children = items;
        }

        /// <summary>
        /// Replaces the byte elements of this array-typed AssetTypeValueField.
        /// </summary>
        public static void SetByteArrayElements(this AssetTypeValueField self, IEnumerable<byte> values)
        {
            var field = GetInnerArrayField(self);

            List<AssetTypeValueField> newFields = new();
            foreach (var value in values)
            {
                AssetTypeValueField baseField = field.CreateArrayElement();
                baseField.AsByte = value;
                newFields.Add(baseField);
            }
            field.SetArrayElements(newFields);
        }

        /// <summary>
        /// Replaces the int elements of this array-typed AssetTypeValueField.
        /// </summary>
        public static void SetIntArrayElements(this AssetTypeValueField self, IEnumerable<int> values)
        {
            var field = GetInnerArrayField(self);

            List<AssetTypeValueField> newFields = new();
            foreach (var value in values)
            {
                AssetTypeValueField baseField = field.CreateArrayElement();
                baseField.AsInt = value;
                newFields.Add(baseField);
            }
            field.SetArrayElements(newFields);
        }

        /// <summary>
        /// Replaces the uint elements of this array-typed AssetTypeValueField.
        /// </summary>
        public static void SetUIntArrayElements(this AssetTypeValueField self, IEnumerable<uint> values)
        {
            var field = GetInnerArrayField(self);

            List<AssetTypeValueField> newFields = new();
            foreach (var value in values)
            {
                AssetTypeValueField baseField = field.CreateArrayElement();
                baseField.AsUInt = value;
                newFields.Add(baseField);
            }
            field.SetArrayElements(newFields);
        }

        private static AssetTypeValueField GetInnerArrayField(AssetTypeValueField self)
        {
            if (self.FieldName != "Array" || self.TypeName != "Array")
                return self["Array"];
            else
                return self;
        }
    }
}
