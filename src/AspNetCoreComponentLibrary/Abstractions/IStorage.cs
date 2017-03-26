using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface IStorage
    {
        T GetRepository<T>() where T : IRepository;
        void Save();
    }
}
