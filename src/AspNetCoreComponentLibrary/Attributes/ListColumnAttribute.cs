using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class ListColumnBaseAttribute : Attribute
    {
        public string Width { get; set; }
        public string Format { get; set; }
        public string Name { get; set; }

        public HtmlString GetWidthAttribute()
        {
            if (string.IsNullOrWhiteSpace(Width)) return null;
            return new HtmlString("width=\""+ Width + "\"");
        }
    }

    public class ListColumnAttribute : ListColumnBaseAttribute
    {

    }

}
