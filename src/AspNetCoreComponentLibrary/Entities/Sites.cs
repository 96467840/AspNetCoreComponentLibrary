using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class Sites : BaseDM<long>
    {
        public string Name { get; set; }
        public DateTime Created { get; set; }

        //public Sites(IStorage storage) : base(storage) { }

        /*public override void FromDB()
        {
            throw new NotImplementedException();
        }

        public long Save()
        {
            //var rep = Storage.GetRepository<ISiteRepository>();

            if (Id > 0)
            {

            }
            return 0;
        }*/
    }

    /*public class User : BaseDM
    {
        public string Name { get; set; }

        public override void FromDB()
        {
            throw new NotImplementedException();
        }
    }*/
}
