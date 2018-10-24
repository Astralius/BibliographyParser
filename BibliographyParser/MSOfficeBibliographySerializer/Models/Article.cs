namespace MSOfficeBibliographySerializer.Models
{
    /// <summary>
    /// Model class representing a single science article.
    /// </summary>
    public abstract class Article : Source
    {
        public string Month { get; set; }
        public string Day { get; set; }
        public int Issue { get; set; }
    }
}
