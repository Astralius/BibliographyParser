using MSOfficeBibliographySerializer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSOfficeBibliographySerializer.Models
{
    public abstract class Source
    {
        private static readonly List<string> TagsInUse = new List<string>();
        
        public string Guid { get; set; } = $"{{{System.Guid.NewGuid().ToString().ToUpper()}}}";
        public SourceType Type { get; protected set; }
        public IList<Person> Authors { get; set; } = new List<Person>();
        public IList<Person> Editors { get; set; } = new List<Person>();
        public string Tag => (string.IsNullOrEmpty(currentTag)) ? ComposeTag() : currentTag;      
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string Publisher { get; set; }
        public string Year { get; set; }
        public string City { get; set; }
        public string Pages { get; set; }
        public string StandardNumber { get; set; }
        public string Comments { get; set; }
        public int Volume { get; set; }

        private string currentTag;

        private string ComposeTag()
        {
            var result = "ERROR_Tag";

            var firstAuthor = this.Authors.FirstOrDefault();
            if (firstAuthor != null)
            {
                var namesCombined = $"{firstAuthor.Last}{firstAuthor.First}{firstAuthor.Middle}";
                var yearStringLength = this.Year.Length;
                if (namesCombined.Length > 0 || yearStringLength >= 0)
                {
                    var namesSubstringLength = (namesCombined.Length < 3) ? namesCombined.Length : 3;
                    var yearSubstringLength = (yearStringLength < 2) ? yearStringLength : 2;

                    result = this.MakeUniqueTag(
                        $"{namesCombined.Substring(0, namesSubstringLength)}" +
                        $"{this.Year.Substring(yearStringLength - yearSubstringLength) }");

                    currentTag = result;
                }
            }

            return result;
        }
        
        private string MakeUniqueTag(string baseTag)
        {       
            if (baseTag != null)
            {
                var suffix = 0;
                var result = baseTag;
                while (TagsInUse.Contains(result) || result.Equals(string.Empty))
                {
                    result = baseTag + ++suffix;
                }
                TagsInUse.Add(result);                    
                return result;                  
            }
            else
            {
                throw new ArgumentException("Null is not a valid argument for this method!");
            }          
        }
    }
}
