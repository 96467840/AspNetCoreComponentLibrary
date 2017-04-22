using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace AspNetCoreComponentLibrary
{
    public abstract class RepositoryWithCache<K, T> : Repository<K, T>/*, IEnumerable<T>*/ where K : struct where T : BaseDM<K>
    {
        protected static Dictionary<K, T> coll;

        public void LoadFromDB()
        {
            //(Storage as Storage)._logger.LogInformation("LoadFromDB");
            coll = this.DbSet.ToDictionary(i => i.Id.Value, i => i);
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

        public override T this[K? index]
        {
            get
            {
                CheckColl();
                if (index == null) return default(T);
                if (!coll.ContainsKey(index.Value)) return default(T);
                return coll[index.Value];
            }
        }

        public override void Save(T item)
        {
            if (item == null) throw new ArgumentNullException();
            if (!BeforeSave(item)) return;
            var isnew = !item.Id.HasValue;
            if (item.Id.HasValue)
            {
                DbSet.Update(item);
            }
            else
            {
                DbSet.Add(item);
            }
            Storage.Save();

            CheckColl();
            if (item.Id.HasValue) coll[item.Id.Value] = item;
            AfterSave(item, isnew);
        }

        public override void Remove(K id)
        {
            var item = this[id]; // CheckColl() here
            if (item == null) return;
            DbSet.Remove(item);
            Storage.Save();
            RemoveFromCache(id);
        }

        protected override void SetBlock(K id, bool value)
        {
            if (typeof(T) is IBlockable)
            {
                T item = (T)Activator.CreateInstance(typeof(T));
                item.Id = id;
                ((IBlockable)item).IsBlocked = value;
                DbSet.Update(item);
                Storage.Save();

                CheckColl();
                coll[id] = item;
            }
        }
        public bool Contains(K? index)
        {
            CheckColl();
            if (index == null) return false;
            return coll.ContainsKey(index.Value);
        }

        public void Clear()
        {
            CheckColl();
            coll.Clear();
        }

        public void RemoveFromCache(K? index)
        {
            CheckColl();
            try
            {
                var item = this[index];
                if (item == null) return;
                coll.Remove(index.Value);
            }
            catch { }
        }

        public void AddToCache(K index, T item)
        {
            CheckColl();
            if (coll.ContainsKey(index)) coll[index] = item;
            else coll.Add(index, item);
        }

        /*
        #region [   IEnumerable<T>   ]

        public IEnumerator<T> GetEnumerator()
        {
            return Coll.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
        /**/
    }
}
