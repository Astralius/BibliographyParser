using MSOfficeBibliographySerializer.Enums;

namespace MSOfficeBibliographySerializer.Models
{
    /// <summary>
    /// Model class representing a single article in a science journal.
    /// </summary>
    public class JournalArticle : Article
    {
        public string Edition { get; set; }
        public string JournalName { get; set; }
        
        public JournalArticle() : base()
        {
            this.Type = SourceType.JournalArticle;
        }
    }
}
