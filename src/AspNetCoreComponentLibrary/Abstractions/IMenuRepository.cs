using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary.Abstractions
{
    interface IMenuRepository : IRepositorySetStorageContext, IRepository<long, Menus>
    {
    }
}
