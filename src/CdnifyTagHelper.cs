using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;

namespace WebEssentials.AspNetCore.CdnTagHelpers
{
    [HtmlTargetElement("*", Attributes = _attrName)]
    public class CdnifyTagHelper : TagHelper
    {
        private string _cdnUrl;
        private const string _attrName = "cdnify";

        public CdnifyTagHelper(IConfiguration config)
        {
            _cdnUrl = config["cdn:url"];
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output?.Attributes?.RemoveAll(_attrName);

            if (string.IsNullOrWhiteSpace(_cdnUrl) || string.IsNullOrEmpty(output.TagName))
            {
                return;
            }

            string html = output.Content.IsModified ? output.Content.GetContent() : (await output.GetChildContentAsync()).GetContent();
            string cdnified = html.CdnifyHtmlImageUrls(_cdnUrl);

            output.Content.SetHtmlContent(cdnified);
        }
    }
}
