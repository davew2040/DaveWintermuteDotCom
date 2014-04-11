using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheDaveSite.Code
{
    public class CommentsHelpers
    {
        public static string formatPost(string input)
        {
            var escaped = HttpUtility.HtmlEncode(input);
            var linebreaks = input.Replace("\n", "<br/>");
            var withUrls = replaceUrls(linebreaks);

            return withUrls;
        }

        public static string replaceUrls(string input)
        {
            string pattern = @"http[s]?://\S*";
            var regex = new System.Text.RegularExpressions.Regex(pattern);
            return regex.Replace(input, Urlizer);
        }

        public static string Urlizer(System.Text.RegularExpressions.Match match)
        {
            var url = match.Captures[0].Value;
            return String.Format("<a href=\"{0}\">{0}</a>", url);
        }
    }
}