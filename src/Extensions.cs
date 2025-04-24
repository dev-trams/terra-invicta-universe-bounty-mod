using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityModManagerNet;

namespace UniverseBounty
{
    public static class Extensions
    {
        public static List<List<T>> Chunk<T>(this List<T> list, int chunkSize)
        {
            var result = new List<List<T>>();

            var current = new List<T>();
            foreach (var t in list)
            {
                current.Add(t);
                if (current.Count != chunkSize) continue;
                result.Add(current);
                current = new List<T>();
            }

            if (current.Count > 0)
                result.Add(current);
            return result;
        }

        public static bool EqualsIgnoreCase(this string value, string otherValue) => value.ToUpper().Equals(otherValue.ToUpper());

        public static string ToWords(this string value) => Regex.Replace(value, "(\\B[A-Z])", " $1");

        // ReSharper disable once InconsistentNaming
        public static int point(this int x) => UnityModManager.UI.Scale(x);
    }
}
