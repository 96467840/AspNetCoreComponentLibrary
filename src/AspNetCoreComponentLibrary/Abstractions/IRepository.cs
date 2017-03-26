using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface IRepository
    {
        void SetStorageContext(IStorageContext storageContext);
    }
}
