using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DbcExtractor
{
    static class Util
    {
        internal static void FixIcon(Type T, uint id)
        {
            string icon = DBC.GetString(T, id);
            if (!String.IsNullOrEmpty(icon))
                DBC.SetString(T, id, Path.GetFileNameWithoutExtension(icon).Urlize());
        }

        internal static string Urlize(this string input)
        {
            input = input
                .Replace(" / ", "-")
                .Replace("'", "")
                .Trim();

            input = Regex.Replace(input, @"[^a-z0-9_]", "-", RegexOptions.IgnoreCase);

            input = input
                .Trim('-')
                .Replace("--", "-")
                .Replace("--", "-");

            return input.ToLower();
        }

        internal static string Enumize(this string input)
        {
            input = input
                .Replace(" / ", "_")
                .Replace("'", "")
                .Trim();

            input = Regex.Replace(input, @"[^a-z0-9_]", "_", RegexOptions.IgnoreCase);

            input = input
                .Trim('_')
                .Replace("__", "_")
                .Replace("__", "_");

            if (input.Length > 0)
            {
                if (!Regex.IsMatch("" + input[0], @"[a-z_]", RegexOptions.IgnoreCase))
                    input = "_" + input;
            }

            return input;
        }
    }
}
