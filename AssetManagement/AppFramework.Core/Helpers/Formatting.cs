using System.Text.RegularExpressions;
using System.Web;

namespace AppFramework.Core.Helpers
{
    public static class Formatting
    {
        /// <summary>
        /// Removes the script tags from HTML input
        /// </summary>
        /// <param name="htmlInput"></param>
        /// <returns></returns>
        public static string RemoveScriptTags(string htmlInput)
        {
            Regex re = new Regex(@"<[/]?(script)[^>]*?>", RegexOptions.IgnoreCase);
            return re.Replace(htmlInput, string.Empty);
        }

        /// <summary>
        /// Removes all HTML tags from input
        /// </summary>
        /// <param name="htmlInput"></param>
        /// <returns></returns>
        public static string RemoveHtmlTags(string htmlInput)
        {
            Regex re = new Regex(@"(\<(/?[^\>]+)\>)", RegexOptions.IgnoreCase);
            return re.Replace(htmlInput, string.Empty);
        }

        /// <summary>
        /// Performs input formatting to be acceptable for searching
        /// </summary>
        /// <param name="htmlInput"></param>
        /// <returns></returns>
        public static string FormatForSearch(string htmlInput)
        {
            // remove html tags
            htmlInput = Formatting.RemoveHtmlTags(htmlInput.Trim().ToLower());
            // decode html entities
            htmlInput = HttpUtility.HtmlDecode(htmlInput);
            // remove extra spaces
            Regex re = new Regex(@"\s+");
            htmlInput = re.Replace(htmlInput, " ");

            return htmlInput;
        }

        public static string Escape(string toEscape)
        {
            if (!string.IsNullOrEmpty(toEscape))
                return toEscape.Replace("'", "''");
            else return toEscape;
        }
    }
}
