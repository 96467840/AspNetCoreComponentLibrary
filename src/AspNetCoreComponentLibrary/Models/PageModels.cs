using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class PageVM: BaseVM
    {
        public BreadcrumbVM Breadcrumb { get; set; }

        public PageVM()
        {
            Breadcrumb = new BreadcrumbVM(new List<BreadcrumbItem>());
        }
    }

    public class PageIM : BaseIM
    {
        public string lang { get; set; }
        public string page { get; set; }

        public IActionResult ToActionResult(Controller2Garin controler)
        {
            var vm = new PageVM();

            return controler.View(vm);
        }
    }
}
