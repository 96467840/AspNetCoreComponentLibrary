using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.Extensions.Logging;

namespace AspNetCoreComponentLibrary
{
    public abstract class BaseRepository<K, T> : IEnumerable<T> where K : struct
    {
        protected IStorageContext StorageContext;
        protected IStorage Storage { get; set; }

        protected static Dictionary<K, T> Coll { get; set; }

        public void SetStorageContext(IStorageContext storageContext, IStorage storage)
        {
            StorageContext = storageContext;
            Storage = storage;
        }

        public abstract K? Save(T item);

        public T this[K? index]
        {
            get
            {
                if (index == null) return default(T);
                if (!Coll.ContainsKey(index.Value)) return default(T);
                return Coll[index.Value];
            }
        }

        public bool Contains(K? index)
        {
            if (index == null) return false;
            return Coll.ContainsKey(index.Value);
        }

        public void Clear()
        {
            Coll.Clear();
        }

        public void RemoveFromCache(K? index)
        {
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
