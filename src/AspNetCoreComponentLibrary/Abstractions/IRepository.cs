using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface IRepositorySetStorageContext
    {
        void SetStorageContext(IStorageContext storageContext, IStorage storage);
    }

    public interface IRepository<K, T> : IEnumerable<T> where K : struct where T : BaseDM<K>
    {
        IQueryable<T> StartQuery();

        K Save(T item);
        void Block(K id);
        void UnBlock(K id);
        void Remove(K id);
    }
}
