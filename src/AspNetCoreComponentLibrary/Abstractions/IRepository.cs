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

        void ClearCache();
        List<T> GetUnblocked(long siteid);

        // в репозитории без кеширования ничего тут не делают
        // надо перенести логику этих функций в Remove и AfterSave. Функции оставим тока сделаем их приватными
        //void RemoveFromCache(K id);
        //void AddToCache(K index); // всегда превытаскиваем сущность из БД
    }

    // репозиторий для связей (нет ключа)
    public interface IRepository<T> where T : class
    {
        IQueryable<T> StartQuery();

        void Save(T item);
        void Remove(T item);

        // для массовых операций
        void Save(List<T> items);
        void Remove(List<T> items);
    }
}
