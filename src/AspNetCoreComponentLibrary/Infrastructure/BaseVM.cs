using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class BaseVM
    {
        public Controller2Garin Controller { get; set; }
        public string Error { get; set; }

        public BaseVM(Controller2Garin controller)
        {
            Controller = controller;
        }
    }
}
