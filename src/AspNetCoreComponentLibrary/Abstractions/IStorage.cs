using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface IStorage
    {
        T GetRepository<T>(bool SiteStorage) where T : IRepositorySetStorageContext;

        IStorageContext GetContextForSite(long siteid);

        void ConnectToSiteDB(long siteid);

        void Save();

        void UpdateDBs();
    }
}
