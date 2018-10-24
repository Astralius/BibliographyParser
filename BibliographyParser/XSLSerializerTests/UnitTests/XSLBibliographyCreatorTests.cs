using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using XSLSerializer;
using XSLSerializer.Models;
using XSLSerializerTests.Utilities;

namespace XSLSerializerTests.UnitTests
{
    [TestClass()]
    public class XSLBibliographyCreatorTests
    {
        private JournalArticle fakeJournalArticle;
        private ArticleInPeriodical fakeArticleInPeriodical;

        private readonly string ValidXSLFilePath = Path.Combine(Path.GetTempPath(), "Example.xsl");
        private readonly UTF8StringWriter writer = new UTF8StringWriter();

        private const string ValidXSLOutputContents =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?><b:Sources SelectedStyle=\"\" xmlns:b=\"http://schemas.openxmlformats.org/officeDocument/2006/bibliography\" xmlns=\"http://schemas.openxmlformats.org/officeDocument/2006/bibliography\"><b:Source><b:Tag>Smi18</b:Tag><b:SourceType>JournalArticle</b:SourceType><b:Guid>{2810274F-5AE2-42C1-9599-49142F8FB552}</b:Guid><b:Title>Is the Earth flat?</b:Title><b:PeriodicalTitle>Science</b:PeriodicalTitle><b:Year>2018</b:Year><b:Month>March</b:Month><b:Pages>128-33</b:Pages><b:Volume>2</b:Volume><b:Issue>21</b:Issue><b:Author><b:Author><b:NameList><b:Person><b:Last>Smith</b:Last><b:Middle>Edward</b:Middle><b:First>John</b:First></b:Person><b:Person><b:Last>O'Neil</b:Last><b:Middle>Sophie</b:Middle><b:First>Kate</b:First></b:Person></b:NameList></b:Author><b:Editor><b:NameList><b:Person><b:Last>Jackson</b:Last><b:Middle>Samuel</b:Middle><b:First>Gregory</b:First></b:Person></b:NameList></b:Editor></b:Author><b:City>New York</b:City><b:Day>3</b:Day><b:Publisher>AAAS</b:Publisher><b:Edition>Special Edition</b:Edition><b:ShortTitle>Flat</b:ShortTitle><b:StandardNumber>DOI</b:StandardNumber><b:Comments>10.1111/all.12423</b:Comments><b:JournalName>Science</b:JournalName></b:Source></b:Sources>";

        [TestInitialize]
        public void InitializeTest()
        {
            fakeJournalArticle = new JournalArticle
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
                        Last = "Smith",
                        Middle = "Edward",
                        First = "John"
                    },
                    new Person
                    {
                        Last = "O'Neil",
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
            fakeArticleInPeriodical = new ArticleInPeriodical();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            writer.Flush();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Initialize_InvalidPath_Throws()
        {
            var invalidPath = "*";
            new XSLBibliographyCreator().Initialize(invalidPath);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Initialize_InvalidFileExtension_Throws()
        {
            var pathWithInvalidExtension = Path.Combine(Path.GetTempPath(), "Example.bmp");
            new XSLBibliographyCreator().Initialize(pathWithInvalidExtension);
        }

        [TestMethod]
        public void Initialize_ValidFileExtension_Passes()
        {
            new XSLBibliographyCreator().Initialize(ValidXSLFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddSource_NotInitialized_Throws()
        {
            new XSLBibliographyCreator().AddSource(fakeJournalArticle);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void AddSource_UnsupportedSource_Throws()
        {
            XSLBibliographyCreator creator = GetInitializedCreator();
            creator.AddSource(fakeArticleInPeriodical);
        }

        [TestMethod]
        public void AddSource_SupportedSource_Passes()
        {
            XSLBibliographyCreator creator = GetInitializedCreator();
            creator.AddSource(fakeJournalArticle);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ApplyChanges_NotInitialized_Throws()
        {
            var creator = new XSLBibliographyCreator();
            creator.ApplyChanges();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ApplyChanges_NoChanges_Throws()
        {
            XSLBibliographyCreator creator = GetInitializedCreator();
            creator.ApplyChanges();
        }

        [TestMethod]
        public void ApplyChanges_SingleValidSource_OutputsCorrectXSL()
        {
            XSLBibliographyCreator creator = GetInitializedCreator();
            creator.AddSource(fakeJournalArticle);
            creator.ApplyChanges();

            Assert.AreEqual(ValidXSLOutputContents, writer.ToString());     
        }

        private XSLBibliographyCreator GetInitializedCreator()
        {
            var creator = new XSLBibliographyCreator();
            creator.Initialize(writer); // using StringBuilder overload to not affect file system.
            return creator;
        }
    }
}