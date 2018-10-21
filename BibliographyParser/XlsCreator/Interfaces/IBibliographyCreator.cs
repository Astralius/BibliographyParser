using XSLSerializer.Models;

namespace XSLSerializer.Interfaces
{
    public interface IBibliographyCreator
    {
        bool Initialized { get; }
        bool Changed { get; }

        void Initialize(string outputPath);
        void AddSource(Source source);
        void ApplyChanges();
        
    }
}
