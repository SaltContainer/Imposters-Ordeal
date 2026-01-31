using System;
using System.Collections.Generic;
using System.Linq;

namespace ImpostersOrdeal
{
    public class ParserCollection
    {
        private FileManager fileManager;
        private Dictionary<Type, IParser> parsers = new Dictionary<Type, IParser>();

        public ParserCollection(FileManager fileManager)
        {
            this.fileManager = fileManager;
        }

        public void ParseAllDataForSet(GameDataSet set)
        {
            foreach (var field in set.GetAllParsableFields())
                field.SetValue(set, GetParserForType(field.FieldType).ParseFromSources(fileManager));
        }

        public void ParseSpecificDataForSet(GameDataSet set, List<Type> typesToParse)
        {
            foreach (var field in set.GetAllParsableFields())
                if (typesToParse.Contains(field.FieldType))
                    field.SetValue(set, GetParserForType(field.FieldType).ParseFromSources(fileManager));
        }

        public void SaveAllChangedDataForSet(GameDataSet set)
        {
            foreach (var field in set.GetAllParsableFields())
            {
                if (set.IsModified(field.FieldType))
                    GetParserForType(field.FieldType).SaveToSources(fileManager, field.GetValue(set));
            }
        }

        public List<Type> GetAllGameDataTypesUsingSourceTypes(List<Type> sourceTypes)
        {
            var setFields = new List<Type>();
            foreach (var parser in parsers)
            {
                if (parser.Value.GetRequiredDataSources().Any(sourceTypes.Contains))
                    setFields.Add(parser.Key);
            }

            return setFields;
        }

        public T ParseFromSources<T>()
        {
            return GetParserForType<T>().ParseFromSources(fileManager);
        }

        public void SaveToSources<T>(T data)
        {
            GetParserForType<T>().SaveToSources(fileManager, data);
        }

        public void AddParserForType<T>(IParser<T> parser)
        {
            parsers[typeof(T)] = parser;
        }

        public void RemoveParserForType<T>(IParser<T> parser)
        {
            parsers.Remove(typeof(T));
        }

        private IParser<T> GetParserForType<T>()
        {
            return (IParser<T>)parsers[typeof(T)];
        }

        private IParser GetParserForType(Type t)
        {
            return parsers[t];
        }
    }
}
