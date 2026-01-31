using System;
using System.Collections.Generic;

namespace ImpostersOrdeal
{
    public interface IParser
    {
        object ParseFromSources(FileManager fileManager);
        void SaveToSources(FileManager fileManager, object data);
        IEnumerable<Type> GetRequiredDataSources();
    }

    public interface IParser<T> : IParser
    {
        new T ParseFromSources(FileManager fileManager);
        void SaveToSources(FileManager fileManager, T data);
    }
}
