using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericEndpointRouting.Entities;

namespace GenericEndpointRouting.Services
{

    public class SlugService : ISlugService
    {
        private List<Slug> _slugTable = new List<Slug>{
            new Slug(1, "blog-post-1"),
            new Slug(1, "blog-post-2"),
            new Slug(1, "blog-post-3"),
            new Slug(1, "blog-post-4"),
            new Slug(1, "blog-post-5"),
            new Slug(2, "blog-category-1"),
            new Slug(2, "blog-category-2"),
            new Slug(2, "blog-category-3"),
            new Slug(2, "blog-category-4"),
            new Slug(2, "blog-category-5")
        };

        public SlugService()
        {

        }

        public Slug GetSlugFromName(string name)
        {
            return _slugTable.Where(slug => slug.Name.Equals(name.ToLower())).FirstOrDefault();
        }
    }
}
