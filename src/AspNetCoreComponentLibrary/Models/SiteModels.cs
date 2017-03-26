using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class Site : BaseDM
    {
        public string Name { get; set; }

        public override void FromDB()
        {
            throw new NotImplementedException();
        }
    }
}
