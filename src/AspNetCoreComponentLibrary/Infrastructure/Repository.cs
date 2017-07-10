using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public abstract class Repository<K, T> : IGetOptions, IRepository<K, T> /*where K : struct/*, IComparable<K>*/ where T : BaseDM<K>
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

        #region Translates

        protected virtual string TranslatesTable => "Translates";

        /// <summary>
        /// В теории переписав эту функцию в потомках можно отменить перевод.
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="siteLanguages"></param>
        /// <returns></returns>
        protected virtual bool NeedTranslate(Languages lang, List<Languages> siteLanguages)
        {
            if (lang == null || lang.IsDefault) return false;
            if (siteLanguages == null || !siteLanguages.Any()) return false;
            return true;
        }

        public void TranslateItem(long siteid, Languages lang, List<Languages> siteLanguages, T item) //ref T item
        {
            if (!NeedTranslate(lang, siteLanguages)) return;
        }

        public void TranslateList(long siteid, Languages lang, List<Languages> siteLanguages, List<T> items) //ref T item
        {
            if (!NeedTranslate(lang, siteLanguages)) return;
        }
        #endregion

        private List<OptionVM> GetTreeOptions(IQueryable<T> query, PropertyInfo propValue, PropertyInfo propTitle, PropertyInfo propParent, object parent, string SelectTreePrefix, string prefix="")
        {
            var res = new List<OptionVM>();
            var subquery = query.Where(ExpressionHelper.ComparePropertyWithConst<T>(propParent.Name, propParent.PropertyType, parent, Expression.Equal));
            foreach (var val in subquery)
            {
                // а вот тут что лучше использовать ToStringVM() или ToString(). По идее индекс всегда лонг или на крайний случай стринг, так что делаю ToString, к тому же лень прокинуть формат
                var value = propValue.GetValue(val, null).ToString();
                var title = propTitle.GetValue(val, null).ToString();
                res.Add(new OptionVM(value, title, parent?.ToString()));
            }
            return res;
        }

        public List<OptionVM> GetOptions(long siteid, string SelectValueName, string SelectTitleName, string SelectParentName, string SelectTreePrefix, bool SelectOnlyUnblocked)
        {
            //if (string.IsNullOrWhiteSpace(SelectValueName)) return null;
            if (string.IsNullOrWhiteSpace(SelectTitleName)) throw new ArgumentNullException(SelectTitleName);
            var propValue = typeof(T).GetProperty(SelectValueName ?? "Id");
            var propTitle = typeof(T).GetProperty(SelectTitleName);
            if (propValue == null) throw new Exception("Cannot find entitiy properties " + SelectValueName ?? "Id");
            if (propTitle == null) throw new Exception("Cannot find entitiy properties " + SelectTitleName);

            var query = SelectOnlyUnblocked ? GetUnblocked(siteid) : StartQuery(siteid);
            query = query.SetDefaultOrder();

            if (!string.IsNullOrWhiteSpace(SelectParentName))
            {
                var propParent = typeof(T).GetProperty(SelectParentName);
                if (propParent == null) throw new Exception("Cannot find entitiy properties " + SelectParentName);
                return GetTreeOptions(query, propValue, propTitle, propParent, null, SelectTreePrefix);
            }

            var res = new List<OptionVM>();
            foreach (var val in query)
            {
                // а вот тут что лучше использовать ToStringVM() или ToString(). По идее индекс всегда лонг или на крайний случай стринг, так что делаю ToString, к тому же лень прокинуть формат
                var value = propValue.GetValue(val, null).ToString();
                var title = propTitle.GetValue(val, null).ToString();
                res.Add(new OptionVM(value, title, null));
            }
            //TranslateList()
            return res;
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
                foreach (var field in form.Fields)
                {
                    var nameLC = field.PropertyName;

                    Logger.LogTrace("..... Filter {name} type: {type} ", nameLC, field.Type.Name);

                    var property = typeof(T).GetProperty(field.PropertyName);
                    if (property == null || property.PropertyType != field.Type) continue;
                    
                    var values = field.GetValueAsObject();
                    if (values == null || !values.Any()) continue;
                    
                    query = query.Where(ExpressionHelper.ComparePropertyWithConstArray<T>(nameLC, field.Type, values, field.Compare.GetCompareExpression()));
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
