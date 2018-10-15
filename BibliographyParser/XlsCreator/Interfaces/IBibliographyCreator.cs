using XSLSerializer.Models;

namespace XSLSerializer.Interfaces
{
    public interface IBibliographyCreator
    {
        void AddSource(Source source);
        void ApplyChanges();
    }
}
