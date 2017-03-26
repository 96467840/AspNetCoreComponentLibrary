using AspNetCoreComponentLibrary.Abstractions;
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

        public List<Site> Sites { get; set; }

        public PageVM(Controller2Garin controler):base(controler)
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
            var vm = new PageVM(controler);
            try
            {
                var storage = controler.Storage;
                vm.Sites = storage.GetRepository<ISiteRepository>().All().ToList();
            }
            catch (Exception e)
            {
                vm.Error = e.ToString();
            }
            return controler.View(vm);
        }
    }
}
