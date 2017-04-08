using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface IMenuRepository : IRepositorySetStorageContext, IRepository<long, Menus>
    {
    }
}
