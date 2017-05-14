using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary.Abstractions
{
    interface IWithSiteId
    {
        long SiteId { get; set; }
    }
}
