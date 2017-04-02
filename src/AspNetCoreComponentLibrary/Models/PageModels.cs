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

        public List<Sites> Sites { get; set; }

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
                var rep = storage.GetRepository<ISiteRepository>();
                var newid = rep.Save(new Sites { Name = "Supper Site " + DateTime.Now });

                vm.Sites = rep.StartQuery().Where(i => i.Id < 30).ToList();
                //vm.Sites = rep.Where(i=>i.Id>10).OrderByDescending(i=>i.Id).ToList();
            }
            catch (Exception e)
            {
                vm.Error = e;
            }
            return controler.View(vm);
        }
    }
}
