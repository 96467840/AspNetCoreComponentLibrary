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

        T this[K index] { get; }

        void Save(T item);
        // так как завершение транзакции выносим наружу, но этот метод оставим в интерфейсе чтобы вызывать его снаружи
        void AfterSave(T item, bool isnew);
        bool BeforeSave(T item);
        void Block(K id);
        void UnBlock(K id);
        void Remove(T item);

        // в репозитории без кеширования ничего тут не делают
        void RemoveFromCache(K id);
        void AddToCache(K index); // всегда превытаскиваем сущность из БД
    }

    // реопзиторий для связей (нет ключа, сохранение идет автоматически при изменении основной сущности, функции сохранения оставим)
    // сохранение оставить сложно так как нет ключа - нет проверки на новое старое
    public interface IRepository<T> where T : class
    {
        IQueryable<T> StartQuery();

        //T this[string index] { get; }

        void Save(T item);
        //void AfterSave(T item, bool isnew);
        //void Block(string id);
        //void UnBlock(string id);
        void Remove(T item);
    }
}
