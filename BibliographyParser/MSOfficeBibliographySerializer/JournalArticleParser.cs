using MSOfficeBibliographySerializer.Exceptions;
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
        private const string NameMatchingRegex = @"([\p{Lu}\p{Ll}-' ]+)(?> )(\p{Lu})(\p{Lu}?)(?!\.)(?>[ ,])?$";
        private const string PublicationYearMatchingRegex = @" \d{4};";
        private const string IssueMatchingRegex = @"(?<=\()\d+(?=\))";
        private const string PagesMatchingRegex = @"(\d+-?\d?)";
        private const string EtAlPartMatchingRegex = @".*(?=et al)";

        private const string InvalidSectionFormatErrorMessage = "Nieprawidłowy format sekcji.";
        private const string SectionNotFoundErrorMessage = "Nie można odnaleźć sekcji";

        public int CurrentElementNumber { get; private set; }
        public string CurrentSectionName { get; private set; }

        public JournalArticle ParseSingle(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException($"{nameof(input)} cannot be null or empty!");
            }

            var remainder = input;
            var result = new JournalArticle();

            #region Authors

            CurrentSectionName = "Autorzy";

            // until the first dot, everything is names or et al
            var authorsSectionLength = remainder.IndexOf('.') + 1;
            var authorsSection = remainder.Substring(0, authorsSectionLength);
            result.Authors = ParsePersons(authorsSection);
            remainder = remainder.Remove(0, authorsSectionLength);

            #endregion
            #region Title

            CurrentSectionName = "Tytuł/podtytuł";

            // Since title section can contain dots, we need to skip over to the end of Journal name/year section to find it out.
            var titleAndJournalNameSectionsLength = GetEndIndexOfJournalNameSection(remainder) + 1;
            var titleAndJournalNameSections = remainder.Substring(0, titleAndJournalNameSectionsLength);

            var titleSectionLength = titleAndJournalNameSections.LastIndexOfAny(new char[] { '.', '?' }) + 1;
            if (titleSectionLength > 1)
            {
                result.Title = titleAndJournalNameSections.Substring(0, titleSectionLength).Trim();
                remainder = remainder.Remove(0, titleSectionLength);
            }
            else
            {
                throw new ParsingException(SectionNotFoundErrorMessage)
                {
                    ElementNumber = CurrentElementNumber,
                    SectionName = CurrentSectionName
                };
            }

            #endregion
            #region Year

            CurrentSectionName = "Nazwa czasopisma/rok wydania";

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
                throw new ParsingException(SectionNotFoundErrorMessage)
                {
                    ElementNumber = CurrentElementNumber,
                    SectionName = CurrentSectionName
                };
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
                throw new ParsingException("Nie można odnaleźć nazwy czasopisma.")
                {
                    ElementNumber = CurrentElementNumber,
                    SectionName = CurrentSectionName
                };
            }

            #endregion
            #region Volume, Issue, Pages

            CurrentSectionName = "Szczegóły (wolumen, wydanie, strony)";

            var detailsSectionLength = remainder.IndexOf('.') + 1;
            var detailsSection = remainder.Substring(0, detailsSectionLength);

            var split = detailsSection.Split(VolumePagesDelimiter);
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
                    throw new ParsingException("Nieprawidłowy format zakresu stron.")
                    {
                        ElementNumber = CurrentElementNumber,
                        SectionName = CurrentSectionName,
                        IncorrectValue = split[1]
                    };
                }
            }
            else
            {
                throw new ParsingException(InvalidSectionFormatErrorMessage)
                {
                    ElementNumber = CurrentElementNumber,
                    SectionName = CurrentSectionName,
                    IncorrectValue = detailsSection
                };
            }
            remainder = remainder.Remove(0, detailsSectionLength);

            #endregion
            #region Other

            CurrentSectionName = "Pozostałe (ostatnia)";

            if (remainder.Trim().Length > 0)
            {
                var otherSplit = remainder.Split(StandardNumberDelimiter);
                if (otherSplit.Length == 2)
                {
                    result.StandardNumber = otherSplit[0].Trim();
                    result.Comments = otherSplit[1].Trim();
                }
                else
                {
                    throw new ParsingException(InvalidSectionFormatErrorMessage)
                    {
                        ElementNumber = CurrentElementNumber,
                        SectionName = CurrentSectionName,
                        IncorrectValue = remainder
                    };
                }
            }

            #endregion

            return result;

            int GetEndIndexOfJournalNameSection(string s1)
            {
                var publishedYearMatch = Regex.Match(s1, PublicationYearMatchingRegex);
                if (publishedYearMatch.Index > 0)
                {
                    return publishedYearMatch.Index + publishedYearMatch.Length - 1;
                }
                else
                {
                    throw new ParsingException(SectionNotFoundErrorMessage)
                    {
                        ElementNumber = CurrentElementNumber,
                        SectionName = CurrentSectionName,
                    };
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
                    throw new ParsingException("Nie można odnaleźć roku publikacji.")
                    {
                        ElementNumber = CurrentElementNumber,
                        SectionName = CurrentSectionName,
                        IncorrectValue = s2
                    };
                }
            }

            int ParseVolume(string s3)
            {
                if (string.IsNullOrEmpty(s3))
                {
                    throw new ParsingException("Nie można odnaleźć numeru woluminu.")
                    {
                        ElementNumber = CurrentElementNumber,
                        SectionName = CurrentSectionName,
                        IncorrectValue = s3
                    };
                }
                if (!int.TryParse(s3, out var volume))
                {
                    if (s3.IsRomanNumber())
                    {
                        volume = s3.FromRoman();
                    }
                    else
                    {
                        throw new ParsingException("Nieprawidłowy format numeru woluminu.")
                        {
                            ElementNumber = CurrentElementNumber,
                            SectionName = CurrentSectionName,
                            IncorrectValue = s3
                        };
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
            for (var index = 0; index < entries.Count; index++)
            {
                CurrentElementNumber = index + 1;   // display 1-based instead of 0-based
                result.Add(ParseSingle(entries[index].Value));
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
                throw new ParsingException(SectionNotFoundErrorMessage)
                {
                    ElementNumber = CurrentElementNumber,
                    SectionName = CurrentSectionName
                };
            }

            foreach (var name in names)
            {
                var match = Regex.Match(name, EtAlPartMatchingRegex);
                if (match.Success)
                {
                    if (match.Value.Length > 0)
                    {
                        result.Add(ParseSinglePerson(match.Value));
                    }
                    else if (names.Count == 1)
                    {
                        throw new ParsingException()
                        {
                            ElementNumber = CurrentElementNumber,
                            SectionName = CurrentSectionName,
                            IncorrectValue = name
                        };                        
                    }
                    continue;
                }
                else
                {
                    result.Add(ParseSinglePerson(name));
                }
            }

            return result;

            Person ParseSinglePerson(string personString)
            {
                if (!string.IsNullOrEmpty(personString))
                {
                    if (personString.EndsWith("."))
                    {
                        personString = personString.Remove(personString.Length - 1, 1);
                    }
                    var match = Regex.Match(personString, NameMatchingRegex);
                    if (match.Success)
                    {
                        return new Person
                        {
                            Last = match.Groups[1].Value,
                            First = match.Groups[2].Value,
                            Middle = (match.Groups.Count == 4)
                                        ? match.Groups[3].Value
                                        : string.Empty
                        };
                    }
                    else
                    {
                        throw new ParsingException()
                        {
                            ElementNumber = CurrentElementNumber,
                            SectionName = CurrentSectionName,
                            IncorrectValue = personString,
                        };
                    }
                }
                else
                {
                    throw new ArgumentException($"{nameof(personString)} cannot be null/empty!");
                }
            }
        }
    }
}
