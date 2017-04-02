using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface IRepositorySetStorageContext
    {
        void SetStorageContext(IStorageContext storageContext, IStorage storage);
    }

    public interface IRepository<K, T> : IEnumerable<T> where K : struct where T : BaseDM<K>
    {
        IEnumerable<T> AllFromDB();
        K Save(T item);
    }
}
