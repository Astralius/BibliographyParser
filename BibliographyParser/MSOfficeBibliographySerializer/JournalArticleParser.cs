using MSOfficeBibliographySerializer.Interfaces;
using MSOfficeBibliographySerializer.Models;
using MSOfficeBibliographySerializer.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MSOfficeBibliographySerializer
{
    public class JournalArticleParser : IBibliographyParser<JournalArticle>
    {
        private const char SectionDelimiter = '.';
        private const char ItemDelitimer = ',';
        private const char VolumePagesDelimiter = ':';
        private const char StandardNumberDelimiter = ':';

        private const string EntriesMatchingRegex = @"(?<=(\d\.[ \t])).{18,}";
        private const string NameMatchingRegex = @"\S+ \S{1,2}";
        private const string PublicationYearMatchingRegex = @" \d{4};";
        private const string IssueMatchingRegex = @"(?<=\()\d+(?=\))";
        private const string PagesMatchingRegex = @"(?<!\D)(\d+(-{1}\d+)?)(?!\D)";

        private const string NamesSectionErrorText = 
                    "This does not appear to be a valid " +
                    "names section of a bibliography record!";

        public JournalArticle ParseSingle(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException($"{nameof(input)} cannot be null or empty!");
            }

            var remainder = input;
            var result = new JournalArticle();

            #region Authors

            // until the first dot, everything is names or et al
            var authorsSectionLength = remainder.IndexOf('.') + 1;
            var authorsSection = remainder.Substring(0, authorsSectionLength);
            result.Authors = ParsePersons(authorsSection);
            remainder = remainder.Remove(0, authorsSectionLength);

            #endregion
            #region Title/Subtitle

            // Since title section can contain dots, we need to skip over to the end of Journal name/year section to find it out.
            var titleAndJournalNameSectionsLength = GetEndIndexOfPublisherSection(remainder) + 1;
            var titleAndJournalNameSections = remainder.Substring(0, titleAndJournalNameSectionsLength);

            var titleSectionLength = titleAndJournalNameSections.LastIndexOf('.') + 1;
            if (titleSectionLength > 1)
            {
                result.Title = titleAndJournalNameSections.Substring(0, titleSectionLength).Trim();
                remainder = remainder.Remove(0, titleSectionLength);
            }
            else
            {
                throw new ArgumentException("This bibliography entry does not seem to contain the required Title section!");
            }

            #endregion
            #region Year

            var JournalNameSectionLength = titleAndJournalNameSectionsLength - titleSectionLength;
            if (JournalNameSectionLength > 0)
            {
                string year = ParsePublicationYear(remainder);
                var yearIndex = remainder.IndexOf(year + ";");
                if (yearIndex > 0)
                {
                    result.Year = year;
                    // do not remove year substring from remainder (done along with publisher section).
                }
            }
            else
            {
                throw new ArgumentException("This bibliography entry does not seem to contain the required Publisher section!");
            }

            #endregion
            #region JournalName
         
            var JournalNameStringLength = JournalNameSectionLength - result.Year.Length - 1;
            if (JournalNameStringLength > 0)
            {
                result.JournalName = remainder.Substring(0, JournalNameStringLength).Trim();
                remainder = remainder.Remove(0, JournalNameSectionLength);
            }
            else
            {
                throw new ArgumentException("This bibliography entry does not seem to contain the required name of publisher!");
            }

            #endregion
            #region Volume, Issue, Pages

            var detailsSectionLength = remainder.IndexOf('.') + 1;
            var detailsSection = remainder.Substring(0, detailsSectionLength);

            var split = detailsSection.Split(new char[] { VolumePagesDelimiter, ItemDelitimer });
            if (split.Length == 2)
            {
                var issueMatch = Regex.Match(split[0], IssueMatchingRegex);
                if (issueMatch.Success)
                {
                    var volumeString = remainder.Substring(0, issueMatch.Index - 1);
                    result.Volume = ParseVolume(volumeString.Trim());
                    result.Issue = int.Parse(issueMatch.Value);
                }
                else
                {
                    result.Volume = ParseVolume(split[0].Trim());
                }

                var pagesMatch = Regex.Match(split[1], PagesMatchingRegex);
                if (pagesMatch.Success)
                {
                    result.Pages = pagesMatch.Value.Trim();
                }
                else
                {
                    throw new ArgumentException("This bibliography entry seems to contain " +
                        "incorrectly formatted pages range!");
                }
            }
            else
            {
                throw new ArgumentException("This bibliography entry seems to contain incorrectly " +
                    "formatted details section (Volume, Issue, Pages)!");
            }
            remainder = remainder.Remove(0, detailsSectionLength);

            #endregion
            #region Other

            if (remainder.Trim().Length > 0)
            {
                var otherSplit = remainder.Split(StandardNumberDelimiter);
                if (otherSplit.Length == 2)
                {
                    result.StandardNumber = otherSplit[1].Trim();
                    result.Comments = otherSplit[2].Trim();
                }
                else
                {
                    throw new ArgumentException("The last section of this bibliography entry " +
                        "seems to be formatted incorrectly!");
                }
            }

            #endregion

            return result;

            int GetEndIndexOfPublisherSection(string s1)
            {
                var publishedYearMatch = Regex.Match(s1, PublicationYearMatchingRegex);
                if (publishedYearMatch.Index > 0)
                {
                    return publishedYearMatch.Index + publishedYearMatch.Length - 1;
                }
                else
                {
                    throw new ArgumentException("Incorrect format of title and/or publisher section(s).");
                }
            }

            string ParsePublicationYear(string s2)
            {
                var publicationYearMatch = Regex.Match(s2, PublicationYearMatchingRegex);
                if (publicationYearMatch.Success)
                {
                    return publicationYearMatch.Value.Substring(1, 4);
                }
                else
                {
                    throw new ArgumentException("The provided input does not seem to contain the required publication year!");
                }
            }

            int ParseVolume(string s3)
            {
                if (string.IsNullOrEmpty(s3))
                {
                    throw new ArgumentException("The provided input does not seem to contain the required volume number!");
                }
                if (!int.TryParse(s3, out var volume))
                {
                    if (s3.IsRomanNumber())
                    {
                        volume = s3.FromRoman();
                    }
                    else
                    {
                        throw new ArgumentException("The provided input volume number seems to be corrupted!");
                    }
                }
                return volume;
            }
        }

        public IList<JournalArticle> ParseFromFile(string filePath)
        {
            var result = new List<JournalArticle>();

            if (File.Exists(filePath))
            {
                var fileContents = File.ReadAllText(filePath, Encoding.UTF8);
                result.AddRange(ParseMultiple(fileContents));
            }
            else
            {
                throw new ArgumentException($"The specified file ({filePath}) does not exist, " +
                    $"is not a file (invalid path), or you have no rights to access it.");
            }
            return result;
        }

        public IList<JournalArticle> ParseMultiple(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException($"{nameof(input)} cannot be null or empty!");
            }

            var result = new List<JournalArticle>();
            var entries = Regex.Matches(input, EntriesMatchingRegex);
            foreach (Match entry in entries)
            {
                result.Add(ParseSingle(entry.Value));
            }
            return result;
        }

        private List<Person> ParsePersons(string namesSection)
        {
            var result = new List<Person>();

            var names = namesSection.Split(ItemDelitimer)
                                    .Select(item => item.Trim())
                                    .ToList();

            if (names.Count == 0)
            {
                throw new ArgumentException(NamesSectionErrorText);
            }

            foreach (var name in names)
            {
                if (name.Contains("et al"))
                {
                    if (names.Count == 1)
                    {
                        throw new ArgumentException(NamesSectionErrorText);
                    }
                    continue;
                }
                else if (Regex.IsMatch(name, NameMatchingRegex))
                {
                    var split = name.Split(new char[] { ' ' });
                    var person = new Person
                    {
                        Last = split[0],
                        First = split[1].First().ToString(),
                        Middle = (split[1].Count() > 1) 
                                    ? split[1].Substring(1, split[1].Length-1) 
                                    : string.Empty,
                        
                    };
                    result.Add(person);
                }
                else
                {
                    throw new ArgumentException(NamesSectionErrorText);
                }
            }

            return result;
        }
    }
}
