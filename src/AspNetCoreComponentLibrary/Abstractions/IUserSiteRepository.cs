using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface IUserSiteRepository : IRepositorySetStorageContext, IRepository<UserSites>
    {
    }
}
