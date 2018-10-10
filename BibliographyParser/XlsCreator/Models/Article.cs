using System.Collections.Generic;

namespace BibliographyParser.Models
{
    /// <summary>
    /// Model class representing a single science article.
    /// </summary>
    public class Article
    {
        public IList<Author> Authors { get; set; }
        public string Title { get; set; }
        public string MagazineName { get; set; }
        public int PublicationYear { get; set; }
        public int PublicationMonth { get; set; }
        public int PublicationDay { get; set; }
        public int PageFrom { get; set; }
        public int PageTo { get; set; }
        public int Volume { get; set; }
        public int Part { get; set; }

        /// <summary>
        /// Digital Object Identifier (DOI) of an article.
        /// </summary>
        public string DOI { get; set; }
    }
}
