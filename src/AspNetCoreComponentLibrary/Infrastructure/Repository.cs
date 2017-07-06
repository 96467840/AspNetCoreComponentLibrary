using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public abstract class Repository<K, T> : IRepository<K, T> /*where K : struct/*, IComparable<K>*/ where T : BaseDM<K>
    {
        protected DbSet<T> DbSet { get; set; }

        protected IStorageContext StorageContext;
        protected IStorage Storage { get; set; }
        protected ILoggerFactory LoggerFactory;
        protected ILogger Logger;
        protected ILogger LoggerMEF;
        protected ILocalizer2Garin Localizer2Garin;

        public void SetStorageContext(IStorageContext storageContext, IStorage storage, ILoggerFactory loggerFactory, ILocalizer2Garin localizer2Garin)
        {
            try
            {
                StorageContext = storageContext;
                Storage = storage;
                LoggerFactory = loggerFactory;
                Logger = LoggerFactory.CreateLogger(this.GetType().FullName);
                // для красоты в логах EntityFrameworkCore
                LoggerMEF = LoggerFactory.CreateLogger(Utils.MEFNameSpace);
                //if (StorageContext == null) Logger.LogCritical("AAAAAAAAAAAAAAAAAAA!!!!!!!!!");
                DbSet = (StorageContext as DbContext).Set<T>();
                Localizer2Garin = localizer2Garin;
            }
            catch (Exception e)
            {
                Logger.LogCritical(e.ToString());
            }
        }

        protected HtmlString LocalizeHtml(string key, params object[] args)
        {
            return Localizer2Garin.LocalizeHtml(key, args);
        }
        protected string Localize(string key, params object[] args)
        {
            return Localizer2Garin.Localize(key, args);
        }

        public virtual IQueryable<T> StartQuery(long siteid)
        {
            if (typeof(T).IsImplementsInterface(typeof(IWithSiteId)) && siteid > 0)
            {
                return DbSet
                    .AsNoTracking()
                    .Where(i => ((IWithSiteId)i).SiteId == siteid);
            }
            return DbSet.AsNoTracking();
        }

        public IQueryable<T> GetFiltered(long siteid, Form form)
        {
            //Logger.LogTrace("Repository GetFiltered for {0}. IWithSiteId = {1}", GetType().FullName, typeof(IWithSiteId).GetTypeInfo().IsAssignableFrom(typeof(T)));
            var query = StartQuery(siteid);

            if (form != null)
            {
                // пример работы с сущностями https://stackoverflow.com/questions/42485128/entity-framework-core-what-is-the-fastest-way-to-check-if-a-generic-entity-is-a

                foreach (var field in form.Fields)
                {
                    var nameLC = field.PropertyName;

                    {
                        {
                            Logger.LogTrace("..... Filter {name} type: {type} ", nameLC, field.Type.Name);
                            try
                            {
                                switch (field.Type.Name)
                                {
                                    case "Boolean":
                                        var value = field.GetValues<bool>();
                                        Logger.LogTrace("..... ==> Boolean filter for {name} args: {boolargs}", nameLC, value);
                                        if (value != null && value.Count == 1) // другие случаи означают все варианты
                                        {
                                            query = query.Where(ExpressionHelper.ComparePropertyWithConst<T, bool>(nameLC, value[0]));
                                        }
                                        break;
                                    case "":
                                        break;
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.LogWarning("Couldn't apply filter {name} with arguments: {exception}", nameLC, e.ToString());
                                // сделать бы перевод сообщения
                                // эта ошибка может возникнуть только при ручном указании параметров
                                //throw new Exception(string.Format(("Couldn't apply filter {name} with arguments: {args}"), attr.Title, string.Join(", ", filter[nameLC]) ));
                            }
                        }
                    }
                }
            }
            return query;
        }

        public IQueryable<T> GetUnblocked(long siteid)
        {
            Logger.LogTrace("Repository GetUnblocked for {0}.", GetType().FullName);
            var query = StartQuery(siteid);

            if (typeof(T).IsImplementsInterface(typeof(IBlockable)))
                query = query.Where(i => !((IBlockable)i).IsBlocked);

            return query;
        }

        public virtual void Save(T item)
        {
            if (item == null) throw new ArgumentNullException();

            if (!BeforeSave(item)) return;

            var isnew = Utils.CheckDefault(item.Id);

            if (!isnew)
            {
                DbSet.Update(item);
            }
            else
            {
                DbSet.Add(item);
            }
        }

        public virtual bool BeforeSave(T item)
        {
            Logger.LogTrace("Repository BeforeSave for {0}", item.Id);
            return true;
        }

        public virtual void AfterSave(T item, bool isnew)
        {
            Logger.LogTrace("Repository AfterSave for {0}", item.Id);
        }/**/

        public virtual void Remove(T item)
        {
            DbSet.Remove(item);
        }

        protected virtual void SetBlock(K id, bool value)
        {
            //if (typeof(T) is IBlockable)
            if (typeof(T).IsImplementsInterface(typeof(IBlockable)))
            {
                T item = (T)Activator.CreateInstance(typeof(T));
                item.Id = id;
                ((IBlockable)item).IsBlocked = value;
                DbSet.Update(item);
            }
        }

        public void Block(K id) { SetBlock(id, true); }
        public void UnBlock(K id) { SetBlock(id, false); }

        //public virtual void AddToCache(K id) { }
        //public virtual void RemoveFromCache(K id) { }

        public virtual T this[K index]
        {
            get
            {
                return GetSingleFromDB(index);
            }
        }

        public virtual T GetSingleFromDB(K index)
        {
            using (new BLog(LoggerMEF, "GetSingleFromDB", GetType().FullName))
            {
                return DbSet
                    // очень важно!
                    .AsNoTracking()
                    .FirstOrDefault(i => i.Id.Equals(index));
            }
        }

        public virtual void ClearCache(long? siteid)
        {
            Logger.LogTrace("Repository ClearCache {0} for siteid={1}", this.GetType().FullName, siteid);
        }

    }
}
