using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public abstract class BaseDM
    {
        public long? Id { get; set; }

        public abstract void FromDB();

    }
}
