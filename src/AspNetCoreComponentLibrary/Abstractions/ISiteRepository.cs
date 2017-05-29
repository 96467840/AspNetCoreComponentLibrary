using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface ISiteRepository : IRepositorySetStorageContext, IRepository<long, Sites>
    {
        List<UserSites> GetUserRights(long siteid);

        IQueryable<Sites> GetForUser(long userid);

    }/**/
}
