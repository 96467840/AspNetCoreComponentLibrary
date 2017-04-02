using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Extensions.Logging;

namespace AspNetCoreComponentLibrary
{
    public abstract class RepositoryWithCache<K, T> : Repository<K, T>, IEnumerable<T> where K : struct
    {
        protected static Dictionary<K, T> Coll { get; set; }

        public abstract void LoadFromDB();

        protected void CheckColl()
        {
            if (Coll == null) LoadFromDB();
            if (Coll == null) throw new Exception(string.Format("Can't load collection {0} from DB", typeof(T).FullName));
        }

        public T this[K? index]
        {
            get
            {
                CheckColl();
                if (index == null) return default(T);
                if (!Coll.ContainsKey(index.Value)) return default(T);
                return Coll[index.Value];
            }
        }

        public bool Contains(K? index)
        {
            CheckColl();
            if (index == null) return false;
            return Coll.ContainsKey(index.Value);
        }

        public void Clear()
        {
            CheckColl();
            Coll.Clear();
        }

        public void RemoveFromCache(K? index)
        {
            CheckColl();
            try
            {
                var item = this[index];
                if (item == null) return;
                Coll.Remove(index.Value);
            }
            catch { }
        }

        public void AddToCache(K index, T item)
        {
            CheckColl();
            if (Coll.ContainsKey(index)) Coll[index] = item;
            else Coll.Add(index, item);
        }

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
    }
}
