using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class SEO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }

        public SEO(string title, string description = null, string keywords = null)
        {
            Title = title;
            Description = description;
            Keywords = keywords;
        }
    }
}
