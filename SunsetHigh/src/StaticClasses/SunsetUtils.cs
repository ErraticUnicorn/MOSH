using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace SunsetHigh
{
    public static class SunsetUtils
    {
        /// <summary>
        /// A util method for placing newlines in a string at appropriate moments
        /// to wrap the text inside a panel.
        /// </summary>
        /// <param name="text">The text to be wrapped</param>
        /// <param name="font">The font object that will render the text</param>
        /// <param name="width">The width, in pixels, that the text can occupy</param>
        /// <returns>The text with newlines inserted</returns>
        public static string wordWrapText(string text, SpriteFont font, int width)
        {
            string line = string.Empty;
            string returnString = string.Empty;
            string[] wordArray = text.Split(' ');

            int numLines = 1;
            foreach (string word in wordArray)
            {
                if (font.MeasureString(line + word).X > width)
                {
                    returnString = returnString + line + '\n';
                    line = string.Empty;
                    numLines++;
                }

                line = line + word + ' ';
            }
            return returnString + line;
        }

        /// <summary>
        /// Util method for casting string to enum
        /// </summary>
        /// <typeparam name="T">The enum type/name</typeparam>
        /// <param name="input">The string to cast</param>
        /// <returns>The appropriate enum value</returns>
        public static T parseEnum<T>(string input)
        {
            return (T)(Enum.Parse(typeof(T), input, true));
        }

        /// <summary>
        /// Util method for casting enum to string
        /// </summary>
        /// <typeparam name="T">The enum type/name</typeparam>
        /// <param name="input">The enum value to cast</param>
        /// <returns>The appropriate string</returns>
        public static string enumToString<T>(T input)
        {
            return Enum.GetName(typeof(T), input);
        }
    }
}
