using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class BreadcrumbItem
    {
        public string Href { get; set; }
        public string Title { get; set; }
        public string liClass { get; set; }
        public string aClass { get; set; }

        public BreadcrumbItem(string href, string title, string liclass=null, string aclass=null)
        {
            Href = href; Title = title;
            liClass = liclass; aClass = aclass;
        }

    }

    public class BreadcrumbVM
    {
        public List<BreadcrumbItem> Items { get; set; }
        public string Class { get; set; }
        public BreadcrumbVM(List<BreadcrumbItem> items, string @class = "breadcrumb")
        {
            Items = items;
            Class = @class;
        }
    }
}
