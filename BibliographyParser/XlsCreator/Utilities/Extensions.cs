using System;

namespace XSLSerializer.Utilities
{
    public static class Extensions
    {
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
    }
}
