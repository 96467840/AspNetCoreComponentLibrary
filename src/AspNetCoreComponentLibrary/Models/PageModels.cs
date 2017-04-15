﻿using AspNetCoreComponentLibrary.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        
        public IActionResult ToActionResult(Controller2Garin controller)
        {
            var Logger = controller.LoggerFactory.CreateLogger(this.GetType().FullName);
            var vm = new PageVM(controller);
            try
            {
                Logger.LogInformation("Begin '{page}' in lang '{lang}'", page, lang);
                var storage = controller.Storage;
                var sites = controller.Sites;//storage.GetRepository<ISiteRepository>(false);
                var menus = controller.Menus;// storage.GetRepository<IMenuRepository>(true);

                //var newitem = new Sites { Name = "Supper Site " + DateTime.Now };
                //sites.Save(newitem);

                var site = sites[2];
                if (site != null)
                {
                    site.Name = "New name 2 " + DateTime.Now;
                    sites.Save(site);
                }
                //vm.Sites = rep.StartQuery().Where(i => i.Id < 30).ToList();
                vm.Sites = sites.StartQuery().Where(i => i.Id < 10).OrderByDescending(i => i.Id).ToList();

                GC.Collect();
                Logger.LogInformation("Memory used after full collection:   {0:N0}", GC.GetTotalMemory(true));
            }
            catch (Exception e)
            {
                vm.Error = e;
            }
            return controller.View(vm);
        }
    }
}
