using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class AlertComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(AlertVM vm)
        {
            return View(vm);
        }
    }
}
