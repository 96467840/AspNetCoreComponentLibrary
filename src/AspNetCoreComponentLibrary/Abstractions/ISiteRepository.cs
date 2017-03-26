using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface ISiteRepository : IRepository
    {
        IEnumerable<Site> All();
    }
}
