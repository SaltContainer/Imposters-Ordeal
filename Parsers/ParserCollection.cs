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
            var fields = set.GetType().GetFields().Where(f => f.IsDefined(typeof(ParsableDataAttribute), false));

            foreach (var field in fields)
                field.SetValue(set, GetParserForType(field.FieldType).ParseFromSources(fileManager));
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
