using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreComponentLibrary
{
    public abstract class RepositoryWithCache<K, T> : Repository<K, T> where T : BaseDM<K>
    {
        protected static Dictionary<K, T> coll;

        public virtual void LoadFromDB()
        {
            using (new BLog(LoggerMEF, "LoadFromDB", GetType().FullName))
            {
                coll = DbSet
                    // очень важный момент!
                    .AsNoTracking()
                    .ToDictionary(i => i.Id, i => i);
            }
        }

        protected void CheckColl()
        {
            if (coll == null) LoadFromDB();
            if (coll == null) throw new Exception(string.Format("Can't load collection {0} from DB", typeof(T).FullName));
        }

        public override IQueryable<T> StartQuery()
        {
            CheckColl();
            return coll.Values.AsQueryable();
        }

        public override T this[K index]
        {
            get
            {
                CheckColl();
                if (!coll.ContainsKey(index)) return default(T);
                return coll[index];
            }
        }

        public override void Save(T item)
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
            
            CheckColl();
        }

        public override void AfterSave(T item, bool isnew)
        {
            base.AfterSave(item, isnew);
            Logger.LogTrace("RepositoryWithCache AfterSave for {0}", item.Id);
            AddToCache(item.Id);
        }/**/

        public override void Remove(T item)
        {
            RemoveFromCache(item.Id);
            DbSet.Remove(item);
        }

        protected override void SetBlock(K id, bool value)
        {
            if (typeof(T) is IBlockable)
            {
                T item = (T)Activator.CreateInstance(typeof(T));
                item.Id = id;
                ((IBlockable)item).IsBlocked = value;
                DbSet.Update(item);
            }
        }

        public bool Contains(K index)
        {
            CheckColl();
            if (index == null) return false;
            return coll.ContainsKey(index);
        }

        public void Clear()
        {
            CheckColl();
            coll.Clear();
        }

        protected void RemoveFromCache(K index)
        {
            CheckColl();
            try
            {
                var item = this[index];
                if (item == null) return;
                coll.Remove(index);
            }
            catch { }
        }

        protected virtual void BeforeAddToCache(T item)
        {
        }/**/

        protected void AddToCache(K index)
        {
            CheckColl();

            var newitem = GetSingleFromDB(index);

            BeforeAddToCache(newitem);

            if (coll.ContainsKey(index)) coll[index] = newitem;
            else coll.Add(index, newitem);
        }


    }
}
