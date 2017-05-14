using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface ILanguageRepository : IRepositorySetStorageContext, IRepository<long, Languages>
    {
    }
}
