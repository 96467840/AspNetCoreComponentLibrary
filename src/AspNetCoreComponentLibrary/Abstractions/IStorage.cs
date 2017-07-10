using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public enum EnumDB
    {
        UserSites = 1,
        Content = 2
    }

    public interface IStorage
    {
        T GetRepository<T>(EnumDB db, bool enableCache = true) where T : IRepositorySetStorageContext;
        IGetOptions GetOptionsRepository(Type type, EnumDB db, bool enableCache = true);

        IStorageContext GetContextForSite(long siteid);

        void ConnectToSiteDB(long siteid);

        void Save(EnumDB db);

        void UpdateDBs();
    }
}
