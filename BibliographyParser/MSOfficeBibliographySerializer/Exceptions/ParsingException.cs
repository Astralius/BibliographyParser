using System;
using System.Text;

namespace MSOfficeBibliographySerializer.Exceptions
{
    public class ParsingException : ArgumentException
    {
        private readonly string customMessage;

        public ParsingException() { }

        public ParsingException(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                this.customMessage = message;
            }     
        }

        public int ElementNumber { get; set; } = -1;
        public string SectionName { get; set; }
        public string IncorrectValue { get; set; }
        
        public override string Message => GetMessage();

        private string GetMessage()
        {
            var result = new StringBuilder();
            result.Append(string.IsNullOrEmpty(customMessage) ? "Błąd w pliku wejściowym." : customMessage);
            result.AppendLine();
            if (ElementNumber >= 0)
            {
                result.Append($" Element: {ElementNumber}.");
            }
            if (!string.IsNullOrEmpty(SectionName))
            {
                result.Append($" Sekcja: {SectionName}.");
            }
            if (!string.IsNullOrEmpty(IncorrectValue))
            {
                result.AppendLine();
                result.Append($" Błędna wartość: {IncorrectValue}");
            }
            return result.ToString();
        }
    }
}
