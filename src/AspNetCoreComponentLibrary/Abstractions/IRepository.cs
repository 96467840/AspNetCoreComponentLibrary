using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface IGetOptions
    {
        List<OptionVM> GetOptions(long siteid, string SelectValueName, string SelectTitleName, string SelectParentName, string SelectTreePrefix, bool SelectOnlyUnblocked);
    }

    public interface IRepositorySetStorageContext
    {
        void SetStorageContext(IStorageContext storageContext, IStorage storage, ILoggerFactory loggerFactory, ILocalizer2Garin localizer2Garin);
    }

    public interface IRepository<K, T> where T : BaseDM<K>
    {
        IQueryable<T> StartQuery(long siteid);
        IQueryable<T> GetUnblocked(long siteid);
        IQueryable<T> GetFiltered(long siteid, Form form);

        T this[K index] { get; }

        T Save(T item);

        // так как завершение транзакции выносим наружу, но этот метод оставим в интерфейсе чтобы вызывать его снаружи
        // теперь все внутри Save
        //void AfterSave(T item, bool isnew);
        //bool BeforeSave(T item);
        void Block(K id);
        void UnBlock(K id);
        void Remove(T item);

        void ClearCache(long? siteid);

        void TranslateItem(long siteid, Languages lang, List<Languages> siteLanguages, T item);
        void TranslateList(long siteid, Languages lang, List<Languages> siteLanguages, List<T> items);

        // в репозитории без кеширования ничего тут не делают
        // надо перенести логику этих функций в Remove и AfterSave. Функции оставим тока сделаем их приватными
        void RemoveFromCache(K id);
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
