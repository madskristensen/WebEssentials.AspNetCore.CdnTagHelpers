using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;

namespace WebEssentials.AspNetCore.CdnTagHelpers
{
    [HtmlTargetElement("img")]
    [HtmlTargetElement("audio")]
    [HtmlTargetElement("video")]
    [HtmlTargetElement("track")]
    [HtmlTargetElement("source")]
    [HtmlTargetElement("link")]
    [HtmlTargetElement("script")]
    public class CdnTagHelper : TagHelper
    {
        private string _cdnUrl;

        private static readonly Dictionary<string, string[]> _attributes = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "audio", new[] { "src" } },
            { "embed", new[] { "src" } },
            { "img", new[] { "src", "srcset" } },
            { "input", new[] { "src" } },
            { "link", new[] { "href" } },
            { "menuitem", new[] { "icon" } },
            { "script", new[] { "src" } },
            { "source", new[] { "src", "srcset" } },
            { "track", new[] { "src" } },
            { "video", new[] { "poster", "src" } },
        };

        public CdnTagHelper(IConfiguration config)
        {
            _cdnUrl = config["cdn:url"];
        }

        public override int Order => int.MaxValue;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(_cdnUrl) || string.IsNullOrEmpty(output.TagName))
            {
                return;
            }

            if (output.Attributes.ContainsName("no-cdn"))
            {
                output.Attributes.RemoveAll("cdn-prop");
                return;
            }

            if (_attributes.TryGetValue(output.TagName, out var attributeNames))
            {
                foreach (string attrName in attributeNames)
                {
                    PrependCdnUrl(output, attrName);
                }
            }

            if (output.Attributes.TryGetAttribute("cdn-prop", out var prop))
            {
                PrependCdnUrl(output, "cdn-prop");
                output.Attributes.RemoveAll("cdn-prop");
            }
        }

        private void PrependCdnUrl(TagHelperOutput output, string attrName)
        {
            string attrValue = GetValue(attrName, output);

            // Don't modify absolute paths
            if (string.IsNullOrWhiteSpace(attrName) || string.IsNullOrWhiteSpace(attrValue) || attrValue.Contains("://") || attrValue.StartsWith("//"))
            {
                return;
            }

            string[] values = attrValue.Split(',');
            string modifiedValue = null;

            foreach (string value in values)
            {
                string fullUrl = _cdnUrl.Trim().TrimEnd('/') + "/" + value.Trim().TrimStart('~', '/');
                modifiedValue += fullUrl + ", ";
            }

            var result = new HtmlString((modifiedValue ?? attrValue).Trim(',', ' '));

            output.Attributes.SetAttribute(attrName, result);
        }

        public static string GetValue(string attrName, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(attrName) || !output.Attributes.TryGetAttribute(attrName, out var attr))
            {
                return null;
            }

            if (attr.Value is string stringValue)
            {
                return stringValue;
            }
            else if (attr.Value is IHtmlContent content)
            {
                if (content is HtmlString htmlString)
                {
                    return htmlString.ToString();
                }

                using (var writer = new StringWriter())
                {
                    content.WriteTo(writer, HtmlEncoder.Default);
                    return writer.ToString();
                }
            }

            return null;
        }
    }
}
