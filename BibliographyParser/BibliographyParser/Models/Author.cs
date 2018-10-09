namespace BibliographyParser.Models
{
    /// <summary>
    /// Model class representing a single author of an article.
    /// </summary>
    public class Author
    {
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
    }
}
