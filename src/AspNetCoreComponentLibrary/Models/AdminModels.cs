using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public interface IAdminVM : IBaseVM {

    }

    public class AdminVM : BaseVM
    {
        public AdminVM(Controller2Garin controller) : base(controller)
        {
            //if ()
        }

        protected bool CheckRights()
        {
            return true;
        }
    }
}
