using System;
using System.Collections.Generic;

namespace Archon.Helpers
{
    public static class StringExtensions
    {

        /// <summary>
        /// Retrieves a substring from this instance. The substring starts after a specified substring and ends at another specified substring.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start">The string that comes before the desired substring</param>
        /// <param name="end">The string that comes after the desired substring</param>
        /// <returns>The substring between the two parameters, or null if the parameters are invalid.</returns>
        public static string Between(this string str, string start, string end)
        {
            if (!str.Contains(start) || !str.Contains(end) || str.IndexOf(end, str.IndexOf(start)) == -1)
            {
                return null;
            }
            int startIndex = str.IndexOf(start) + start.Length;
            int substringLength = str.IndexOf(end, str.IndexOf(start)) - startIndex;
            return str.Substring(startIndex, substringLength);
        }

        /// <summary>
        /// Retrieves a substring from this instance. The substring starts after a specified substring and continues to the end of the instance.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start">The string that comes before the desired substring</param>
        /// <returns>The substring after the parameter, or null if the parameter is invalid.</returns>
        public static string After(this string str, string start)
        {
            return !str.Contains(start) ? null : str.Substring(str.IndexOf(start) + start.Length);
        }

        /// <summary>
        /// Retrieves the substring of this instance before the specified string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="end">The string that comes after the desired substring</param>
        /// <returns>The substring before the parameter, or null if the parameter is invalid.</returns>
        public static string Before(this string str, string end)
        {
            return !str.Contains(end) ? str : str.Substring(0, str.IndexOf(end));
        }

        public static string Capitalize(this string str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }

        public static string AddTimeStamp(this string str)
        {
            return $"{str}_{DateTime.Now.Month}.{DateTime.Now.Day}.{DateTime.Now.Year}_{DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}";
        }
    }
}
