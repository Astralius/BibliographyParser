using System;
using System.IO;
using System.Xml;
using XSLSerializer.Models;
using XSLSerializer.Interfaces;

namespace XlsCreator
{
    public class XSLCreator : IXSLCreator
    {
        private readonly XmlWriter xmlWriter;

        public XSLCreator(string outputPath)
        {
            if (IsValidXSLPath(outputPath))
            {
                var settings = new XmlWriterSettings
                {
                    Indent = false,
                    NewLineChars = string.Empty,
                    NewLineHandling = NewLineHandling.Replace,
                    NewLineOnAttributes = false,
                    WriteEndDocumentOnClose = true,
                    ConformanceLevel = ConformanceLevel.Document
                };
                this.xmlWriter = XmlWriter.Create(outputPath, settings);
            }
            else
            {
                throw new ArgumentException("The provided path is not a valid XSL file path!");
            }

            bool IsValidXSLPath(string path)
                    => Path.GetExtension(path).EndsWith("xsl");
        }

        public void AddSource(Source source)
        {
            throw new NotImplementedException();
        }

        public void AddSourcesArguments()
        {
            throw new NotImplementedException();
        }

        public void ApplyChanges()
        {
            throw new NotImplementedException();
        }
    }
}
