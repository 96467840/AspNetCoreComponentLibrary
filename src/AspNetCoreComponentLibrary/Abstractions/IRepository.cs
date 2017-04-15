using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface IRepositorySetStorageContext
    {
        void SetStorageContext(IStorageContext storageContext, IStorage storage, ILoggerFactory loggerFactory);
    }

    public interface IRepository<K, T> where K : struct where T : BaseDM<K>
    {
        IQueryable<T> StartQuery();
        //IEnumerable<T> StartQuery();

        T this[K? index] { get; }

        void Save(T item);
        void AfterSave(T item);
        void Block(K id);
        void UnBlock(K id);
        void Remove(K id);
    }
}
