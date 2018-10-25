using MSOfficeBibliographySerializer.Interfaces;
using MSOfficeBibliographySerializer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace MSOfficeBibliographySerializer
{
    public class XMLBibliographyCreator : IBibliographyCreator
    {
        private const string bibliographyPrefix = "b";
        private readonly string NotInitializedMessage = 
            $"You must first initialize this {nameof(XMLBibliographyCreator)} to use this method!";
        
        private XmlWriter XmlWriter;

        public bool Initialized { get; private set; }
        public bool Changed { get; private set; }

        /// <summary>
        /// Opens a .xml file on the specified path for editing and adds necessary heading elements.
        /// <para>Must be called before any other <see cref="XMLBibliographyCreator"/>'s method.</para>
        /// </summary>
        /// <param name="outputPath">Absolute path of the XML file to write to.</param>
        public void Initialize(string outputPath)
        {
            if (!IsValidXMLPath(outputPath))
            {
                throw new ArgumentException("The provided path is not a valid XSL file path!");
            }

            XmlWriterSettings settings = GetXMLWriterSettings();

            XmlWriter = XmlWriter.Create(outputPath, settings);
            AddOpeningSection(XmlWriter);

            Initialized = true;
            Changed = false;

            bool IsValidXMLPath(string path)
                    => Path.GetExtension(path).EndsWith("xml");
        }
       
        /// <summary>
        /// Redirects the output of all other <see cref="XMLBibliographyCreator"/>'s methods to a chosen <see cref="StringBuilder"/> object.
        /// <para>Must be called before any other <see cref="XMLBibliographyCreator"/>'s method.</para>
        /// </summary>
        /// <param name="output"><see cref="StringBuilder"/> object for initialization of the XMLWriter.</param>
        public void Initialize(StringWriter output)
        {
            if (output == null)
            {
                throw new ArgumentNullException("The provided output cannot be null!");
            }

            XmlWriter = XmlWriter.Create(output, GetXMLWriterSettings());
            AddOpeningSection(XmlWriter);

            Initialized = true;
            Changed = false;
        }

        /// <summary>
        /// Adds a single source to the XSL, using the initialized writer.
        /// </summary>
        /// <param name="source"><see cref="Source"/> object to be serialized to XSL.</param>
        public void AddSource(Source source)
        {
            if (!Initialized)
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

            Changed = true;
        }

        /// <summary>
        /// Saved changes to the currently opened XSL document and closes it for editing.
        /// <see cref="Initialize"/> must be called again in order to call any other method.
        /// </summary>
        public void ApplyChanges()
        {
            if (!Initialized)
            {
                throw new InvalidOperationException(NotInitializedMessage);
            }
            if (!Changed)
            {
                throw new InvalidOperationException("Attempted to apply changes, " +
                    "but no changes have been made.");
            }

            XmlWriter.WriteEndElement();
            XmlWriter.WriteEndDocument();
            XmlWriter.Close();
            XmlWriter = null;

            Initialized = false;
        }

        private static XmlWriterSettings GetXMLWriterSettings()
        {
            return new XmlWriterSettings
            {
                Indent = false,
                NewLineChars = string.Empty,
                NewLineHandling = NewLineHandling.Replace,
                NewLineOnAttributes = false,
                ConformanceLevel = ConformanceLevel.Document,
                CloseOutput = true
            };
        }

        private static void AddOpeningSection(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement(bibliographyPrefix, "Sources", "http://schemas.openxmlformats.org/officeDocument/2006/bibliography");
            writer.WriteAttributeString("SelectedStyle", "");
            writer.WriteAttributeString("xmlns", bibliographyPrefix, null, "http://schemas.openxmlformats.org/officeDocument/2006/bibliography");
            writer.WriteAttributeString("xmlns", null, "http://schemas.openxmlformats.org/officeDocument/2006/bibliography");
        }

        private void AddJournalArticle(JournalArticle article)
        {
            XmlWriter.WriteStartElement(bibliographyPrefix, "Source", null);
            XmlWriter.WriteElementString(bibliographyPrefix, "Tag", null, article.Tag);
            XmlWriter.WriteElementString(bibliographyPrefix, "SourceType", null, article.Type.ToString());
            XmlWriter.WriteElementString(bibliographyPrefix, "Guid", null, article.Guid.ToString());
            XmlWriter.WriteElementString(bibliographyPrefix, "Title", null, article.Title);            
            XmlWriter.WriteElementString(bibliographyPrefix, "PeriodicalTitle", null, article.JournalName);
            XmlWriter.WriteElementString(bibliographyPrefix, "Year", null, article.Year);
            if (!string.IsNullOrEmpty(article.Month))
            XmlWriter.WriteElementString(bibliographyPrefix, "Month", null, article.Month);
            XmlWriter.WriteElementString(bibliographyPrefix, "Pages", null, article.Pages);
            XmlWriter.WriteElementString(bibliographyPrefix, "Volume", null, article.Volume.ToString());
            XmlWriter.WriteElementString(bibliographyPrefix, "Issue", null, article.Issue.ToString());

            XmlWriter.WriteStartElement(bibliographyPrefix, "Author", null);
            AddPersons("Author", article.Authors);
            if (article.Editors.Count > 0) AddPersons("Editor", article.Editors);
            XmlWriter.WriteEndElement();

            if (!string.IsNullOrEmpty(article.City))            XmlWriter.WriteElementString(bibliographyPrefix, "City", null, article.City);
            if (!string.IsNullOrEmpty(article.Day))             XmlWriter.WriteElementString(bibliographyPrefix, "Day", null, article.Day);
            if (!string.IsNullOrEmpty(article.Publisher))       XmlWriter.WriteElementString(bibliographyPrefix, "Publisher", null, article.Publisher);
            if (!string.IsNullOrEmpty(article.Edition))         XmlWriter.WriteElementString(bibliographyPrefix, "Edition", null, article.Edition);
            if (!string.IsNullOrEmpty(article.ShortTitle))      XmlWriter.WriteElementString(bibliographyPrefix, "ShortTitle", null, article.ShortTitle);
            if (!string.IsNullOrEmpty(article.StandardNumber))  XmlWriter.WriteElementString(bibliographyPrefix, "StandardNumber", null, article.StandardNumber);
            if (!string.IsNullOrEmpty(article.Comments))        XmlWriter.WriteElementString(bibliographyPrefix, "Comments", null, article.Comments);

            XmlWriter.WriteElementString(bibliographyPrefix, "JournalName", null, article.JournalName);
            XmlWriter.WriteEndElement();
        }

        private void AddPersons(string groupName, IList<Person> persons)
        {
            XmlWriter.WriteStartElement(bibliographyPrefix, groupName, null);
            XmlWriter.WriteStartElement(bibliographyPrefix, "NameList", null);
            foreach(var person in persons)
            {
                XmlWriter.WriteStartElement(bibliographyPrefix, "Person", null);
                XmlWriter.WriteElementString(bibliographyPrefix, "Last", null, person.Last);
                if (!string.IsNullOrEmpty(person.Middle)) XmlWriter.WriteElementString(bibliographyPrefix, "Middle", null, person.Middle);
                XmlWriter.WriteElementString(bibliographyPrefix, "First", null, person.First);
                XmlWriter.WriteEndElement();
            }
            XmlWriter.WriteEndElement();
            XmlWriter.WriteEndElement();
        }
    }
}
