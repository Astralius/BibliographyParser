using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSOfficeBibliographySerializer;
using MSOfficeBibliographySerializer.Exceptions;
using System;
using System.Text;

namespace MSOfficeBibliographySerializerTests.UnitTests
{
    [TestClass]
    public class JournalArticleParserTests
    {
        private const string InvalidEntry = "Nwaru BI, Hickstein L, Panesar SS, Roberts G, Muraro A and Sheikh A on behalf of the EAACI Food Allergy and Anaphylaxis Guidelines Group. Prevalence of common food allergies in Europe: a systematic review and meta-analysis. Allergy 2014; DOI:10.1111/all.12423.";
        private const string ValidEntry = "Koletzko S, Niggemann B, Arato A, et al. European Society of Pediatric Gastroenterology, Hepatology, and Nutrition. Diagnostic approach and management of cow-s-milk protein allergy in infants and children: ESPGHAN GI Committee practical guidelines. J Pediatr Gastroenterol Nutr 2012;55:221-229.";

        private JournalArticleParser parser;

        [TestInitialize]
        public void InitializeTest()
        {
            parser = new JournalArticleParser();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSingle_Empty_Throws()
        {
            parser.ParseSingle(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSingle_Null_Throws()
        {
            parser.ParseSingle(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ParsingException))]
        public void ParseSingle_InvalidFormat_Throws()
        {
            parser.ParseSingle(InvalidEntry);
        }

        [TestMethod]
        public void ParseSingle_ValidEntry_Passes()
        {
            parser.ParseSingle(ValidEntry);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseMultiple_Empty_Throws()
        {
            parser.ParseMultiple(string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseMultiple_Null_Throws()
        {
            parser.ParseMultiple(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ParsingException))]
        public void ParseMultiple_OneValidOneInvalid_Throws()
        {
            var builder = new StringBuilder();
            builder.Append($"1. {ValidEntry}");
            builder.AppendLine();
            builder.Append($"2. {InvalidEntry}");

            parser.ParseMultiple(builder.ToString());
        }

        [TestMethod]
        public void ParseMultiple_ValidEntries_Passes()
        {
            var builder = new StringBuilder();
            builder.Append($"1. {ValidEntry}");
            builder.AppendLine();
            builder.Append($"2. {ValidEntry}");

            parser.ParseMultiple(builder.ToString());
        }
    }
}
