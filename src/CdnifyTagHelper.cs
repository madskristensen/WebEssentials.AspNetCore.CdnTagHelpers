using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;

namespace WebEssentials.AspNetCore.CdnTagHelpers
{
    [HtmlTargetElement("*", Attributes = "cdnify")]
    public class CdnifyTagHelper : TagHelper
    {
        private string _cdnUrl;

        public CdnifyTagHelper(IConfiguration config)
        {
            _cdnUrl = config["cdn:url"];
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(_cdnUrl) || string.IsNullOrEmpty(output.TagName))
            {
                return;
            }

            TagHelperContent content = await output.GetChildContentAsync(NullHtmlEncoder.Default);
            string html = content.GetContent();
            string cdnified = html.CdnifyHtmlImageUrls(_cdnUrl);

            output.Attributes.RemoveAll("cdnify");
            output.Content.SetHtmlContent(cdnified);
        }
    }
}
