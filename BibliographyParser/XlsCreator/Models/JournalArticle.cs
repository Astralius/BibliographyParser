using XSLSerializer.Enums;

namespace XSLSerializer.Models
{
    /// <summary>
    /// Model class representing a single article in a science journal.
    /// </summary>
    public class JournalArticle : Article
    {
        public string JournalName { get; set; }
        
        public JournalArticle() : base()
        {
            this.Type = SourceType.JournalArticle;
        }
    }
}
