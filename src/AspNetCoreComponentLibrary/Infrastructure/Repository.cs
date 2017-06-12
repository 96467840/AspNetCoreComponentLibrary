using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Html;
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

        protected HtmlString Localize(string key)
        {
            return Localizer2Garin.Localize(key);
            //return new HtmlString(key);
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

        public IQueryable<T> GetFiltered(long siteid, Dictionary<string, List<string>> filter = null)
        {
            //Logger.LogTrace("Repository GetFiltered for {0}. IWithSiteId = {1}", GetType().FullName, typeof(IWithSiteId).GetTypeInfo().IsAssignableFrom(typeof(T)));
            var query = StartQuery(siteid);

            if (filter != null)
            {
                // пример работы с сущностями https://stackoverflow.com/questions/42485128/entity-framework-core-what-is-the-fastest-way-to-check-if-a-generic-entity-is-a
                //var entityType = (StorageContext as DbContext).Model.FindEntityType(typeof(T));
                //var primaryKey = entityType.FindPrimaryKey();
                
                var props = typeof(T).GetProperties();
                foreach (var p in props)
                {
                    var nameLC = p.Name.ToLower();
                    var attr = (FilterAttribute)p.GetCustomAttribute(typeof(FilterAttribute));
                    if (attr != null && filter.ContainsKey(nameLC) && filter[nameLC].Any())
                    {
                        Logger.LogTrace("..... Filter {name} type: {type} ", nameLC, p.PropertyType.Name);
                        try
                        {
                            //var prop = entityType.FindProperty(p.Name);
                            switch (p.PropertyType.Name)
                            {
                                case "Boolean":
                                    //case "bool":
                                    var values = filter[nameLC].Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => bool.Parse(i)).Distinct().ToList();
                                    Logger.LogTrace("..... ==> Boolean filter for {name} args: {args} => {boolargs}", nameLC, string.Join(", ", filter[nameLC]), string.Join(", ", values.Select(i=>i.ToString())));
                                    if (values.Count == 1) // другие случаи означают все варианты
                                    {
                                        // такой вариант не работает с БД (точнее выполняется на клиенте, а не на стороне БД)
                                        // до кучи это решение в 10 раз медленнее!!!
                                        //query = query.Where(i => (bool)p.GetValue(i, null) == values[0]);
                                        query = query.Where(ExpressionHelper.ComparePropertyWithConst<T, bool>(p.Name, values[0]));//.AsQueryable();

                                    }
                                    break;
                                case "":
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogWarning("Couldn't apply filter {name} with arguments: {args}\n{exception}", nameLC, string.Join(", ", filter[nameLC]), e.ToString());
                            // сделать бы перевод сообщения
                            // эта ошибка может возникнуть только при ручном указании параметров
                            //throw new Exception(string.Format(("Couldn't apply filter {name} with arguments: {args}"), attr.Title, string.Join(", ", filter[nameLC]) ));
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

        public virtual void ClearCache()
        {
            Logger.LogTrace("Repository ClearCache {0}", this.GetType().FullName);
        }

    }
}
