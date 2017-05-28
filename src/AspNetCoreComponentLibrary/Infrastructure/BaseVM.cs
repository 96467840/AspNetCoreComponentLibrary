using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public interface IBaseVM
    {
        Controller2Garin Controller { get; set; }
        Exception Error { get; set; }
        MenuVM Breadcrumb { get; set; }
        Sites Site { get; }
        Users SessionUser { get; }
        SEO SEO { get; }
    }

    public class BaseVM: IBaseVM
    {
        public Controller2Garin Controller { get; set; }
        public Exception Error { get; set; }
        public MenuVM Breadcrumb { get; set; }
        public Sites Site { get { return Controller?.Site; }  }
        public Users SessionUser { get { return Controller?.SessionUser; }  }
        public SEO SEO { get; set; }

        public BaseVM(Controller2Garin controller)
        {
            Controller = controller;
            Breadcrumb = new MenuVM(new List<MenuItem>());
        }
    }
}
