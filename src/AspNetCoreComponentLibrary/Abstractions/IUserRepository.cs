//using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface IUserRepository : IRepositorySetStorageContext, IRepository<long, Users>
    {
    }
}
