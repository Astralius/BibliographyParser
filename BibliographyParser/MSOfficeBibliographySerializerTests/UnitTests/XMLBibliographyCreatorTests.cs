using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSOfficeBibliographySerializer;
using MSOfficeBibliographySerializer.Models;
using MSOfficeBibliographySerializerTests.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace MSOfficeBibliographySerializerTests.UnitTests
{
    [TestClass()]
    public class XMLBibliographyCreatorTests
    {
        private JournalArticle fullValidJournalArticle;
        private ArticleInPeriodical emptyArticleInPeriodical;

        private readonly string ValidXMLFilePath = Path.Combine(Path.GetTempPath(), "Example.xml");
        private readonly UTF8StringWriter writer = new UTF8StringWriter();

        private const string ValidXMLOutputContentsSingle =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><b:Sources SelectedStyle=\"\" xmlns:b=\"http://schemas.openxmlformats.org/officeDocument/2006/bibliography\" xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/bibliography\"><b:Source><b:Tag>Van18</b:Tag><b:SourceType>JournalArticle</b:SourceType><b:Guid>{2810274F-5AE2-42C1-9599-49142F8FB552}</b:Guid><b:Title>Is the Earth flat?</b:Title><b:PeriodicalTitle>Science</b:PeriodicalTitle><b:Year>2018</b:Year><b:Month>March</b:Month><b:Pages>128-33</b:Pages><b:Volume>2</b:Volume><b:Issue>21</b:Issue><b:Author><b:Author><b:NameList><b:Person><b:Last>Van Graaf</b:Last><b:Middle>Edward</b:Middle><b:First>John</b:First></b:Person><b:Person><b:Last>O'Neil-Tronce</b:Last><b:Middle>Sophie</b:Middle><b:First>Kate</b:First></b:Person></b:NameList></b:Author><b:Editor><b:NameList><b:Person><b:Last>Jackson</b:Last><b:Middle>Samuel</b:Middle><b:First>Gregory</b:First></b:Person></b:NameList></b:Editor></b:Author><b:City>New York</b:City><b:Day>3</b:Day><b:Publisher>AAAS</b:Publisher><b:Edition>Special Edition</b:Edition><b:ShortTitle>Flat</b:ShortTitle><b:StandardNumber>DOI</b:StandardNumber><b:Comments>10.1111/all.12423</b:Comments><b:JournalName>Science</b:JournalName></b:Source></b:Sources>";
        private const string ValidXMLOutputContentsMultiple =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><b:Sources SelectedStyle=\"\" xmlns:b=\"http://schemas.openxmlformats.org/officeDocument/2006/bibliography\" xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/bibliography\"><b:Source><b:Tag>AAA00</b:Tag><b:SourceType>JournalArticle</b:SourceType><b:Guid>{BFBC5CD3-6C53-4139-B970-597CE2F1550E}</b:Guid><b:Title>A</b:Title><b:PeriodicalTitle>A</b:PeriodicalTitle><b:Year>2000</b:Year><b:Month>1</b:Month><b:Pages>25-6</b:Pages><b:Volume>1</b:Volume><b:Issue>1</b:Issue><b:Author><b:Author><b:NameList><b:Person><b:Last>A</b:Last><b:Middle>A</b:Middle><b:First>A</b:First></b:Person></b:NameList></b:Author></b:Author><b:JournalName>A</b:JournalName></b:Source><b:Source><b:Tag>BBB00</b:Tag><b:SourceType>JournalArticle</b:SourceType><b:Guid>{A1897368-CA90-4D6E-A59F-A3B5461EBA45}</b:Guid><b:Title>B</b:Title><b:PeriodicalTitle>B</b:PeriodicalTitle><b:Year>2000</b:Year><b:Month>2</b:Month><b:Pages>69-72</b:Pages><b:Volume>3</b:Volume><b:Issue>2</b:Issue><b:Author><b:Author><b:NameList><b:Person><b:Last>B</b:Last><b:Middle>B</b:Middle><b:First>B</b:First></b:Person></b:NameList></b:Author></b:Author><b:JournalName>B</b:JournalName></b:Source><b:Source><b:Tag>CCC00</b:Tag><b:SourceType>JournalArticle</b:SourceType><b:Guid>{ADACDE88-5BEA-41CC-8777-07AB04CEA6ED}</b:Guid><b:Title>C</b:Title><b:PeriodicalTitle>C</b:PeriodicalTitle><b:Year>2000</b:Year><b:Month>3</b:Month><b:Pages>33-6</b:Pages><b:Volume>3</b:Volume><b:Issue>33</b:Issue><b:Author><b:Author><b:NameList><b:Person><b:Last>C</b:Last><b:Middle>C</b:Middle><b:First>C</b:First></b:Person></b:NameList></b:Author></b:Author><b:JournalName>C</b:JournalName></b:Source></b:Sources>";

        [TestInitialize]
        public void InitializeTest()
        {
            fullValidJournalArticle = new JournalArticle
            {
                Guid = "{2810274F-5AE2-42C1-9599-49142F8FB552}",    // for matching purposes only (normally assigned automatically).
                Title = "Is the Earth flat?",
                Year = "2018",
                City = "New York",
                Publisher = "AAAS",
                JournalName = "Science",
                Month = "March",
                Volume = 2,
                Issue = 21,
                Authors = new List<Person>
                {
                    new Person
                    {
                        Last = "Van Graaf",
                        Middle = "Edward",
                        First = "John"
                    },
                    new Person
                    {
                        Last = "O'Neil-Tronce",
                        Middle = "Sophie",
                        First = "Kate"
                    }
                },
                Editors = new List<Person>
                {
                    new Person
                    {
                        Last = "Jackson",
                        Middle = "Samuel",
                        First = "Gregory"
                    }
                },
                Day = "3",
                Pages = "128-33",
                ShortTitle = "Flat",
                Edition = "Special Edition",
                StandardNumber = "DOI",
                Comments = "10.1111/all.12423"
            };
            emptyArticleInPeriodical = new ArticleInPeriodical();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            Source.ResetTagsList();
            writer.Flush();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Initialize_InvalidPath_Throws()
        {
            var invalidPath = "*";
            new XMLBibliographyCreator().Initialize(invalidPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Initialize_InvalidFileExtension_Throws()
        {
            var pathWithInvalidExtension = Path.Combine(Path.GetTempPath(), "Example.bmp");
            new XMLBibliographyCreator().Initialize(pathWithInvalidExtension);
        }

        [TestMethod]
        public void Initialize_ValidFileExtension_Passes()
        {
            new XMLBibliographyCreator().Initialize(ValidXMLFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddSource_NotInitialized_Throws()
        {
            new XMLBibliographyCreator().AddSource(fullValidJournalArticle);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AddSource_UnsupportedSource_Throws()
        {
            XMLBibliographyCreator creator = GetInitializedCreator();
            creator.AddSource(emptyArticleInPeriodical);
        }

        [TestMethod]
        public void AddSource_SupportedSource_Passes()
        {
            XMLBibliographyCreator creator = GetInitializedCreator();
            creator.AddSource(fullValidJournalArticle);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ApplyChanges_NotInitialized_Throws()
        {
            var creator = new XMLBibliographyCreator();
            creator.ApplyChanges();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ApplyChanges_NoChanges_Throws()
        {
            XMLBibliographyCreator creator = GetInitializedCreator();
            creator.ApplyChanges();
        }

        [TestMethod]
        public void ApplyChanges_SingleValidSource_OutputsCorrectXML()
        {
            XMLBibliographyCreator creator = GetInitializedCreator();
            creator.AddSource(fullValidJournalArticle);
            creator.ApplyChanges();

            Assert.AreEqual(ValidXMLOutputContentsSingle, writer.ToString());     
        }

        [TestMethod]
        public void ApplyChanges_MultipleValidSources_OutputsCorrectXML()
        {
            var sources = new List<Source>()
            {
                new JournalArticle
                {
                    Guid = "{BFBC5CD3-6C53-4139-B970-597CE2F1550E}",
                    Title = "A",
                    JournalName = "A",
                    Year = "2000",
                    Month = "1",
                    Volume = 1,
                    Issue = 1,
                    Pages = "25-6",
                    Authors = new List<Person>()
                    {
                        new Person()
                        {
                            Last = "A",
                            Middle = "A",
                            First = "A"
                        }
                    }
                },
                new JournalArticle
                {
                    Guid = "{A1897368-CA90-4D6E-A59F-A3B5461EBA45}",
                    Title = "B",
                    JournalName = "B",
                    Year = "2000",
                    Month = "2",
                    Volume = 3,
                    Issue = 2,
                    Pages = "69-72",
                    Authors = new List<Person>()
                    {
                        new Person()
                        {
                            Last = "B",
                            Middle = "B",
                            First = "B"
                        }
                    }
                },
                new JournalArticle
                {
                    Guid = "{ADACDE88-5BEA-41CC-8777-07AB04CEA6ED}",
                    Title = "C",
                    JournalName = "C",
                    Year = "2000",
                    Month = "3",
                    Volume = 3,
                    Issue = 33,
                    Pages = "33-6",
                    Authors = new List<Person>()
                    {
                        new Person()
                        {
                            Last = "C",
                            Middle = "C",
                            First = "C"
                        }
                    }
                }
            };
            XMLBibliographyCreator creator = GetInitializedCreator();
            foreach(var source in sources)
            {
                creator.AddSource(source);
            }
            creator.ApplyChanges();
            Assert.AreEqual(ValidXMLOutputContentsMultiple, writer.ToString());
        }

        private XMLBibliographyCreator GetInitializedCreator()
        {
            var creator = new XMLBibliographyCreator();
            creator.Initialize(writer); // using StringBuilder overload to not affect file system.
            return creator;
        }
    }
}