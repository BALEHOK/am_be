using System;
using System.Collections.Generic;

namespace AppFramework.Core.Validation
{
    public static class StringExtender
    {
        public static List<int> AllIndexesOf(this string s, string value)
        {
            var indexes = new List<int>();

            if (String.IsNullOrEmpty(value))
                return indexes;

            for (int index = 0; ; index += value.Length)
            {
                index = s.IndexOf(value, index, StringComparison.Ordinal);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
    }
}