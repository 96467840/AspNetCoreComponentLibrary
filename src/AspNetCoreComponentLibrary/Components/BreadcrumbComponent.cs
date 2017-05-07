using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class BreadcrumbComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(MenuVM vm)
        {
            //var vm = new TestVM() { Text = text };
            return View(vm);
        }
    }
}
