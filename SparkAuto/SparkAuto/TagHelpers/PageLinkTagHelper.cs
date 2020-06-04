using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SparkAuto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;

namespace SparkAuto.TagHelpers
{
    // Tag helpers, such as asp-page, need a target html tag
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        // ViewContext provides access to http request, response, etc.
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        // Values filled in from the view where the data is passed into the model
        // Values get to the view form the code behind class filling in the data
        // from the data returned from the db.
        public PagingInfo PageModel { get; set; }

        public String PageAction { get; set; }

        public String PageClass { get; set; }

        public String PageClassNormal { get; set; }

        public String PageClassSelected { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // dependancy injection: You inject them in startup class and then use it across all the classes in constructors.
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");

            for (int i=1; i <= (PageModel.TotalPage); i++)
            {
                TagBuilder tag = new TagBuilder("a");

                // PageModel is our class and UrlParm is a string in our model
                string url = PageModel.UrlParm.Replace(":", i.ToString());
                tag.Attributes["href"] = url;
                tag.AddCssClass(PageClass);
                tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                tag.InnerHtml.Append( i.ToString () );
                result.InnerHtml.AppendHtml(tag);
            }

            // Output list of pagination
            output.Content.AppendHtml(result.InnerHtml);
        }


    }

    
}
