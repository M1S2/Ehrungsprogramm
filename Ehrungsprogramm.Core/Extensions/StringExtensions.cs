using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ehrungsprogramm.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// This method creates an ellipsis in the middle of a path and it also allows you to speciy any length or delimiter.
        /// This is an extension method so you can use it like: `"c:\path\file.foo".EllipsisString()`
        /// </summary>
        /// <param name="path">Path to ellipsis</param>
        /// <param name="maxLength">Maximum number of characters after shortening the string</param>
        /// <param name="delimiter">The delimiter used to split the path</param>
        /// <returns>Shortened rawString (Length < maxLength)</returns>
        /// see: https://stackoverflow.com/questions/8403086/long-path-with-ellipsis-in-the-middle
        public static string EllipsisPath(this string path, int maxLength = 30, char delimiter = '\\')
        {
            if (path.Length <= maxLength)
            {
                return path;
            }

            maxLength -= 3; // account for ellipsis characters ("...")

            string final = path;
            List<string> parts;

            int loops = 0;
            while (loops++ < 100)
            {
                parts = path.Split(delimiter).ToList();
                parts.RemoveRange(parts.Count - 1 - loops, loops);
                if (parts.Count == 1)
                {
                    return parts.Last();
                }

                parts.Insert(parts.Count - 1, "...");
                final = string.Join(delimiter.ToString(), parts);
                if (final.Length < maxLength)
                {
                    return final;
                }
            }

            return path.Split(delimiter).ToList().Last();
        }
    }
}
