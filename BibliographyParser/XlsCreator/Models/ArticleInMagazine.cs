using System;

namespace XSLSerializer.Models
{
    /// <summary>
    /// Model class representing a single science article.
    /// </summary>
    public class ArticleInMagazine : Source
    {
        public string MagazineName { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public int Part { get; set; }
    }
}
