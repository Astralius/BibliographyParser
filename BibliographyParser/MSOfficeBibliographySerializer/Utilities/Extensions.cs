using System;
using System.Text.RegularExpressions;

namespace MSOfficeBibliographySerializer.Utilities
{
    public static class Extensions
    {
        private const string RomanMatchingRegex = @"^M{0,4}(CM|CD|D?C{0,3})(XC|XL|L?X{0,3})(IX|IV|V?I{0,3})$";

        /// <summary>
        /// Converts given integer number to its corresponding Roman numeral.
        /// </summary>
        /// <param name="decimalNumber">Decimal number to convert.</param>
        /// <returns>String representation of the conversion (Roman numeral).</returns>
        public static string ToRoman(this int decimalNumber)
        {
            if (decimalNumber > 3999)
            {
                throw new ArgumentException($"The number {decimalNumber} is too high for this conversion method.");
            }

            return
                new string('I', decimalNumber)
                    .Replace(new string('I', 1000), "M")
                    .Replace(new string('I', 900), "CM")
                    .Replace(new string('I', 500), "D")
                    .Replace(new string('I', 400), "CD")
                    .Replace(new string('I', 100), "C")
                    .Replace(new string('I', 90), "XC")
                    .Replace(new string('I', 50), "L")
                    .Replace(new string('I', 40), "XL")
                    .Replace(new string('I', 10), "X")
                    .Replace(new string('I', 9), "IX")
                    .Replace(new string('I', 5), "V")
                    .Replace(new string('I', 4), "IV");
        }

        /// <summary>
        /// Converts given Roman numeral to an integer.
        /// </summary>
        /// <param name="romanNumber">String representation of a Roman numeral to convert.</param>
        /// <returns>Integer result of the conversion (decimal number).</returns>
        public static int FromRoman(this string romanNumber)
        {
            var checkString = romanNumber.Trim(new char[] { 'I', 'V', 'X', 'L', 'D', 'C', 'M' });
            if (checkString.Length > 0)
            {
                throw new ArgumentException($"The number {romanNumber} is too high for this conversion method.");
            }

            return
                romanNumber
                    .Replace("IV", new string('I', 4))
                    .Replace("IX", new string('I', 9))
                    .Replace("XL", new string('I', 40))
                    .Replace("XC", new string('I', 90))
                    .Replace("CD", new string('I', 400))
                    .Replace("CM", new string('I', 900))
                    .Replace("V", new string('I', 5))
                    .Replace("X", new string('I', 10))
                    .Replace("L", new string('I', 50))
                    .Replace("D", new string('I', 500))
                    .Replace("C", new string('I', 100))
                    .Replace("M", new string('I', 1000)).Length;
        }

        /// <summary>
        /// Checks if the given string represents a valid Roman numeral.
        /// </summary>
        /// <param name="stringToCheck"></param>
        /// <returns>True, if the string represents a valid Roman numeral.</returns>
        public static bool IsRomanNumber(this string stringToCheck)
        {
            return Regex.IsMatch(stringToCheck, RomanMatchingRegex);
        }
    }
}
