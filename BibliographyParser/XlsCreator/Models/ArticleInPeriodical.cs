using XSLSerializer.Enums;

namespace XSLSerializer.Models
{
    /// <summary>
    /// Model class representing a single article in a science periodical.
    /// </summary>
    public class ArticleInPeriodical : Article
    {
        public string PeriodicalTitle { get; set; }
        
        public ArticleInPeriodical() : base()
        {
            this.Type = SourceType.ArticleInAPeriodical;
        }
    }
}
