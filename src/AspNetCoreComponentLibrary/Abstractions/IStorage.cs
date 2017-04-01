using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface IStorage
    {
        T GetRepository<T>() where T : IRepositorySetStorageContext;

        int Save();

        //Task<int> SaveAsync();
    }
}
