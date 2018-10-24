using System.Collections.Generic;
using System.IO;

namespace XSLSerializer.Interfaces
{
    public interface IBibliographyParser<T>
    {
        T ParseSingle(string input);
        IList<T> ParseMultiple(string input);
        IList<T> ParseFromFile(string filePath);
    }
}
