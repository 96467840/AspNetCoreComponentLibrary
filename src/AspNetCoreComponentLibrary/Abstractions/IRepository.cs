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

    public interface IRepository<K, T> where T : BaseDM<K>
    {
        IQueryable<T> StartQuery();
        //IEnumerable<T> StartQuery();

        T this[K index] { get; }

        void Save(T item);
        void AfterSave(T item, bool isnew);
        void Block(K id);
        void UnBlock(K id);
        void Remove(K id);
    }

    // реопзиторий для связей (нет ключа, сохранение идет автоматически при изменении основной сущности, функции сохранения оставим)
    // сохранение оставить сложно так как нет ключа - нет проверки на новое старое
    public interface IRepository<T> where T : class
    {
        IQueryable<T> StartQuery();

        //T this[string index] { get; }

        //void Save(T item);
        //void AfterSave(T item, bool isnew);
        //void Block(string id);
        //void UnBlock(string id);
        //void Remove(string id);
    }
}
