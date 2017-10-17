using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebEssentials.AspNetCore.CdnTagHelpers
{
    public static class CdnHelper
    {
        public static string CdnifyHtmlImageUrls(this string html, string cdnUrl)
        {
            string result = html;
            MatchCollection matchCollection = Regex.Matches(result, "<img[^>]+src=\"(?<src>[^\"]+)\"[^>]+>");
            IEnumerable<Match> matches = new List<Match>(matchCollection.Cast<Match>()).ToArray().Reverse();

            foreach (Match match in matches)
            {
                Group group = match.Groups["src"];
                string value = group.Value;

                if (value.Contains("://") || value.StartsWith("//") || value.StartsWith("data:"))
                    continue;

                string sep = value.StartsWith("/") ? "" : "/";

                result = result.Insert(group.Index, $"{cdnUrl.TrimEnd('/')}{sep}");
            }

            return result;
        }
    }
}
