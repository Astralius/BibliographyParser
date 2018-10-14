using XSLSerializer.Models;

namespace XSLSerializer.Interfaces
{
    public interface IXSLCreator
    {
        void AddSource(Source article);
        void AddSourcesArguments();
        void ApplyChanges();
    }
}
