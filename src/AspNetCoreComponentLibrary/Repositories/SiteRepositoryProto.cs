using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public class SiteRepositoryProto : RepositoryWithCache<long, Sites>//, ISiteRepository
    //public class SiteRepositoryProto : Repository<long, Sites>//, ISiteRepository
    {
        
    }
}
