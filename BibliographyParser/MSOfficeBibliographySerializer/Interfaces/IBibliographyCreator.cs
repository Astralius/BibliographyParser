using MSOfficeBibliographySerializer.Models;

namespace MSOfficeBibliographySerializer.Interfaces
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
