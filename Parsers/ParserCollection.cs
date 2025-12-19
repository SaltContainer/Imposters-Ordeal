using System;
using System.Collections.Generic;

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
    }
}
