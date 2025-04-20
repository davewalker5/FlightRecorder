using System;
using System.Text;

namespace FlightRecorder.Tests.RandomGenerators
{
    public class StringGenerators
    {
        private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static readonly Random _random = new();

        /// <summary>
        /// Generate a random alphanumeric word of the specified length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomWord(int length)
        {
            // Construct the set of letters to choose from
            string characters = $"{Letters.ToLower()}{Letters.ToUpper()}";

            // Iterate over the number of characters
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                // Select a random offset within the character set and append that character
                var offset = (int)_random.NextInt64(0, characters.Length);
                builder.Append(characters[offset]);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generate a phrase consisting of the specified number of words 
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static string GenerateRandomPhrase(int words)
        {
            StringBuilder builder = new StringBuilder();

            // Iterate over the required number of words
            for (int i = 0; i < words; i++)
            {
                if (builder.Length > 0)
                {
                    builder.Append(' ');
                }

                var wordLength = (int)_random.NextInt64(1, 16);
                var word = GenerateRandomWord(wordLength);
                builder.Append(word);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Generate a random numeric string
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomNumericString(int minimum, int maximum, int length)
        {
            var format = $"D{length}";
            var numeric = _random.Next(minimum, maximum).ToString(format);
            return numeric;
        }
    }
}
