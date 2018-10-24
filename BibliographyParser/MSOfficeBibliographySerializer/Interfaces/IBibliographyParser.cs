using System.Collections.Generic;

namespace MSOfficeBibliographySerializer.Interfaces
{
    public interface IBibliographyParser<T>
    {
        T ParseSingle(string input);
        IList<T> ParseMultiple(string input);
        IList<T> ParseFromFile(string filePath);
    }
}
