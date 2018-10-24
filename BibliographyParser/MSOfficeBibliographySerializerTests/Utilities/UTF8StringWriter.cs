using System.IO;
using System.Text;

namespace MSOfficeBibliographySerializerTests.Utilities
{
    public class UTF8StringWriter : StringWriter
    {
        public UTF8StringWriter() : base(new StringBuilder())
        {
            this.Encoding = Encoding.UTF8;
        }

        public override Encoding Encoding { get; }
    }
}
