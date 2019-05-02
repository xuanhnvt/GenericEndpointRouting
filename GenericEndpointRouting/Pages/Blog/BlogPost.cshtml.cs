using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace GenericEndpointRouting.Pages.Blog
{
    public class BlogPostModel : PageModel
    {
        public string RouteDataString = String.Empty;
        public string Slug;

        public void OnGet(string genericSlug)
        {
            Slug = genericSlug;
            this.RouteDataString = String.Format("Route Data: {0}", JsonConvert.SerializeObject(this.HttpContext.GetRouteData()));
        }
    }
}