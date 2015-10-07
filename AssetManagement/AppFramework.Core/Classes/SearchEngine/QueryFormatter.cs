using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.Helpers;

namespace AppFramework.Core.Classes.SearchEngine
{
    public static class QueryFormatter
    {
        /// <summary>
        /// Performs the Query String formatiing to be used in search queries
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static string Format(string queryString)
        {
            // format string
            string formattedString = Formatting.FormatForSearch(queryString);
            // remove excluding words
            formattedString = string.Join(" ", (from string word in formattedString.Split(new char[] { ' ', ',' })
                                                where !string.IsNullOrEmpty(word) &&
                                                      !ApplicationSettings.SearchExcludeWords.Contains(word)
                                                select word).ToArray());
            return formattedString;
        }
    }
}
