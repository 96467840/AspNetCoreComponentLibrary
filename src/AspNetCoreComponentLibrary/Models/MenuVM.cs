using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class MenuItem
    {
        public string Href { get; set; }
        public string Title { get; set; }
        public string liClass { get; set; }
        public string aClass { get; set; }
        public bool IsBlank { get; set; }
        public int Priority { get; set; }

        public MenuItem(string href, string title, string liclass=null, string aclass=null, bool isBlank = false)
        {
            Href = href; Title = title;
            liClass = liclass; aClass = aclass;
            IsBlank = isBlank;
        }

    }

    public class MenuVM
    {
        public List<MenuItem> Items { get; set; }
        public string Class { get; set; }
        public MenuVM(List<MenuItem> items, string @class = "breadcrumb")
        {
            Items = items;
            Class = @class;
        }

        public void SortMenu()
        {
            Items = Items.OrderBy(i => i.Priority).ToList();
        }

    }
}
