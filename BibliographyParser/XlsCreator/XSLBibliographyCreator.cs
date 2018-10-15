using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using XSLSerializer.Interfaces;
using XSLSerializer.Models;

namespace XSLSerializer
{
    public class XSLBibliographyCreator : IBibliographyCreator
    {
        private const string bibliographyNamespace = "b";
        private readonly string NotInitializedMessage = 
            $"You must first initialize this {nameof(XSLBibliographyCreator)} to use this method!";

        private static XmlWriter xmlWriter;
        private static bool initialized = false;

        /// <summary>
        /// Opens a .xsl file on the specified path for editing and adds necessary heading elements.
        /// <para>Must be called before any other <see cref="XSLBibliographyCreator"/>'s method.</para>
        /// </summary>
        /// <param name="outputPath"></param>
        public static void Initialize(string outputPath)
        {
            if (!IsValidXSLPath(outputPath))
            {
                throw new ArgumentException("The provided path is not a valid XSL file path!");             
            }

            var settings = new XmlWriterSettings
            {
                Indent = false,
                NewLineChars = string.Empty,
                NewLineHandling = NewLineHandling.Replace,
                NewLineOnAttributes = false,
                WriteEndDocumentOnClose = true,
                ConformanceLevel = ConformanceLevel.Document
            };
            xmlWriter = XmlWriter.Create(outputPath, settings);
            AddOpeningSection(xmlWriter);
            initialized = true;

            bool IsValidXSLPath(string path)
                    => Path.GetExtension(path).EndsWith("xsl");
        }

        /// <summary>
        /// Adds a single source to the XSL, using the initialized writer.
        /// </summary>
        /// <param name="source"><see cref="Source"/> object to be serialized to XSL.</param>
        public void AddSource(Source source)
        {
            if (!initialized)
            {
                throw new InvalidOperationException(NotInitializedMessage);
            }
            
            if (source is JournalArticle journalArticle)
            {
                AddJournalArticle(journalArticle);
            }
            else
            {
                throw new NotSupportedException($"Adding this type of {nameof(Source)} is not supported");
            }
        }

        /// <summary>
        /// Saved changes to the currently opened XSL document and closes it for editing.
        /// <see cref="Initialize"/> must be called again in order to call any other method.
        /// </summary>
        public void ApplyChanges()
        {
            if (!initialized)
            {
                throw new InvalidOperationException(NotInitializedMessage);
            }

            xmlWriter.WriteEndElement();
            xmlWriter.Close();
            xmlWriter = null;

            initialized = false;
        }

        private static void AddOpeningSection(XmlWriter writer)
        {
            writer.WriteStartElement("Sources", bibliographyNamespace);
            writer.WriteAttributeString("SelectedStyle", "");
            writer.WriteAttributeString(bibliographyNamespace, "xmlns", "http://schemas.openxmlformats.org/officeDocument/2006/bibliography");
            writer.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/officeDocument/2006/bibliography");           
        }

        private void AddJournalArticle(JournalArticle article)
        {
            xmlWriter.WriteStartElement("Source", bibliographyNamespace);
            xmlWriter.WriteElementString("Tag", bibliographyNamespace, article.Tag);
            xmlWriter.WriteElementString("SourceType", bibliographyNamespace, article.Type.ToString());
            xmlWriter.WriteElementString("Guid", bibliographyNamespace, article.Guid.ToString());
            xmlWriter.WriteElementString("Title", bibliographyNamespace, article.Title);            
            xmlWriter.WriteElementString("Year", bibliographyNamespace, article.Year);
            xmlWriter.WriteElementString("City", bibliographyNamespace, article.City);
            xmlWriter.WriteElementString("Publisher", bibliographyNamespace, article.Publisher);
            xmlWriter.WriteElementString("JournalName", bibliographyNamespace, article.JournalName);
            xmlWriter.WriteElementString("Month", bibliographyNamespace, article.Month);
            xmlWriter.WriteElementString("Volume", bibliographyNamespace, article.Volume.ToString());
            xmlWriter.WriteElementString("Issue", bibliographyNamespace, article.Issue.ToString());

            xmlWriter.WriteStartElement("Author", bibliographyNamespace);
            AddPersons("Author", article.Authors);
            AddPersons("Editor", article.Editors);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteElementString("Day", bibliographyNamespace, article.Day);
            xmlWriter.WriteElementString("Pages", bibliographyNamespace, article.Pages);
        }

        private void AddPersons(string groupName, IList<Person> persons)
        {
            xmlWriter.WriteStartElement(groupName, bibliographyNamespace);
            xmlWriter.WriteStartElement("NameList", bibliographyNamespace);
            foreach(var person in persons)
            {
                xmlWriter.WriteStartElement("Person", bibliographyNamespace);
                xmlWriter.WriteElementString("Last", bibliographyNamespace, person.Last);
                xmlWriter.WriteElementString("Middle", bibliographyNamespace, person.Middle);
                xmlWriter.WriteElementString("First", bibliographyNamespace, person.First);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }
    }
}
