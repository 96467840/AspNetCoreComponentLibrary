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
        BreadcrumbVM Breadcrumb { get; set; }
    }

    public class BaseVM: IBaseVM
    {
        public Controller2Garin Controller { get; set; }
        public Exception Error { get; set; }
        public BreadcrumbVM Breadcrumb { get; set; }

        public BaseVM(Controller2Garin controller)
        {
            Controller = controller;
            Breadcrumb = new BreadcrumbVM(new List<BreadcrumbItem>());
        }
    }
}
