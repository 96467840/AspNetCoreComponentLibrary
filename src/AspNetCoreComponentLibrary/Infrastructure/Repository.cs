using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    public abstract class Repository<K, T> where K : struct
    {

        protected IStorageContext StorageContext;
        protected IStorage Storage { get; set; }

        public void SetStorageContext(IStorageContext storageContext, IStorage storage)
        {
            StorageContext = storageContext;
            Storage = storage;
        }

        public abstract K Save(T item);
        public abstract void Remove(K id);

        public abstract void SetBlock(K id, bool value);

        public void Block(K id) { SetBlock(id, true); }
        public void UnBlock(K id) { SetBlock(id, false); }

    }
}
