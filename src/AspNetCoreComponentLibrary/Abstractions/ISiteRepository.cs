using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface ISiteRepository : IRepositorySetStorageContext, IRepository<long, Sites>
    {

    }/**/
}
