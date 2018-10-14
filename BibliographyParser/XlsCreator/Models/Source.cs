using System;
using System.Collections.Generic;
using System.Linq;

namespace XSLSerializer.Models
{
    public class Source
    {
        private static readonly List<string> Tags = new List<string>();
        
        public Guid Guid { get; private set; }
        public IList<Person> Authors { get; set; } = new List<Person>();
        public IList<Person> Editors { get; set; } = new List<Person>();
        public string Tag => ComposeTag();      
        public string Title { get; set; }
        public string ShortTitle { get; set; }
        public string Publisher { get; set; }
        public string Year { get; set; }
        public string City { get; set; }
        public string Pages { get; set; }
        public string StandardCodeType { get; set; }
        public string Comment { get; set; }
        public int Volume { get; set; }

        public Source()
        {
            this.Guid = Guid.NewGuid();
        }

        private string ComposeTag()
        {
            var result = GetUniqueTag("Tag");

            var firstAuthor = this.Authors.FirstOrDefault();
            if (firstAuthor != null)
            {
                var namesCombined = $"{firstAuthor.Last}{firstAuthor.First}{firstAuthor.Middle}";
                var yearStringLength = this.Year.Length;
                if (namesCombined.Length > 0 || yearStringLength >= 0)
                {
                    var namesSubstringLength = (namesCombined.Length < 3) ? namesCombined.Length : 3;
                    var yearSubstringLength = (yearStringLength < 2) ? yearStringLength : 2;
                    result = this.GetUniqueTag(
                        $"{namesCombined.Substring(0, namesSubstringLength)}" +
                        $"{this.Year.Substring(yearStringLength - yearSubstringLength) }");
                }
            }
            return result;
        }
        
        private string GetUniqueTag(string tag)
        {
            if (tag != null)
            {
                int suffix = 0;
                while (Tags.Contains(tag) || tag.Equals(string.Empty))
                {
                    suffix += 1;
                    tag += suffix;
                }
                return tag;
            }
            else
            {
                throw new ArgumentException("Null is not a valid argument for this method!");
            }
        }
    }
}
