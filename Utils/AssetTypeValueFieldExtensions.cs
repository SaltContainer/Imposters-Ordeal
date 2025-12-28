using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
        /// Replaces the elements of this array-typed AssetTypeValueField and initialize their values.
        /// </summary>
        public static void SetArrayElementsAndInit<T>(this AssetTypeValueField self, IEnumerable<T> values, Action<AssetTypeValueField, T> initAction)
        {
            var field = GetInnerArrayField(self);

            List<AssetTypeValueField> newFields = new();
            foreach (var value in values)
            {
                AssetTypeValueField baseField = field.CreateArrayElement();
                initAction.Invoke(baseField, value);
                newFields.Add(baseField);
            }
            field.SetArrayElements(newFields);
        }

        /// <summary>
        /// Gets the elements of this Vector3-typed AssetTypeValueField.
        /// </summary>
        public static Vector3 GetAsVector3(this AssetTypeValueField self)
        {
            return new Vector3()
            {
                X = self["x"].AsFloat,
                Y = self["y"].AsFloat,
                Z = self["z"].AsFloat,
            };
        }

        /// <summary>
        /// Sets the elements of this Vector3-typed AssetTypeValueField.
        /// </summary>
        public static void SetAsVector3(this AssetTypeValueField self, Vector3 value)
        {
            self["x"].AsFloat = value.X;
            self["y"].AsFloat = value.X;
            self["z"].AsFloat = value.X;
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
