using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class OrderByAttribute : Attribute
    {
        // чтоб по умолчанию была сортировка в порядке возрастания
        public bool Desc { get; set; }
        public int Priority { get; set; }
    }
}
