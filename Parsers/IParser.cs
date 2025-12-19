namespace ImpostersOrdeal
{
    public interface IParser
    {
        object ParseFromSources(FileManager fileManager);
        void SaveToSources(FileManager fileManager, object data);
    }

    public interface IParser<T> : IParser
    {
        new T ParseFromSources(FileManager fileManager);
        void SaveToSources(FileManager fileManager, T data);
    }
}
