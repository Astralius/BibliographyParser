namespace MSOfficeBibliographySerializer.Models
{
    /// <summary>
    /// Model class representing a single person.
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Gets or sets the person's surname.
        /// </summary>
        public string Last { get; set; }

        /// <summary>
        /// Gets or sets the person's first name.
        /// </summary>
        public string First { get; set; }

        /// <summary>
        /// Gets or sets the person's second name.
        /// </summary>
        public string Middle { get; set; }
    }
}
