using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public interface IBaseDM
    {
        bool IsBlockable { get; }
    }

    public abstract class BaseDM<K> : IBaseDM
    {
        public K Id { get; set; }

        public bool IsBlockable
        {
            get
            {
                return GetType().IsImplementsInterface(typeof(IBlockable));
            }
        }
    }
}
