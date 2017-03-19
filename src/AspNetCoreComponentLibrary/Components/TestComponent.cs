using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class TestComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var vm = new TestVM() { Text = "Hello from component" };
            return View(vm);

        }
    }
}
