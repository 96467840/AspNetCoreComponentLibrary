using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    //public class SiteRepositoryBase : RepositoryWithCache<long, Sites>, ISiteRepository
    public class SiteRepositoryProto : Repository<long, Sites>, ISiteRepository
    {
    }
}
